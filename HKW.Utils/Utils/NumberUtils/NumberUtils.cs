using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils;

/// <summary>
/// 数值转换, 比较及运算工具, 仅支持既有的十一种数值类型; 位运算仅支持八种整数类型.
/// </summary>
/// <remarks>
/// 普通算术, 比较和位运算方法先使用 <see cref="Convert"/> 转换输入, 转换溢出仍会抛出异常.
/// 名称以 F 结尾的方法要求输入的装箱类型与目标类型完全一致, 不进行数值转换.
/// 比较结果统一返回 <see cref="int"/>, 仅保证其正负号, 不保证幅值为一.
/// 算术及位运算结果保持目标运行时类型, 小整数提升运算后以 unchecked 方式截断到目标位宽.
/// 整数运算采用 unchecked 规则, 但除零及有符号大整数最小值除以负一仍遵循类型本身的异常规则.
/// 浮点及 decimal 运算保留各自的溢出, 非数值及除零规则.
/// </remarks>
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
