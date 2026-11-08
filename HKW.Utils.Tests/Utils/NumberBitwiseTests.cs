using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberBitwiseTests
{
    public static IEnumerable<object[]> Matrix =>
        from type in NumberTestData.Integers
        from symbol in "|&^"
        from sample in NumberTestData.Samples(type)
        select new object[] { type, symbol, sample };

    [TestMethod]
    [DynamicData(nameof(Matrix))]
    public void Binary_AllIntegerTypesOperatorsAndEntryPoints(
        Type type,
        char symbol,
        string sample
    ) => NumberTestData.Run(typeof(NumberBitwiseTests), nameof(CheckBinary), type, symbol, sample);

    private static void CheckBinary<T>(char symbol, string sample)
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        var (left, right) = NumberTestData.Operands<T>(sample);
        var type = NumberUtils.GetNumberType(typeof(T));
        var op = NumberUtils.GetBitwiseOperatorType(symbol);
        Assert.IsTrue(NumberUtils.TryGetBitwiseOperatorType(symbol, out var parsed));
        Assert.AreEqual(op, parsed);
        NumberTestData.Check(
            () =>
                symbol switch
                {
                    '|' => left | right,
                    '&' => left & right,
                    '^' => left ^ right,
                    _ => throw new ArgumentException("Unknown operator."),
                },
            () => NumberUtils.BitwiseOperator<T>(left, right, symbol),
            () => NumberUtils.BitwiseOperator<T>(left, right, op),
            () => NumberUtils.BitwiseOperator(left, right, typeof(T), symbol),
            () => NumberUtils.BitwiseOperator(left, right, typeof(T), op),
            () => NumberUtils.BitwiseOperator(left, right, type, symbol),
            () => NumberUtils.BitwiseOperator(left, right, type, op),
            () => NumberUtils.BitwiseOperatorF<T>(left, right, symbol),
            () => NumberUtils.BitwiseOperatorF<T>(left, right, op),
            () => NumberUtils.BitwiseOperatorF(left, right, typeof(T), symbol),
            () => NumberUtils.BitwiseOperatorF(left, right, typeof(T), op),
            () => NumberUtils.BitwiseOperatorF(left, right, type, symbol),
            () => NumberUtils.BitwiseOperatorF(left, right, type, op)
        );
    }

    public static IEnumerable<object[]> ComplementMatrix =>
        from type in NumberTestData.Integers
        from sample in NumberTestData.Samples(type)
        select new object[] { type, sample };

    [TestMethod]
    [DynamicData(nameof(ComplementMatrix))]
    public void Complement_NormalizesToRequestedWidth(Type type, string sample) =>
        NumberTestData.Run(typeof(NumberBitwiseTests), nameof(CheckComplement), type, sample);

    private static void CheckComplement<T>(string sample)
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        var value = NumberTestData.Operands<T>(sample).Left;
        var type = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => ~value,
            () => NumberUtils.BitwiseComplement<T>(value),
            () => NumberUtils.BitwiseComplement(value, typeof(T)),
            () => NumberUtils.BitwiseComplement(value, type),
            () => NumberUtils.BitwiseComplementF<T>(value),
            () => NumberUtils.BitwiseComplementF(value, typeof(T)),
            () => NumberUtils.BitwiseComplementF(value, type)
        );
    }

    public static IEnumerable<object[]> ShiftMatrix =>
        from type in NumberTestData.Integers
        from symbol in new[] { "<<", ">>", ">>>" }
        from sample in new[] { "basic", "max", "min" }
        from shift in new[] { -129, -65, -33, -1, 0, 1, 7, 8, 15, 16, 31, 32, 33, 63, 64, 65, 127, 128, 129 }
        select new object[] { type, symbol, sample, shift };

    [TestMethod]
    [DynamicData(nameof(ShiftMatrix))]
    public void Shift_AllIntegerTypesAndMaskedCounts(
        Type type,
        string symbol,
        string sample,
        int shift
    ) =>
        NumberTestData.Run(
            typeof(NumberBitwiseTests),
            nameof(CheckShift),
            type,
            symbol,
            sample,
            shift
        );

    private static void CheckShift<T>(string symbol, string sample, int shift)
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        var value = NumberTestData.Operands<T>(sample).Left;
        var type = NumberUtils.GetNumberType(typeof(T));
        var op = NumberUtils.GetBitwiseShiftType(symbol);
        Assert.IsTrue(NumberUtils.TryGetBitwiseShiftType(symbol, out var parsed));
        Assert.AreEqual(op, parsed);
        // 小整数先提升到 32 位, 泛型数学的小整数移位掩码不同.
        var promotedWidth =
            typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128) ? 128
            : typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) ? IntPtr.Size * 8
            : typeof(T) == typeof(long) || typeof(T) == typeof(ulong) ? 64
            : 32;
        var count = shift & (promotedWidth - 1);
        var wide = BigInteger.CreateChecked(value);
        if (symbol == ">>>" && wide.Sign < 0)
            wide += BigInteger.One << promotedWidth;
        var expected = T.CreateTruncating(symbol == "<<" ? wide << count : wide >> count);
        NumberTestData.Check(
            () => expected,
            () => NumberUtils.BitwiseShift<T>(value, shift, symbol),
            () => NumberUtils.BitwiseShift<T>(value, shift, op),
            () => NumberUtils.BitwiseShift(value, shift, typeof(T), symbol),
            () => NumberUtils.BitwiseShift(value, shift, typeof(T), op),
            () => NumberUtils.BitwiseShift(value, shift, type, symbol),
            () => NumberUtils.BitwiseShift(value, shift, type, op),
            () => NumberUtils.BitwiseShiftF<T>(value, shift, symbol),
            () => NumberUtils.BitwiseShiftF<T>(value, shift, op),
            () => NumberUtils.BitwiseShiftF(value, shift, typeof(T), symbol),
            () => NumberUtils.BitwiseShiftF(value, shift, typeof(T), op),
            () => NumberUtils.BitwiseShiftF(value, shift, type, symbol),
            () => NumberUtils.BitwiseShiftF(value, shift, type, op)
        );
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(65536)]
    public void UInt16Shift_ValidatesInputRangeBeforeShifting(int value)
    {
        foreach (var op in Enum.GetValues<BitwiseShiftType>())
        {
            Assert.ThrowsExactly<OverflowException>(() =>
                NumberUtils.BitwiseShift<ushort>(value, 1, op)
            );
            Assert.ThrowsExactly<OverflowException>(() =>
                NumberUtils.BitwiseShift(value, 1, typeof(ushort), op)
            );
            Assert.ThrowsExactly<OverflowException>(() =>
                NumberUtils.BitwiseShift(value, 1, NumberType.UInt16, op)
            );
        }
    }

    public static IEnumerable<object[]> Types =>
        NumberTestData.Integers.Select(type => new object[] { type });

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void ConversionAndExactUnboxing_AreDifferentContracts(Type type) =>
        NumberTestData.Run(typeof(NumberBitwiseTests), nameof(CheckConversion), type);

    private static void CheckConversion<T>()
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        var type = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => T.CreateChecked(3),
            () => NumberUtils.BitwiseOperator<T>("1", "2", '|'),
            () => NumberUtils.BitwiseOperator("1", "2", typeof(T), '|'),
            () => NumberUtils.BitwiseOperator("1", "2", type, '|')
        );
        NumberTestData.Check(
            () => T.CreateChecked(2),
            () => NumberUtils.BitwiseShift<T>("1", "1", "<<"),
            () => NumberUtils.BitwiseShift("1", "1", typeof(T), "<<"),
            () => NumberUtils.BitwiseShift("1", "1", type, "<<")
        );
        NumberTestData.Check(
            () => ~T.One,
            () => NumberUtils.BitwiseComplement<T>("1"),
            () => NumberUtils.BitwiseComplement("1", typeof(T)),
            () => NumberUtils.BitwiseComplement("1", type)
        );
        Assert.ThrowsExactly<InvalidCastException>(() =>
            NumberUtils.BitwiseOperatorF<T>("1", "2", '|')
        );
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.BitwiseComplementF<T>("1"));
        Assert.ThrowsExactly<InvalidCastException>(() =>
            NumberUtils.BitwiseShiftF<T>("1", 1, "<<")
        );
        Assert.ThrowsExactly<InvalidCastException>(() =>
            NumberUtils.BitwiseShiftF<T>(T.One, (short)1, "<<")
        );
        Assert.ThrowsExactly<FormatException>(() =>
            NumberUtils.BitwiseShift<T>(T.One, "invalid", "<<")
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.BitwiseOperator<T>(T.One, T.One, (BitwiseOperatorType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.BitwiseOperatorF<T>(T.One, T.One, (BitwiseOperatorType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.BitwiseShift<T>(T.One, 1, (BitwiseShiftType)999)
        );
        Assert.ThrowsExactly<NotImplementedException>(() =>
            NumberUtils.BitwiseShiftF<T>(T.One, 1, (BitwiseShiftType)999)
        );
    }

    [TestMethod]
    public void UnsupportedTypesAndSymbols_AreRejected()
    {
        foreach (
            var type in new[]
            {
                NumberType.Single,
                NumberType.Double,
                NumberType.Decimal,
                NumberType.Half,
                (NumberType)999,
            }
        )
        {
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseOperator(1, 2, type, '|')
            );
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseOperatorF(1, 2, type, '|')
            );
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseShift(1, 2, type, "<<")
            );
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseShiftF(1, 2, type, "<<")
            );
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseComplement(1, type)
            );
            Assert.ThrowsExactly<NotImplementedException>(() =>
                NumberUtils.BitwiseComplementF(1, type)
            );
        }
        Assert.IsFalse(NumberUtils.TryGetBitwiseOperatorType('?', out _));
        Assert.ThrowsExactly<KeyNotFoundException>(() => NumberUtils.GetBitwiseOperatorType('?'));
        foreach (var symbol in new[] { "", ">", "<", ">>>>", " >>", "<<1" })
        {
            Assert.IsFalse(NumberUtils.TryGetBitwiseShiftType(symbol, out _));
            Assert.ThrowsExactly<KeyNotFoundException>(() =>
                NumberUtils.GetBitwiseShiftType(symbol)
            );
        }
        Assert.ThrowsExactly<OverflowException>(() =>
            NumberUtils.BitwiseShift<int>(1, long.MaxValue, "<<")
        );
    }
}
