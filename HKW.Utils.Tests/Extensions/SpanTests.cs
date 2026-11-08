using System.Buffers;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class SpanTests
{
    [TestMethod]
    [DataRow(2048)]
    [DataRow(2049)]
    [DataRow(4097)]
    public void PascalUsesOnlyInputLength(int length)
    {
        var input = new string('A', length);
        var expected = "A" + new string('a', length - 1);
        Assert.AreEqual(expected, input.AsSpan().ToPascal('_', true, CultureInfo.InvariantCulture));
        Assert.AreEqual(
            expected,
            input.AsSpan().ToPascal(SearchValues.Create("_"), true, CultureInfo.InvariantCulture)
        );
        Assert.AreEqual(
            expected,
            input.ToCharArray().AsSpan().ToPascal('_', true, CultureInfo.InvariantCulture)
        );
        Assert.AreEqual(
            expected,
            input
                .ToCharArray()
                .AsSpan()
                .ToPascal(SearchValues.Create("_"), true, CultureInfo.InvariantCulture)
        );
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("_", "")]
    [DataRow("___", "")]
    [DataRow("_rED__bLUE_", "RedBlue")]
    [DataRow("rED", "Red")]
    public void PascalSkipsEmptySegments(string input, string expected)
    {
        Assert.AreEqual(expected, input.AsSpan().ToPascal('_', true, CultureInfo.InvariantCulture));
        Assert.AreEqual(
            expected,
            input.AsSpan().ToPascal(SearchValues.Create("_"), true, CultureInfo.InvariantCulture)
        );
        Assert.AreEqual(
            expected,
            input.ToCharArray().AsSpan().ToPascal('_', true, CultureInfo.InvariantCulture)
        );
        Assert.AreEqual(
            expected,
            input
                .ToCharArray()
                .AsSpan()
                .ToPascal(SearchValues.Create("_"), true, CultureInfo.InvariantCulture)
        );
    }

    [TestMethod]
    public void PascalPreservesCultureAndExistingOverloadDefaults()
    {
        var culture = CultureInfo.GetCultureInfo("tr-TR");
        Assert.AreEqual("İİ", "i_i".AsSpan().ToPascal('_', false, culture));
        Assert.AreEqual("İİ", "i_i".AsSpan().ToPascal(SearchValues.Create("_"), false, culture));
        Assert.AreEqual("Red", "rED".AsSpan().ToPascal('_'));
        Assert.AreEqual("RED", "rED".ToCharArray().AsSpan().ToPascal('_'));
        Assert.AreEqual("Red", "rED".AsSpan().ToPascal(SearchValues.Create("_")));
        Assert.AreEqual("RED", "rED".ToCharArray().AsSpan().ToPascal(SearchValues.Create("_")));
    }

    [TestMethod]
    public void LineDocumentationIncludesTrailingEmptySegments()
    {
        const string input = "a\n b\r c\r\n d\n\r e\r\r f\n\n \n";
        string[][] expected =
        [
            ["a", " b", " c", " d", "", " e", "", " f", "", " ", ""],
            ["a", " b", " c", " d", " e", " f", " "],
            ["a", "b", "c", "d", "", "e", "", "f", "", "", ""],
            ["a", "b", "c", "d", "e", "f"],
        ];
        for (var option = 0; option < expected.Length; option++)
        {
            var lines = input.AsSpan().SplitLine((StringSplitOptions)option);
            CollectionAssert.AreEqual(
                expected[option],
                lines.ToList(x => x.CurrentValue.ToString())
            );
        }
    }

    [TestMethod]
    public void TryGetSpanIsAStorageViewRatherThanSnapshot()
    {
        var values = new List<string>(4) { "a", "b" };
        Assert.IsTrue(values.TryGetSpan(out var view));
        Assert.AreEqual(2, view.Length);
        values[0] = "changed";
        Assert.AreEqual("changed", view[0]);
        var snapshot = values.ToArray();
        // 验证扩容不会使已有区块自动跟随新存储。
        values.Capacity = 8;
        values[0] = "new storage";
        Assert.AreEqual("changed", view[0]);
        Assert.AreEqual("changed", snapshot[0]);
        Assert.IsTrue(values.TryGetSpan(out var current));
        // 验证清空引用元素会影响同一底层数组的区块。
        values.Clear();
        Assert.AreEqual(2, current.Length);
        Assert.IsNull(current[0]);
        Assert.IsNull(current[1]);
        Assert.AreEqual("changed", snapshot[0]);
    }

    // 使用私有核心注入独立池，避免污染共享池或依赖其桶复用顺序。
    private delegate string PascalCore<TSeparator>(
        ReadOnlySpan<char> input,
        TSeparator separator,
        bool lower,
        CultureInfo? culture,
        ArrayPool<char> pool
    );

    private static PascalCore<TSeparator> GetPascalCore<TSeparator>()
    {
        var method = typeof(ReadOnlySpanExtensions).GetMethod(
            "ToPascalCore",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
            new[]
            {
                typeof(ReadOnlySpan<char>),
                typeof(TSeparator),
                typeof(bool),
                typeof(CultureInfo),
                typeof(ArrayPool<char>),
            }
        );
        Assert.IsNotNull(method);
        return method.CreateDelegate<PascalCore<TSeparator>>();
    }

    private static string ConvertWithPool(
        string input,
        bool searchValues,
        bool lower,
        CultureInfo culture,
        ArrayPool<char> pool
    )
    {
        return searchValues
            ? GetPascalCore<SearchValues<char>>()(
                input.AsSpan(),
                SearchValues.Create("_"),
                lower,
                culture,
                pool
            )
            : GetPascalCore<char>()(input.AsSpan(), '_', lower, culture, pool);
    }

    private sealed class TrackingPool : ArrayPool<char>
    {
        public int RentCalls { get; private set; }
        public int FailOnRent { get; init; }
        public List<char[]> Rented { get; } = [];
        public List<char[]> Returned { get; } = [];

        public override char[] Rent(int minimumLength)
        {
            RentCalls++;
            if (RentCalls == FailOnRent)
                throw new InvalidOperationException("Expected rent failure.");
            var buffer = new char[minimumLength + 137];
            Array.Fill(buffer, 'Z');
            Rented.Add(buffer);
            return buffer;
        }

        public override void Return(char[] array, bool clearArray = false)
        {
            Assert.IsTrue(Rented.Contains(array));
            Assert.IsFalse(Returned.Contains(array));
            Returned.Add(array);
            Array.Clear(array);
        }
    }

    private sealed class ThrowingCulture : CultureInfo
    {
        public ThrowingCulture()
            : base("en-US") { }

        public override TextInfo TextInfo =>
            throw new InvalidOperationException("Expected culture failure.");
    }

    [TestMethod]
    [DataRow(2048, false)]
    [DataRow(2049, false)]
    [DataRow(4097, false)]
    [DataRow(2048, true)]
    [DataRow(2049, true)]
    [DataRow(4097, true)]
    public void PascalIgnoresControlledNonzeroPoolTailAndReturnsBuffers(
        int length,
        bool searchValues
    )
    {
        foreach (var lower in new[] { false, true })
        {
            var pool = new TrackingPool();
            try
            {
                var input = "_" + new string('A', length - 2) + "_";
                var expected = "A" + new string(lower ? 'a' : 'A', length - 3);
                var actual = ConvertWithPool(
                    input,
                    searchValues,
                    lower,
                    CultureInfo.InvariantCulture,
                    pool
                );
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(length - 2, actual.Length);
                Assert.AreEqual(length > 2048 ? 2 : 0, pool.RentCalls);
                CollectionAssert.AreEquivalent(pool.Rented, pool.Returned);
            }
            finally
            {
                foreach (var buffer in pool.Rented)
                    Array.Clear(buffer);
            }
        }
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(true, false)]
    [DataRow(false, true)]
    [DataRow(true, true)]
    public void PascalReturnsBuffersWhenCultureThrows(bool searchValues, bool lower)
    {
        var pool = new TrackingPool();
        try
        {
            var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
                ConvertWithPool(
                    new string('a', 2049),
                    searchValues,
                    lower,
                    new ThrowingCulture(),
                    pool
                )
            );
            Assert.AreEqual("Expected culture failure.", exception.Message);
            Assert.AreEqual(2, pool.Rented.Count);
            CollectionAssert.AreEquivalent(pool.Rented, pool.Returned);
        }
        finally
        {
            foreach (var buffer in pool.Rented)
                Array.Clear(buffer);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void PascalReturnsFirstBufferWhenSecondRentFails(bool searchValues)
    {
        var pool = new TrackingPool { FailOnRent = 2 };
        try
        {
            var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
                ConvertWithPool(
                    new string('a', 2049),
                    searchValues,
                    false,
                    CultureInfo.InvariantCulture,
                    pool
                )
            );
            Assert.AreEqual("Expected rent failure.", exception.Message);
            Assert.AreEqual(1, pool.Rented.Count);
            CollectionAssert.AreEquivalent(pool.Rented, pool.Returned);
        }
        finally
        {
            foreach (var buffer in pool.Rented)
                Array.Clear(buffer);
        }
    }

    [TestMethod]
    public void CurrentSeparatorIncludesAdjacentAndTrailingSeparators()
    {
        var split = ",a,,b,".AsSpan().Split(',');
        var values = new List<string>();
        var separators = new List<char>();
        while (split.MoveNext())
        {
            values.Add(split.CurrentValue.ToString());
            separators.Add(split.CurrentSeparator);
        }
        CollectionAssert.AreEqual(new[] { "", "a", "", "b", "" }, values);
        CollectionAssert.AreEqual(new[] { ',', ',', ',', ',', '\0' }, separators);
        var multi = "a::b".AsSpan().Split("::".AsSpan());
        Assert.IsTrue(multi.MoveNext());
        Assert.AreEqual(':', multi.CurrentSeparator);
        Assert.IsTrue(multi.MoveNext());
        Assert.AreEqual('\0', multi.CurrentSeparator);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(-1)]
    [DataRow(7)]
    public void SplitRejectsUnknownOptions(int option)
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = "a".AsSpan().Split(SearchValues.Create(","), (StringSplitOptions)option);
        });
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = "a".AsSpan().SplitLine((StringSplitOptions)option);
        });
    }

    [TestMethod]
    public void TrimPreservesOriginalRangesAndResetClearsValues()
    {
        var chars = " a , b "
            .AsSpan()
            .Split(SearchValues.Create(","), StringSplitOptions.TrimEntries);
        Assert.IsTrue(chars.MoveNext());
        Assert.AreEqual(" a ", chars.Source[chars.Current].ToString());
        Assert.AreEqual("a", chars.CurrentValue.ToString());
        chars.Reset();
        Assert.AreEqual(0..0, chars.Current);
        Assert.IsTrue(chars.CurrentValue.IsEmpty);
        Assert.IsTrue(chars.MoveNext());
        Assert.AreEqual("a", chars.CurrentValue.ToString());
        var lines = " a \r\n b \n".AsSpan().SplitLine(StringSplitOptions.TrimEntries);
        Assert.IsTrue(lines.MoveNext());
        Assert.AreEqual(" a ", lines.Source[lines.Current].ToString());
        Assert.AreEqual("a", lines.CurrentValue.ToString());
        Assert.AreEqual("\r\n", lines.CurrentLineChars.ToString());
        lines.Reset();
        Assert.AreEqual(0..0, lines.Current);
        Assert.IsTrue(lines.CurrentValue.IsEmpty);
        Assert.IsTrue(lines.CurrentLineChars.IsEmpty);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void SplitBoundaryMatrixMatchesStringSplit(int value)
    {
        var options = (StringSplitOptions)value;
        foreach (var input in new[] { "", ",", ",,", " a ,, b ,", "plain", "  " })
        {
            var chars = input.AsSpan().Split(SearchValues.Create(","), options);
            CollectionAssert.AreEqual(
                input.Split(',', options),
                chars.ToList(x => x.CurrentValue.ToString())
            );
            Assert.IsTrue(chars.CurrentValue.IsEmpty);
            Assert.AreEqual(0..0, chars.Current);
        }
        foreach (
            var input in new[]
            {
                "",
                "\r\n",
                "\n\r",
                "a\n",
                " a\r\n \r\nb\r\n",
                "a\n b\r c\r\n d\n\r e\r\r f\n\n \n",
            }
        )
        {
            var lines = input.AsSpan().SplitLine(options);
            CollectionAssert.AreEqual(
                input.Split(new[] { "\r\n", "\r", "\n" }, options),
                lines.ToList(x => x.CurrentValue.ToString())
            );
            Assert.IsTrue(lines.CurrentValue.IsEmpty);
            Assert.AreEqual(0..0, lines.Current);
        }
    }

    [TestMethod]
    public void ToListRestartsFromBeginningAndRejectsNullConverters()
    {
        var chars = "a,b,".AsSpan().Split(SearchValues.Create(","), StringSplitOptions.None);
        Assert.IsTrue(chars.MoveNext());
        CollectionAssert.AreEqual(
            new[] { "a", "b", "" },
            chars.ToList(x => x.CurrentValue.ToString())
        );
        Assert.IsTrue(chars.CurrentValue.IsEmpty);
        var lines = "a\r\nb\n".AsSpan().SplitLine();
        Assert.IsTrue(lines.MoveNext());
        CollectionAssert.AreEqual(
            new[] { "a", "b", "" },
            lines.ToList(x => x.CurrentValue.ToString())
        );
        Assert.IsTrue(lines.CurrentValue.IsEmpty);
        var values = "a,b,".AsSpan().Split(SearchValues.Create(","));
        Assert.IsTrue(values.MoveNext());
        CollectionAssert.AreEqual(
            new[] { "a", "b", "" },
            values.ToList(x => x.CurrentValue.ToString())
        );
        Assert.IsTrue(values.CurrentValue.IsEmpty);
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ""
                .AsSpan()
                .Split(SearchValues.Create(","), StringSplitOptions.RemoveEmptyEntries)
                .ToList<string>(null!)
        );
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            "".AsSpan().SplitLine(StringSplitOptions.RemoveEmptyEntries).ToList<string>(null!)
        );
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            "".AsSpan().Split(SearchValues.Create(",")).ToList<string>(null!)
        );
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void ToListResetsAllEnumeratorsAfterConverterThrows(int failAt)
    {
        var calls = 0;
        var chars = "a,b".AsSpan().Split(SearchValues.Create(","), StringSplitOptions.None);
        try
        {
            chars.ToList(x =>
                ++calls == failAt
                    ? throw new InvalidOperationException("Expected callback failure.")
                    : x.CurrentValue.ToString()
            );
            Assert.Fail();
        }
        catch (InvalidOperationException) { }
        Assert.AreEqual(failAt, calls);
        Assert.IsTrue(chars.CurrentValue.IsEmpty);
        Assert.AreEqual(0..0, chars.Current);
        Assert.IsTrue(chars.MoveNext());
        Assert.AreEqual("a", chars.CurrentValue.ToString());

        calls = 0;
        var lines = "a\nb".AsSpan().SplitLine();
        try
        {
            lines.ToList(x =>
                ++calls == failAt
                    ? throw new InvalidOperationException("Expected callback failure.")
                    : x.CurrentValue.ToString()
            );
            Assert.Fail();
        }
        catch (InvalidOperationException) { }
        Assert.AreEqual(failAt, calls);
        Assert.IsTrue(lines.CurrentValue.IsEmpty);
        Assert.AreEqual(0..0, lines.Current);
        Assert.IsTrue(lines.MoveNext());
        Assert.AreEqual("a", lines.CurrentValue.ToString());

        calls = 0;
        var values = "a,b".AsSpan().Split(SearchValues.Create(","));
        try
        {
            values.ToList(x =>
                ++calls == failAt
                    ? throw new InvalidOperationException("Expected callback failure.")
                    : x.CurrentValue.ToString()
            );
            Assert.Fail();
        }
        catch (InvalidOperationException) { }
        Assert.AreEqual(failAt, calls);
        Assert.IsTrue(values.CurrentValue.IsEmpty);
        Assert.AreEqual(0..0, values.Current);
        Assert.IsTrue(values.MoveNext());
        Assert.AreEqual("a", values.CurrentValue.ToString());
    }
}
