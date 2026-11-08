using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    private static T ConvertNumber<T>(object? value, IFormatProvider? provider = null)
        where T : struct, INumber<T>
    {
        return value switch
        {
            sbyte number => T.CreateChecked(number),
            byte number => T.CreateChecked(number),
            short number => T.CreateChecked(number),
            ushort number => T.CreateChecked(number),
            int number => T.CreateChecked(number),
            uint number => T.CreateChecked(number),
            long number => T.CreateChecked(number),
            ulong number => T.CreateChecked(number),
            float number => T.CreateChecked(number),
            double number => T.CreateChecked(number),
            decimal number => T.CreateChecked(number),
            nint number => T.CreateChecked(number),
            nuint number => T.CreateChecked(number),
            Int128 number => T.CreateChecked(number),
            UInt128 number => T.CreateChecked(number),
            Half number => T.CreateChecked(number),
            BigInteger number => T.CreateChecked(number),
            _ => ConvertNonNumber<T>(value, provider),
        };
    }

    private static T ConvertNonNumber<T>(object? value, IFormatProvider? provider)
        where T : struct, INumber<T>
    {
        // 保留旧目标的 IConvertible 方法选择, 包括 char 和自定义实现的异常行为.
        if (typeof(T) == typeof(sbyte))
            return (T)(object)Convert.ToSByte(value, provider);
        if (typeof(T) == typeof(byte))
            return (T)(object)Convert.ToByte(value, provider);
        if (typeof(T) == typeof(short))
            return (T)(object)Convert.ToInt16(value, provider);
        if (typeof(T) == typeof(ushort))
            return (T)(object)Convert.ToUInt16(value, provider);
        if (typeof(T) == typeof(int))
            return (T)(object)Convert.ToInt32(value, provider);
        if (typeof(T) == typeof(uint))
            return (T)(object)Convert.ToUInt32(value, provider);
        if (typeof(T) == typeof(long))
            return (T)(object)Convert.ToInt64(value, provider);
        if (typeof(T) == typeof(ulong))
            return (T)(object)Convert.ToUInt64(value, provider);
        if (typeof(T) == typeof(float))
            return (T)(object)Convert.ToSingle(value, provider);
        if (typeof(T) == typeof(double))
            return (T)(object)Convert.ToDouble(value, provider);
        if (typeof(T) == typeof(decimal))
            return (T)(object)Convert.ToDecimal(value, provider);

        return value switch
        {
            null => T.Zero,
            string text => T.Parse(text, provider),
            bool boolean => boolean ? T.One : T.Zero,
            char character => T.CreateChecked((ushort)character),
            IConvertible convertible => (T)convertible.ToType(typeof(T), provider),
            _ => throw new InvalidCastException(
                $"Cannot convert {value.GetType()} to {typeof(T)}."
            ),
        };
    }
}
