using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberBigIntegerTests
{
    public static IEnumerable<object[]> Operands
    {
        get
        {
            BigInteger[] values = [0, 1, -1, (BigInteger.One << 256) + 17, -(BigInteger.One << 192) - 13];
            foreach (var left in values)
                foreach (var right in values)
                    yield return [left, right];
        }
    }

    [TestMethod]
    [DynamicData(nameof(Operands))]
    public void ArithmeticAndComparison_AllEntryPoints(BigInteger left, BigInteger right)
    {
        foreach (var symbol in "+-*/%")
        {
            var op = NumberUtils.GetArithmeticOperatorType(symbol);
            NumberTestData.Check(() => symbol switch
                {
                    '+' => left + right,
                    '-' => left - right,
                    '*' => left * right,
                    '/' => left / right,
                    '%' => left % right,
                    _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
                },
                () => NumberUtils.Arithmetic<BigInteger>(left, right, symbol),
                () => NumberUtils.Arithmetic<BigInteger>(left, right, op),
                () => NumberUtils.Arithmetic(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.Arithmetic(left, right, typeof(BigInteger), op),
                () => NumberUtils.Arithmetic(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.Arithmetic(left, right, NumberType.BigInteger, op),
                () => NumberUtils.ArithmeticF<BigInteger>(left, right, symbol),
                () => NumberUtils.ArithmeticF<BigInteger>(left, right, op),
                () => NumberUtils.ArithmeticF(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.ArithmeticF(left, right, typeof(BigInteger), op),
                () => NumberUtils.ArithmeticF(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.ArithmeticF(left, right, NumberType.BigInteger, op));
        }
        NumberTestData.Check(() => left.CompareTo(right),
            () => NumberUtils.Compare<BigInteger>(left, right),
            () => NumberUtils.Compare(left, right, typeof(BigInteger)),
            () => NumberUtils.Compare(left, right, NumberType.BigInteger),
            () => NumberUtils.CompareF<BigInteger>(left, right),
            () => NumberUtils.CompareF(left, right, typeof(BigInteger)),
            () => NumberUtils.CompareF(left, right, NumberType.BigInteger));
        foreach (var symbol in new[] { "==", "!=", "<", ">", "<=", ">=" })
        {
            var op = NumberUtils.GetComparisonOperatorType(symbol);
            NumberTestData.Check(() => symbol switch
                {
                    "==" => left == right,
                    "!=" => left != right,
                    "<" => left < right,
                    ">" => left > right,
                    "<=" => left <= right,
                    ">=" => left >= right,
                    _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
                },
                () => NumberUtils.CompareBy<BigInteger>(left, right, symbol),
                () => NumberUtils.CompareBy<BigInteger>(left, right, op),
                () => NumberUtils.CompareBy(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.CompareBy(left, right, typeof(BigInteger), op),
                () => NumberUtils.CompareBy(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.CompareBy(left, right, NumberType.BigInteger, op),
                () => NumberUtils.CompareByF<BigInteger>(left, right, symbol),
                () => NumberUtils.CompareByF<BigInteger>(left, right, op),
                () => NumberUtils.CompareByF(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.CompareByF(left, right, typeof(BigInteger), op),
                () => NumberUtils.CompareByF(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.CompareByF(left, right, NumberType.BigInteger, op));
        }
    }

    [TestMethod]
    [DynamicData(nameof(Operands))]
    public void Bitwise_AllEntryPoints(BigInteger left, BigInteger right)
    {
        foreach (var symbol in "|&^")
        {
            var op = NumberUtils.GetBitwiseOperatorType(symbol);
            NumberTestData.Check(() => symbol switch
                {
                    '|' => left | right,
                    '&' => left & right,
                    '^' => left ^ right,
                    _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
                },
                () => NumberUtils.BitwiseOperator<BigInteger>(left, right, symbol),
                () => NumberUtils.BitwiseOperator<BigInteger>(left, right, op),
                () => NumberUtils.BitwiseOperator(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.BitwiseOperator(left, right, typeof(BigInteger), op),
                () => NumberUtils.BitwiseOperator(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.BitwiseOperator(left, right, NumberType.BigInteger, op),
                () => NumberUtils.BitwiseOperatorF<BigInteger>(left, right, symbol),
                () => NumberUtils.BitwiseOperatorF<BigInteger>(left, right, op),
                () => NumberUtils.BitwiseOperatorF(left, right, typeof(BigInteger), symbol),
                () => NumberUtils.BitwiseOperatorF(left, right, typeof(BigInteger), op),
                () => NumberUtils.BitwiseOperatorF(left, right, NumberType.BigInteger, symbol),
                () => NumberUtils.BitwiseOperatorF(left, right, NumberType.BigInteger, op));
        }
        NumberTestData.Check(() => ~left,
            () => NumberUtils.BitwiseComplement<BigInteger>(left),
            () => NumberUtils.BitwiseComplement(left, typeof(BigInteger)),
            () => NumberUtils.BitwiseComplement(left, NumberType.BigInteger),
            () => NumberUtils.BitwiseComplementF<BigInteger>(left),
            () => NumberUtils.BitwiseComplementF(left, typeof(BigInteger)),
            () => NumberUtils.BitwiseComplementF(left, NumberType.BigInteger));
    }

    public static IEnumerable<object[]> Shifts =>
        from value in new BigInteger[] { 0, 1, -1, (BigInteger.One << 256) + 17, -(BigInteger.One << 192) - 13 }
        from count in new[] { -257, -129, -65, -1, 0, 1, 31, 32, 63, 64, 127, 128, 129, 257 }
        from symbol in new[] { "<<", ">>", ">>>" }
        select new object[] { value, count, symbol };

    [TestMethod]
    [DynamicData(nameof(Shifts))]
    public void Shifts_UseNativeArbitraryPrecisionSemantics(BigInteger value, int count, string symbol)
    {
        var op = NumberUtils.GetBitwiseShiftType(symbol);
        NumberTestData.Check(() => symbol switch
            {
                "<<" => value << count,
                ">>" => value >> count,
                ">>>" => value >>> count,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
            },
            () => NumberUtils.BitwiseShift<BigInteger>(value, count, symbol),
            () => NumberUtils.BitwiseShift<BigInteger>(value, count, op),
            () => NumberUtils.BitwiseShift(value, count, typeof(BigInteger), symbol),
            () => NumberUtils.BitwiseShift(value, count, typeof(BigInteger), op),
            () => NumberUtils.BitwiseShift(value, count, NumberType.BigInteger, symbol),
            () => NumberUtils.BitwiseShift(value, count, NumberType.BigInteger, op),
            () => NumberUtils.BitwiseShiftF<BigInteger>(value, count, symbol),
            () => NumberUtils.BitwiseShiftF<BigInteger>(value, count, op),
            () => NumberUtils.BitwiseShiftF(value, count, typeof(BigInteger), symbol),
            () => NumberUtils.BitwiseShiftF(value, count, typeof(BigInteger), op),
            () => NumberUtils.BitwiseShiftF(value, count, NumberType.BigInteger, symbol),
            () => NumberUtils.BitwiseShiftF(value, count, NumberType.BigInteger, op));
    }

    [TestMethod]
    public void MixedInputsAndFUnboxing_HaveDistinctContracts()
    {
        Assert.AreEqual(new BigInteger(4), NumberUtils.Arithmetic<BigInteger>(1.9, "3", '+'));
        Assert.AreEqual(0, NumberUtils.Compare<BigInteger>(1.9, BigInteger.One));
        Assert.IsTrue(NumberUtils.CompareBy<BigInteger>(1.9, "1", "=="));
        Assert.AreEqual(new BigInteger(3), NumberUtils.BitwiseOperator<BigInteger>(1.9, "2", '|'));
        Assert.AreEqual(new BigInteger(-2), NumberUtils.BitwiseComplement<BigInteger>(1.9));
        Assert.AreEqual(new BigInteger(2), NumberUtils.BitwiseShift<BigInteger>(1.9, 1.9, "<<"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.ArithmeticF<BigInteger>(1, 2, '+'));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareF<BigInteger>(1, 2));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.CompareByF<BigInteger>(1, 2, "<"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.BitwiseOperatorF<BigInteger>(1, 2, '|'));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.BitwiseComplementF<BigInteger>(1));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.BitwiseShiftF<BigInteger>(BigInteger.One, BigInteger.One, "<<"));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.Arithmetic<BigInteger>(1, 2, (ArithmeticOperatorType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.CompareBy<BigInteger>(1, 2, (ComparisonOperatorType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.BitwiseOperator<BigInteger>(1, 2, (BitwiseOperatorType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.BitwiseShift<BigInteger>(1, 2, (BitwiseShiftType)999));
    }
}
