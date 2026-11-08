using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

/// <summary>
/// 文件工具
/// </summary>
public static class FileUtils
{
    /// <summary>
    /// 比较两个文件
    /// </summary>
    /// <param name="file1">文件1</param>
    /// <param name="file2">文件2</param>
    /// <param name="chunkSize">缓冲区大小, 必须大于零</param>
    /// <returns>文件存在且字节内容相同时返回真</returns>
    /// <remarks>
    /// 比较期间调用方不得修改文件; 本方法不提供文件系统快照.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">缓冲区大小不大于零</exception>
    public static bool Compare(string file1, string file2, int chunkSize = 4096)
    {
        ArgumentNullException.ThrowIfNull(file1);
        ArgumentNullException.ThrowIfNull(file2);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
        var fileInfo1 = new FileInfo(file1);
        var fileInfo2 = new FileInfo(file2);
        return Compare(fileInfo1, fileInfo2, chunkSize);
    }

    /// <summary>
    /// 比较两个文件
    /// </summary>
    /// <param name="fileInfo1">文件1</param>
    /// <param name="fileInfo2">文件2</param>
    /// <param name="chunkSize">字节缓冲区大小, 必须大于零; 大于 4096 时将使用内存池</param>
    /// <returns>文件存在且字节内容相同时返回真</returns>
    /// <remarks>
    /// 刷新文件元数据后比较内容; 比较期间调用方不得修改文件, 不提供文件系统快照.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">缓冲区大小不大于零</exception>
    public static bool Compare(FileInfo fileInfo1, FileInfo fileInfo2, int chunkSize = 4096)
    {
        ArgumentNullException.ThrowIfNull(fileInfo1);
        ArgumentNullException.ThrowIfNull(fileInfo2);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
        fileInfo1.Refresh();
        fileInfo2.Refresh();
        if (fileInfo1.Exists is false || fileInfo2.Exists is false)
            return false;

        using var stream1 = fileInfo1.OpenRead();
        using var stream2 = fileInfo2.OpenRead();
        if (stream1.Length != stream2.Length)
            return false;
        return CompareStreams(stream1, stream2, chunkSize, ArrayPool<byte>.Shared);
    }

    private static bool CompareStreams(
        Stream stream1,
        Stream stream2,
        int chunkSize,
        ArrayPool<byte> pool
    )
    {
        byte[]? rented1 = null;
        byte[]? rented2 = null;
        try
        {
            Span<byte> buffer1 =
                chunkSize <= 4096
                    ? stackalloc byte[chunkSize]
                    : (rented1 = pool.Rent(chunkSize)).AsSpan(0, chunkSize);
            Span<byte> buffer2 =
                chunkSize <= 4096
                    ? stackalloc byte[chunkSize]
                    : (rented2 = pool.Rent(chunkSize)).AsSpan(0, chunkSize);
            while (true)
            {
                var count1 = stream1.ReadAtLeast(buffer1, chunkSize, throwOnEndOfStream: false);
                var count2 = stream2.ReadAtLeast(buffer2, chunkSize, throwOnEndOfStream: false);
                if (count1 != count2)
                    return false;
                if (count1 == 0)
                    return true;
                if (buffer1[..count1].SequenceEqual(buffer2[..count2]) is false)
                    return false;
            }
        }
        finally
        {
            if (rented1 is not null)
                pool.Return(rented1);
            if (rented2 is not null)
                pool.Return(rented2);
        }
    }
}
