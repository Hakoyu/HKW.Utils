using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberConversionTests
{
    public static IEnumerable<object[]> Types =>
        NumberTestData.Numbers.Append(typeof(BigInteger)).Select(type => new object[] { type });

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void NumericSources_MatchCreateChecked(Type target) =>
        NumberTestData.Run(typeof(NumberConversionTests), nameof(CheckTarget), target);

    private static void CheckTarget<T>()
        where T : struct, INumber<T>
    {
        CheckSource<T, sbyte>(sbyte.MinValue, 0, 1, sbyte.MaxValue);
        CheckSource<T, byte>(0, 1, byte.MaxValue);
        CheckSource<T, short>(short.MinValue, 0, 1, short.MaxValue);
        CheckSource<T, ushort>(0, 1, ushort.MaxValue);
        CheckSource<T, int>(int.MinValue, 0, 1, int.MaxValue);
        CheckSource<T, uint>(0, 1, uint.MaxValue);
        CheckSource<T, long>(long.MinValue, 0, 1, long.MaxValue);
        CheckSource<T, ulong>(0, 1, ulong.MaxValue);
        CheckSource<T, nint>(nint.MinValue, 0, 1, nint.MaxValue);
        CheckSource<T, nuint>(0, 1, nuint.MaxValue);
        CheckSource<T, Int128>(Int128.MinValue, 0, 1, Int128.MaxValue);
        CheckSource<T, UInt128>(0, 1, UInt128.MaxValue);
        CheckSource<T, Half>(Half.MinValue, (Half)(-1.9), Half.NegativeZero, Half.One,
            (Half)2.5, Half.MaxValue, Half.NaN, Half.PositiveInfinity, Half.NegativeInfinity);
        CheckSource<T, float>(float.MinValue, -1.9f, -0.0f, 1, 2.5f, 3.5f, float.MaxValue,
            float.NaN, float.PositiveInfinity, float.NegativeInfinity);
        CheckSource<T, double>(double.MinValue, -1.9, -0.0, 1, 2.5, 3.5, 255.9, 256,
            double.MaxValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity);
        CheckSource<T, decimal>(decimal.MinValue, -1.9m, 0, 1, 2.5m, 3.5m, 255.9m, decimal.MaxValue);
        CheckSource<T, BigInteger>(-(BigInteger.One << 256), -1, 0, 1,
            (BigInteger.One << 100) + 1, (BigInteger.One << 256) + 123);
    }

    private static void CheckSource<T, TSource>(params TSource[] values)
        where T : struct, INumber<T>
        where TSource : struct, INumber<TSource>
    {
        var numberType = NumberUtils.GetNumberType(typeof(T));
        foreach (var value in values)
        {
            NumberTestData.Check(
                () => T.CreateChecked(value),
                () => NumberUtils.ConvertTo<T>(value),
                () => NumberUtils.ConvertTo(value, typeof(T)),
                () => NumberUtils.ConvertTo(value, numberType)
            );
        }
    }

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void DefaultAndStringConversion_PreserveTargetType(Type target) =>
        NumberTestData.Run(typeof(NumberConversionTests), nameof(CheckDefaults), target);

    private static void CheckDefaults<T>()
        where T : struct, INumber<T>
    {
        var numberType = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(
            () => T.Zero,
            () => NumberUtils.GetDefault<T>(),
            () => NumberUtils.GetDefault(typeof(T)),
            () => NumberUtils.GetDefault(numberType),
            () => NumberUtils.ConvertTo<T>((object)null!),
            () => NumberUtils.ConvertTo((object)null!, typeof(T)),
            () => NumberUtils.ConvertTo((object)null!, numberType)
        );
        NumberTestData.Check(
            () => T.CreateChecked(12),
            () => NumberUtils.ConvertTo<T>((object)"12", CultureInfo.InvariantCulture),
            () => NumberUtils.ConvertTo((object)"12", typeof(T), CultureInfo.InvariantCulture),
            () => NumberUtils.ConvertTo((object)"12", numberType, CultureInfo.InvariantCulture)
        );
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.ConvertTo<T>(new object()));
    }

    public static IEnumerable<object[]> FiniteTypes =>
        NumberTestData.Numbers.Select(type => new object[] { type });

    [TestMethod]
    [DynamicData(nameof(FiniteTypes))]
    public void FiniteBounds_MatchNativeConstants(Type target) =>
        NumberTestData.Run(typeof(NumberConversionTests), nameof(CheckBounds), target);

    private static void CheckBounds<T>()
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var numberType = NumberUtils.GetNumberType(typeof(T));
        NumberTestData.Check(() => T.MinValue,
            () => NumberUtils.GetMinValue<T>(),
            () => NumberUtils.GetMinValue(typeof(T)),
            () => NumberUtils.GetMinValue(numberType));
        NumberTestData.Check(() => T.MaxValue,
            () => NumberUtils.GetMaxValue<T>(),
            () => NumberUtils.GetMaxValue(typeof(T)),
            () => NumberUtils.GetMaxValue(numberType));
    }

    [TestMethod]
    public void TypeIdentityAndUnsupportedBounds_AreStable()
    {
        Type[] types = [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
            typeof(nint), typeof(nuint), typeof(Int128), typeof(UInt128), typeof(Half), typeof(BigInteger)];
        Assert.HasCount(types.Length, Enum.GetValues<NumberType>());
        for (var i = 0; i < types.Length; i++)
            Assert.AreEqual((NumberType)i, NumberUtils.GetNumberType(types[i]));
        Assert.AreEqual(NumberType.IntPtr, NumberUtils.GetNumberType(typeof(IntPtr)));
        Assert.AreEqual(NumberType.UIntPtr, NumberUtils.GetNumberType(typeof(UIntPtr)));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMinValue<BigInteger>());
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMaxValue<BigInteger>());
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMinValue(typeof(BigInteger)));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMaxValue(typeof(BigInteger)));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMinValue(NumberType.BigInteger));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMaxValue(NumberType.BigInteger));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetNumberType(typeof(bool)));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.ConvertTo(1, (NumberType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetDefault((NumberType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMinValue((NumberType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.GetMaxValue((NumberType)999));
        Assert.ThrowsExactly<ArgumentNullException>(() => NumberUtils.GetNumberType(null!));
    }

    [TestMethod]
    public void LegacyNonNumericInputs_KeepConvertBehavior()
    {
        Type[] legacy = [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)];
        foreach (var type in legacy)
        {
            foreach (var value in new object[] { true, false, 'A', "12", "invalid", DateTime.UnixEpoch })
            {
                NumberTestData.Check(
                    () => Convert.ChangeType(value, type, CultureInfo.InvariantCulture),
                    () => NumberUtils.ConvertTo(value, type, CultureInfo.InvariantCulture));
            }
        }
        var provider = CultureInfo.GetCultureInfo("fr-FR");
        var custom = new ConversionProbe();
        Assert.AreEqual(42, NumberUtils.ConvertTo<int>(custom, provider));
        Assert.AreSame(provider, custom.Provider);
        Assert.AreEqual(43, NumberUtils.Arithmetic<int>(custom, 1, '+'));
        Assert.IsNull(custom.Provider);
        Assert.ThrowsExactly<InvalidOperationException>(() => NumberUtils.ConvertTo<long>(custom));
    }

    [TestMethod]
    public void NumericConversionRules_AreSharedByNormalOperations()
    {
        Assert.AreEqual(4, NumberUtils.Arithmetic<int>(1.9, 3.9m, '+'));
        Assert.AreEqual(0, NumberUtils.Compare<int>(1.9, 1));
        Assert.IsTrue(NumberUtils.CompareBy<int>(1.9, 1, "=="));
        Assert.AreEqual(1, NumberUtils.BitwiseOperator<int>(1.9, 0, '|'));
        Assert.AreEqual(-2, NumberUtils.BitwiseComplement<int>(1.9));
        Assert.AreEqual(2, NumberUtils.BitwiseShift<int>(1.9, 1.9, "<<"));
        Assert.AreEqual(2, NumberUtils.BitwiseShift<int>(1, (BigInteger)1, "<<"));
        Assert.ThrowsExactly<OverflowException>(() => NumberUtils.BitwiseShift<int>(1, BigInteger.One << 40, "<<"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.BitwiseShiftF<int>(1, (nint)1, "<<"));
        Assert.ThrowsExactly<InvalidCastException>(() => NumberUtils.ArithmeticF<int>(1.9, 1, '+'));
        Assert.AreEqual(1.5m, NumberUtils.ConvertTo<decimal>((object)"1,5", CultureInfo.GetCultureInfo("fr-FR")));
    }

    [TestMethod]
    public void HalfNegativeZero_PreservesItsSign()
    {
        var zero = Half.NegativeZero;
        var bits = BitConverter.HalfToInt16Bits(zero);
        Assert.AreEqual(bits, BitConverter.HalfToInt16Bits(NumberUtils.ConvertTo<Half>((double)(-0.0))));
        Assert.AreEqual(bits, BitConverter.HalfToInt16Bits(NumberUtils.Arithmetic<Half>(zero, Half.One, '*')));
        Assert.AreEqual(bits, BitConverter.HalfToInt16Bits(NumberUtils.ArithmeticF<Half>(zero, Half.One, '*')));
        Assert.AreEqual(0, NumberUtils.Compare<Half>(zero, Half.Zero));
        Assert.IsTrue(NumberUtils.CompareBy<Half>(zero, Half.Zero, "=="));
    }

    private sealed class ConversionProbe : IConvertible
    {
        public IFormatProvider? Provider { get; private set; }
        public int ToInt32(IFormatProvider? provider) { Provider = provider; return 42; }
        public TypeCode GetTypeCode() => TypeCode.Object;
        public bool ToBoolean(IFormatProvider? provider) => throw new InvalidOperationException();
        public byte ToByte(IFormatProvider? provider) => throw new InvalidOperationException();
        public char ToChar(IFormatProvider? provider) => throw new InvalidOperationException();
        public DateTime ToDateTime(IFormatProvider? provider) => throw new InvalidOperationException();
        public decimal ToDecimal(IFormatProvider? provider) => throw new InvalidOperationException();
        public double ToDouble(IFormatProvider? provider) => throw new InvalidOperationException();
        public short ToInt16(IFormatProvider? provider) => throw new InvalidOperationException();
        public long ToInt64(IFormatProvider? provider) => throw new InvalidOperationException();
        public sbyte ToSByte(IFormatProvider? provider) => throw new InvalidOperationException();
        public float ToSingle(IFormatProvider? provider) => throw new InvalidOperationException();
        public string ToString(IFormatProvider? provider) => throw new InvalidOperationException();
        public object ToType(Type conversionType, IFormatProvider? provider) => throw new InvalidOperationException();
        public ushort ToUInt16(IFormatProvider? provider) => throw new InvalidOperationException();
        public uint ToUInt32(IFormatProvider? provider) => throw new InvalidOperationException();
        public ulong ToUInt64(IFormatProvider? provider) => throw new InvalidOperationException();
    }
}
