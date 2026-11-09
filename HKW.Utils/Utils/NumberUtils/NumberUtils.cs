using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils;

/// <summary>
/// 提供数值转换, 比较及运算工具.
/// </summary>
public static partial class NumberUtils
{
    /// <summary>
    /// 获取运行时类型对应的数值类型枚举值.
    /// </summary>
    /// <param name="numberType">本工具支持的数值运行时类型.</param>
    /// <returns>对应的数值类型枚举值.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
#pragma warning disable S3776
    public static NumberType GetNumberType(Type numberType)
#pragma warning restore S3776
    {
        ArgumentNullException.ThrowIfNull(numberType);

        if (numberType == typeof(sbyte))
            return NumberType.SByte;
        else if (numberType == typeof(byte))
            return NumberType.Byte;
        else if (numberType == typeof(short))
            return NumberType.Int16;
        else if (numberType == typeof(ushort))
            return NumberType.UInt16;
        else if (numberType == typeof(int))
            return NumberType.Int32;
        else if (numberType == typeof(uint))
            return NumberType.UInt32;
        else if (numberType == typeof(long))
            return NumberType.Int64;
        else if (numberType == typeof(ulong))
            return NumberType.UInt64;
        else if (numberType == typeof(float))
            return NumberType.Single;
        else if (numberType == typeof(double))
            return NumberType.Double;
        else if (numberType == typeof(decimal))
            return NumberType.Decimal;
        else if (numberType == typeof(nint))
            return NumberType.IntPtr;
        else if (numberType == typeof(nuint))
            return NumberType.UIntPtr;
        else if (numberType == typeof(Int128))
            return NumberType.Int128;
        else if (numberType == typeof(UInt128))
            return NumberType.UInt128;
        else if (numberType == typeof(Half))
            return NumberType.Half;
        else if (numberType == typeof(System.Numerics.BigInteger))
            return NumberType.BigInteger;
        else
            throw new NotImplementedException($"Unsupported number type: {numberType.FullName}.");
    }

    private static NotImplementedException CreateUnsupportedNumberTypeException(
        NumberType numberType
    )
    {
        return new NotImplementedException($"Unsupported number type: {numberType}.");
    }

    private static NotImplementedException CreateUnsupportedOperatorException(Enum operatorType)
    {
        return new NotImplementedException($"Unsupported operator or type: {operatorType}.");
    }
}
