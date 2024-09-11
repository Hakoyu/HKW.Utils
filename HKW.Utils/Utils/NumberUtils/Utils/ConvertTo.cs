using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 数值类型工具
/// </summary>
public static partial class NumberUtils
{
    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value">值</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo<T>(object? value, IFormatProvider? provider = null)
        where T : struct, INumber<T>
    {
        var type = typeof(T);
        if (value is IConvertible ic)
        {
            if (type == typeof(sbyte))
                return ic.ToSByte(provider);
            else if (type == typeof(byte))
                return ic.ToByte(provider);
            else if (type == typeof(short))
                return ic.ToInt16(provider);
            else if (type == typeof(ushort))
                return ic.ToUInt16(provider);
            else if (type == typeof(int))
                return ic.ToInt32(provider);
            else if (type == typeof(uint))
                return ic.ToUInt32(provider);
            else if (type == typeof(long))
                return ic.ToInt64(provider);
            else if (type == typeof(ulong))
                return ic.ToUInt64(provider);
            else if (type == typeof(float))
                return ic.ToSingle(provider);
            else if (type == typeof(double))
                return ic.ToDouble(provider);
            else if (type == typeof(decimal))
                return ic.ToDecimal(provider);
        }
        if (type == typeof(sbyte))
            return Convert.ToSByte(value);
        else if (type == typeof(byte))
            return Convert.ToByte(value);
        else if (type == typeof(short))
            return Convert.ToInt16(value);
        else if (type == typeof(ushort))
            return Convert.ToUInt16(value);
        else if (type == typeof(int))
            return Convert.ToInt32(value);
        else if (type == typeof(uint))
            return Convert.ToUInt32(value);
        else if (type == typeof(long))
            return Convert.ToInt64(value);
        else if (type == typeof(ulong))
            return Convert.ToUInt64(value);
        else if (type == typeof(float))
            return Convert.ToSingle(value);
        else if (type == typeof(double))
            return Convert.ToDouble(value);
        else if (type == typeof(decimal))
            return Convert.ToDecimal(value);
        else
            return Convert.ChangeType(value, type)!;
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(object? value, Type numberType, IFormatProvider? provider = null)
    {
        if (value is IConvertible ic)
        {
            if (numberType == typeof(sbyte))
                return ic.ToSByte(provider);
            else if (numberType == typeof(byte))
                return ic.ToByte(provider);
            else if (numberType == typeof(short))
                return ic.ToInt16(provider);
            else if (numberType == typeof(ushort))
                return ic.ToUInt16(provider);
            else if (numberType == typeof(int))
                return ic.ToInt32(provider);
            else if (numberType == typeof(uint))
                return ic.ToUInt32(provider);
            else if (numberType == typeof(long))
                return ic.ToInt64(provider);
            else if (numberType == typeof(ulong))
                return ic.ToUInt64(provider);
            else if (numberType == typeof(float))
                return ic.ToSingle(provider);
            else if (numberType == typeof(double))
                return ic.ToDouble(provider);
            else if (numberType == typeof(decimal))
                return ic.ToDecimal(provider);
        }
        if (numberType == typeof(sbyte))
            return Convert.ToSByte(value);
        else if (numberType == typeof(byte))
            return Convert.ToByte(value);
        else if (numberType == typeof(short))
            return Convert.ToInt16(value);
        else if (numberType == typeof(ushort))
            return Convert.ToUInt16(value);
        else if (numberType == typeof(int))
            return Convert.ToInt32(value);
        else if (numberType == typeof(uint))
            return Convert.ToUInt32(value);
        else if (numberType == typeof(long))
            return Convert.ToInt64(value);
        else if (numberType == typeof(ulong))
            return Convert.ToUInt64(value);
        else if (numberType == typeof(float))
            return Convert.ToSingle(value);
        else if (numberType == typeof(double))
            return Convert.ToDouble(value);
        else if (numberType == typeof(decimal))
            return Convert.ToDecimal(value);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="type">目标类型</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(object? value, NumberType type, IFormatProvider? provider = null)
    {
        if (value is IConvertible ic)
        {
            if (type is NumberType.SByte)
                return ic.ToSByte(provider);
            else if (type is NumberType.Byte)
                return ic.ToByte(provider);
            else if (type is NumberType.Int16)
                return ic.ToInt16(provider);
            else if (type is NumberType.UInt16)
                return ic.ToUInt16(provider);
            else if (type is NumberType.Int32)
                return ic.ToInt32(provider);
            else if (type is NumberType.UInt32)
                return ic.ToUInt32(provider);
            else if (type is NumberType.Int64)
                return ic.ToInt64(provider);
            else if (type is NumberType.UInt64)
                return ic.ToUInt64(provider);
            else if (type is NumberType.Single)
                return ic.ToSingle(provider);
            else if (type is NumberType.Double)
                return ic.ToDouble(provider);
            else if (type is NumberType.Decimal)
                return ic.ToDecimal(provider);
        }
        if (type is NumberType.SByte)
            return Convert.ToSByte(provider);
        else if (type is NumberType.Byte)
            return Convert.ToByte(provider);
        else if (type is NumberType.Int16)
            return Convert.ToInt16(provider);
        else if (type is NumberType.UInt16)
            return Convert.ToUInt16(provider);
        else if (type is NumberType.Int32)
            return Convert.ToInt32(provider);
        else if (type is NumberType.UInt32)
            return Convert.ToUInt32(provider);
        else if (type is NumberType.Int64)
            return Convert.ToInt64(provider);
        else if (type is NumberType.UInt64)
            return Convert.ToUInt64(provider);
        else if (type is NumberType.Single)
            return Convert.ToSingle(provider);
        else if (type is NumberType.Double)
            return Convert.ToDouble(provider);
        else if (type is NumberType.Decimal)
            return Convert.ToDecimal(provider);
        else
            throw new NotImplementedException();
    }
}
