using System.Buffers;
using System.Reflection;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class FileUtilsTests
{
    private string _directory = null!;
    private string _first = null!;
    private string _second = null!;

    [TestInitialize]
    public void Initialize()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"HKW.FileUtils.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _first = Path.Combine(_directory, "first.bin");
        _second = Path.Combine(_directory, "second.bin");
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_directory, true);

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void InvalidChunkSize_IsRejectedByBothOverloads(int chunkSize)
    {
        File.WriteAllBytes(_first, [1]);
        File.WriteAllBytes(_second, [2]);
        var pathError = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            FileUtils.Compare(_first, _second, chunkSize)
        );
        Assert.AreEqual("chunkSize", pathError.ParamName);
        var infoError = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            FileUtils.Compare(new FileInfo(_first), new FileInfo(_second), chunkSize)
        );
        Assert.AreEqual("chunkSize", infoError.ParamName);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(1, 4096)]
    [DataRow(4095, 4096)]
    [DataRow(4096, 4096)]
    [DataRow(4097, 4096)]
    [DataRow(8193, 4097)]
    [DataRow(16385, 8193)]
    public void ComparesOnlyFileContent(int length, int chunkSize)
    {
        var data = Enumerable.Range(0, length).Select(i => (byte)(i % 251)).ToArray();
        File.WriteAllBytes(_first, data);
        File.WriteAllBytes(_second, data);
        Assert.IsTrue(FileUtils.Compare(_first, _second, chunkSize));
        Assert.IsTrue(FileUtils.Compare(new FileInfo(_first), new FileInfo(_second), chunkSize));
        if (length > 0)
        {
            data[^1] ^= 1;
            File.WriteAllBytes(_second, data);
            Assert.IsFalse(FileUtils.Compare(_first, _second, chunkSize));
        }
    }

    [TestMethod]
    public void MissingOrDifferentLengthFiles_AreNotEqual()
    {
        Assert.IsFalse(FileUtils.Compare(_first, _second));
        File.WriteAllBytes(_first, [1]);
        Assert.IsFalse(FileUtils.Compare(_first, _second));
        File.WriteAllBytes(_second, [1, 2]);
        Assert.IsFalse(FileUtils.Compare(_first, _second));
    }

    private static readonly Func<Stream, Stream, int, ArrayPool<byte>, bool> CompareStreams =
        typeof(FileUtils)
            .GetMethod("CompareStreams", BindingFlags.NonPublic | BindingFlags.Static)!
            .CreateDelegate<Func<Stream, Stream, int, ArrayPool<byte>, bool>>();

    [TestMethod]
    public void PooledBuffers_IgnoreDirtyTailsAndHandleDifferentShortReads()
    {
        var data = Enumerable.Range(0, 8197).Select(i => (byte)(i % 251)).ToArray();
        using var first = new ShortReadStream(data, 7);
        using var second = new ShortReadStream(data, 13);
        var pool = new TrackingPool();
        Assert.IsTrue(CompareStreams(first, second, 5001, pool));
        Assert.AreEqual(2, pool.Returned.Count);
        Assert.IsTrue(pool.Rented.All(pool.Returned.Contains));
        Assert.IsTrue(first.LargestRequest <= 5001);
        Assert.IsTrue(second.LargestRequest <= 5001);
    }

    [TestMethod]
    public void ReadFailure_ReturnsBothBuffers()
    {
        using var first = new ShortReadStream([1], 1, true);
        using var second = new MemoryStream([1]);
        var pool = new TrackingPool();
        Assert.ThrowsExactly<IOException>(() => CompareStreams(first, second, 5001, pool));
        Assert.AreEqual(2, pool.Returned.Count);
        Assert.IsTrue(pool.Rented.All(pool.Returned.Contains));
    }

    [TestMethod]
    public void SecondRentFailure_ReturnsFirstBuffer()
    {
        using var first = new MemoryStream([1]);
        using var second = new MemoryStream([1]);
        var pool = new TrackingPool { FailSecondRent = true };
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            CompareStreams(first, second, 5001, pool)
        );
        Assert.AreEqual(1, pool.Returned.Count);
        Assert.AreSame(pool.Rented[0], pool.Returned[0]);
    }

    [TestMethod]
    public void OpenFailure_DisposesAlreadyOpenedFile()
    {
        File.WriteAllBytes(_first, [1]);
        File.WriteAllBytes(_second, [1]);
        using var exclusive = new FileStream(
            _second,
            FileMode.Open,
            FileAccess.Read,
            FileShare.None
        );
        Assert.ThrowsExactly<IOException>(() => FileUtils.Compare(_first, _second, 8193));
        using var firstExclusive = new FileStream(
            _first,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None
        );
        Assert.AreEqual(1L, firstExclusive.Length);
    }

    [TestMethod]
    public void CachedFileInfo_IsRefreshed()
    {
        var first = new FileInfo(_first);
        var second = new FileInfo(_second);
        Assert.IsFalse(first.Exists);
        Assert.IsFalse(second.Exists);
        File.WriteAllBytes(_first, [1]);
        File.WriteAllBytes(_second, [1]);
        Assert.IsTrue(FileUtils.Compare(first, second));
    }

    private sealed class ShortReadStream(byte[] data, int limit, bool throwOnRead = false)
        : MemoryStream(data)
    {
        public int LargestRequest { get; private set; }

        public override int Read(Span<byte> buffer)
        {
            LargestRequest = Math.Max(LargestRequest, buffer.Length);
            if (throwOnRead)
                throw new IOException("Controlled read failure.");
            return base.Read(buffer[..Math.Min(limit, buffer.Length)]);
        }
    }

    private sealed class TrackingPool : ArrayPool<byte>
    {
        public bool FailSecondRent { get; init; }
        public List<byte[]> Rented { get; } = [];
        public List<byte[]> Returned { get; } = [];

        public override byte[] Rent(int minimumLength)
        {
            if (FailSecondRent && Rented.Count == 1)
                throw new InvalidOperationException("Controlled rental failure.");
            var buffer = new byte[minimumLength + 17];
            buffer.AsSpan().Fill((byte)(Rented.Count + 1));
            Rented.Add(buffer);
            return buffer;
        }

        public override void Return(byte[] array, bool clearArray = false)
        {
            Assert.IsTrue(Rented.Contains(array));
            Assert.IsFalse(Returned.Contains(array));
            Returned.Add(array);
        }
    }
}
