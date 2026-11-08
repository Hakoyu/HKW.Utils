using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberArithmeticTests
{
    public static IEnumerable<object[]> Matrix =>
        from type in NumberTestData.Numbers
        from symbol in "+-*/%"
        from sample in NumberTestData.Samples(type)
        select new object[] { type, symbol, sample };

    [TestMethod]
    [DynamicData(nameof(Matrix))]
    public void Arithmetic_AllTypesOperatorsAndEntryPoints(Type type, char symbol, string sample) =>
        NumberTestData.Run(
            typeof(NumberArithmeticTests),
            nameof(CheckArithmetic),
            type,
            symbol,
            sample
        );

    private static void CheckArithmetic<T>(char symbol, string sample)
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var (left, right) = NumberTestData.Operands<T>(sample);
        var type = NumberUtils.GetNumberType(typeof(T));
        var op = NumberUtils.GetArithmeticOperatorType(symbol);
        Assert.IsTrue(NumberUtils.TryGetArithmeticOperatorType(symbol, out var parsed));
        Assert.AreEqual(op, parsed);
        NumberTestData.Check(
            () =>
                symbol switch
                {
                    '+' => unchecked(left + right),
                    '-' => unchecked(left - right),
                    '*' => unchecked(left * right),
                    '/' => left / right,
                    '%' => left % right,
                    _ => throw new ArgumentException("Unknown operator."),
                },
            () => NumberUtils.Arithmetic<T>(left, right, symbol),
            () => NumberUtils.Arithmetic<T>(left, right, op),
            () => NumberUtils.Arithmetic(left, right, typeof(T), symbol),
            () => NumberUtils.Arithmetic(left, right, typeof(T), op),
            () => NumberUtils.Arithmetic(left, right, type, symbol),
            () => NumberUtils.Arithmetic(left, right, type, op),
            () => NumberUtils.ArithmeticF<T>(left, right, symbol),
            () => NumberUtils.ArithmeticF<T>(left, right, op),
            () => NumberUtils.ArithmeticF(left, right, typeof(T), symbol),
            () => NumberUtils.ArithmeticF(left, right, typeof(T), op),
            () => NumberUtils.ArithmeticF(left, right, type, symbol),
            () => NumberUtils.ArithmeticF(left, right, type, op)
        );
    }

    public static IEnumerable<object[]> Types =>
        NumberTestData.Numbers.Select(type => new object[] { type });

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void ConversionAndExactUnboxing_AreDifferentContracts(Type type) =>
        NumberTestData.Run(typeof(NumberArithmeticTests), nameof(CheckConversion), type);

    private static void CheckConversion<T>()
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var type = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => T.CreateChecked(5),
            () => NumberUtils.Arithmetic<T>("2", "3", '+'),
            () => NumberUtils.Arithmetic("2", "3", typeof(T), '+'),
            () => NumberUtils.Arithmetic("2", "3", type, '+')
        );
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.ArithmeticF<T>("2", "3", '+'));
        Assert.ThrowsExactly<InvalidCastException>(() =>
            NumberUtils.ArithmeticF("2", "3", typeof(T), '+')
        );
        Assert.ThrowsExactly<InvalidCastException>(() =>
            NumberUtils.ArithmeticF("2", "3", type, '+')
        );
        Assert.ThrowsExactly<FormatException>(() => NumberUtils.Arithmetic<T>("invalid", "3", '+'));
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.Arithmetic<T>(T.One, T.One, (ArithmeticOperatorType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.ArithmeticF<T>(T.One, T.One, (ArithmeticOperatorType)999)
        );
    }

    [TestMethod]
    public void InvalidOperatorsTypesAndConversionOverflow_AreRejected()
    {
        Assert.IsFalse(NumberUtils.TryGetArithmeticOperatorType('?', out _));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            NumberUtils.GetArithmeticOperatorType('?')
        );
        Assert.ThrowsExactly<KeyNotFoundException>(() => NumberUtils.Arithmetic<int>(1, 2, '?'));
        Assert.ThrowsExactly<KeyNotFoundException>(() => NumberUtils.ArithmeticF<int>(1, 2, '?'));
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.Arithmetic(1, 2, (NumberType)999, '+')
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.ArithmeticF(1, 2, (NumberType)999, '+')
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.Arithmetic<Half>((Half)1, (Half)2, '+')
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.ArithmeticF<Half>((Half)1, (Half)2, '+')
        );
        Assert.ThrowsExactly<OverflowException>(() => NumberUtils.Arithmetic<byte>(256, 1, '+'));
        Assert.ThrowsExactly<OverflowException>(() => NumberUtils.Arithmetic<uint>(-1, 1, '+'));
        Assert.ThrowsExactly<OverflowException>(() =>
            NumberUtils.Arithmetic<int>(double.NaN, 1, '+')
        );
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.ArithmeticF<byte>(1, 2, '+'));
    }
}
