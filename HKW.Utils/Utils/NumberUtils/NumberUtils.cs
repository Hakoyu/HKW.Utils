using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 获取数值类型
    /// </summary>
    /// <param name="numberType">类型</param>
    /// <returns>数值类型</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static NumberType GetNumberType(Type numberType)
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
