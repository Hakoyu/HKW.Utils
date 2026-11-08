using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberComparisonTests
{
    public static IEnumerable<object[]> Matrix =>
        from type in NumberTestData.Numbers
        from sample in NumberTestData.Samples(type)
        select new object[] { type, sample };

    [TestMethod]
    [DynamicData(nameof(Matrix))]
    public void Compare_AllTypesAndEntryPoints_ReturnInt(Type type, string sample) =>
        NumberTestData.Run(typeof(NumberComparisonTests), nameof(CheckCompare), type, sample);

    private static void CheckCompare<T>(string sample)
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var (left, right) = NumberTestData.Operands<T>(sample);
        var type = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => left.CompareTo(right),
            () => NumberUtils.Compare<T>(left, right),
            () => NumberUtils.Compare(left, right, typeof(T)),
            () => NumberUtils.Compare(left, right, type),
            () => NumberUtils.CompareF<T>(left, right),
            () => NumberUtils.CompareF(left, right, typeof(T)),
            () => NumberUtils.CompareF(left, right, type)
        );
    }

    public static IEnumerable<object[]> RelationalMatrix =>
        from row in Matrix
        from symbol in new[] { "==", "!=", "<", ">", "<=", ">=" }
        select new object[] { row[0], row[1], symbol };

    [TestMethod]
    [DynamicData(nameof(RelationalMatrix))]
    public void CompareBy_AllTypesOperatorsAndEntryPoints(
        Type type,
        string sample,
        string symbol
    ) =>
        NumberTestData.Run(
            typeof(NumberComparisonTests),
            nameof(CheckCompareBy),
            type,
            sample,
            symbol
        );

    private static void CheckCompareBy<T>(string sample, string symbol)
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var (left, right) = NumberTestData.Operands<T>(sample);
        var type = NumberUtils.GetNumberType(typeof(T));
        var op = NumberUtils.GetComparisonOperatorType(symbol);
        NumberTestData.Check(
            () =>
                symbol switch
                {
                    "==" => left == right,
                    "!=" => left != right,
                    "<" => left < right,
                    ">" => left > right,
                    "<=" => left <= right,
                    ">=" => left >= right,
                    _ => throw new ArgumentException("Unknown operator."),
                },
            () => NumberUtils.CompareBy<T>(left, right, symbol),
            () => NumberUtils.CompareBy<T>(left, right, op),
            () => NumberUtils.CompareBy(left, right, typeof(T), symbol),
            () => NumberUtils.CompareBy(left, right, typeof(T), op),
            () => NumberUtils.CompareBy(left, right, type, symbol),
            () => NumberUtils.CompareBy(left, right, type, op),
            () => NumberUtils.CompareByF<T>(left, right, symbol),
            () => NumberUtils.CompareByF<T>(left, right, op),
            () => NumberUtils.CompareByF(left, right, typeof(T), symbol),
            () => NumberUtils.CompareByF(left, right, typeof(T), op),
            () => NumberUtils.CompareByF(left, right, type, symbol),
            () => NumberUtils.CompareByF(left, right, type, op)
        );
    }

    [TestMethod]
    public void Compare_PublicSignatures_ReturnInt()
    {
        var methods = typeof(NumberUtils)
            .GetMethods()
            .Where(method => method.Name is "Compare" or "CompareF")
            .ToArray();
        Assert.HasCount(6, methods);
        foreach (var method in methods)
            Assert.AreEqual(typeof(int), method.ReturnType, method.ToString());
    }

    [TestMethod]
    [DataRow("==", ComparisonOperatorType.Equality)]
    [DataRow("!=", ComparisonOperatorType.Inequality)]
    [DataRow("<=", ComparisonOperatorType.LessThanOrEqual)]
    [DataRow(">=", ComparisonOperatorType.GreaterThanOrEqual)]
    [DataRow("<", ComparisonOperatorType.LessThan)]
    [DataRow(">", ComparisonOperatorType.GreaterThan)]
    public void PrefixParser_UsesLongestMatch(string symbol, ComparisonOperatorType expected)
    {
        Assert.AreEqual(expected, NumberUtils.GetComparisonOperatorType(symbol));
        foreach (var suffix in new[] { "", "10", "-10", " 10", "=invalid", ">10" })
        {
            var expression = symbol + suffix;
            var expectedSymbol =
                symbol is "<" or ">" && suffix.StartsWith('=') ? symbol + "=" : symbol;
            Assert.AreEqual(
                NumberUtils.GetComparisonOperatorType(expectedSymbol),
                NumberUtils.GetComparisonOperatorType(expression, out var length)
            );
            Assert.AreEqual(expectedSymbol.Length, length);
            Assert.AreEqual(expression[length..], expression[expectedSymbol.Length..]);
        }
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("!")]
    [DataRow("=")]
    [DataRow("  <=10")]
    [DataRow("10")]
    [DataRow("≤10")]
    [DataRow("invalid")]
    public void InvalidPrefix_HasZeroLength(string expression)
    {
        Assert.AreEqual(default, NumberUtils.GetComparisonOperatorType(expression, out var length));
        Assert.AreEqual(0, length);
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            NumberUtils.GetComparisonOperatorType(expression)
        );
    }

    public static IEnumerable<object[]> Types =>
        NumberTestData.Numbers.Select(type => new object[] { type });

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void ConversionAndInvalidInputs(Type type) =>
        NumberTestData.Run(typeof(NumberComparisonTests), nameof(CheckConversion), type);

    private static void CheckConversion<T>()
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var type = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => T.One.CompareTo(T.CreateChecked(2)),
            () => NumberUtils.Compare<T>("1", "2"),
            () => NumberUtils.Compare("1", "2", typeof(T)),
            () => NumberUtils.Compare("1", "2", type)
        );
        Assert.IsTrue(NumberUtils.CompareBy<T>("1", "2", "<"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareF<T>("1", "2"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareF("1", "2", typeof(T)));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareF("1", "2", type));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareByF<T>("1", "2", "<"));
        Assert.ThrowsExactly<FormatException>(() => NumberUtils.Compare<T>("invalid", "2"));
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.CompareBy<T>(T.One, T.One, (ComparisonOperatorType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.CompareByF<T>(T.One, T.One, (ComparisonOperatorType)999)
        );
    }

    [TestMethod]
    public void UnsupportedTypesAndOperators_AreRejected()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            NumberUtils.GetComparisonOperatorType(null!, out _)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.Compare(1, 2, (NumberType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.CompareF(1, 2, (NumberType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.Compare(1, 2, typeof(bool))
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.CompareF(1, 2, typeof(bool))
        );
        Assert.ThrowsExactly<OverflowException>(() => NumberUtils.Compare<byte>(256, 1));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareF<byte>(1, 2));
        Assert.ThrowsExactly<KeyNotFoundException>(() => NumberUtils.CompareBy<int>(1, 2, "=<"));
        Assert.ThrowsExactly<KeyNotFoundException>(() => NumberUtils.CompareByF<int>(1, 2, "=<"));
    }
}
