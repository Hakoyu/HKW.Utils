using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 数值类型工具
/// </summary>
public static partial class NumberUtils
{
    #region object
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
    #endregion

    #region ReadOnlySpan
    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="str">字符串</param>
    /// <param name="style">样式</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo<T>(
        ReadOnlySpan<char> str,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
        where T : struct, INumber<T>
    {
        if (style is null)
            return T.Parse(str, provider);
        else
            return T.Parse(str, style.Value, provider);
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="style">样式</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(
        ReadOnlySpan<char> str,
        Type numberType,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
    {
        if (style is null)
        {
            if (numberType == typeof(sbyte))
                return sbyte.Parse(str, provider);
            else if (numberType == typeof(byte))
                return byte.Parse(str, provider);
            else if (numberType == typeof(short))
                return short.Parse(str, provider);
            else if (numberType == typeof(ushort))
                return ushort.Parse(str, provider);
            else if (numberType == typeof(int))
                return int.Parse(str, provider);
            else if (numberType == typeof(uint))
                return uint.Parse(str, provider);
            else if (numberType == typeof(long))
                return long.Parse(str, provider);
            else if (numberType == typeof(ulong))
                return ulong.Parse(str, provider);
            else if (numberType == typeof(float))
                return float.Parse(str, provider);
            else if (numberType == typeof(double))
                return double.Parse(str, provider);
            else if (numberType == typeof(decimal))
                return decimal.Parse(str, provider);
            else
                throw new NotImplementedException();
        }
        else
        {
            if (numberType == typeof(sbyte))
                return sbyte.Parse(str, style.Value, provider);
            else if (numberType == typeof(byte))
                return byte.Parse(str, style.Value, provider);
            else if (numberType == typeof(short))
                return short.Parse(str, style.Value, provider);
            else if (numberType == typeof(ushort))
                return ushort.Parse(str, style.Value, provider);
            else if (numberType == typeof(int))
                return int.Parse(str, style.Value, provider);
            else if (numberType == typeof(uint))
                return uint.Parse(str, style.Value, provider);
            else if (numberType == typeof(long))
                return long.Parse(str, style.Value, provider);
            else if (numberType == typeof(ulong))
                return ulong.Parse(str, style.Value, provider);
            else if (numberType == typeof(float))
                return float.Parse(str, style.Value, provider);
            else if (numberType == typeof(double))
                return double.Parse(str, style.Value, provider);
            else if (numberType == typeof(decimal))
                return decimal.Parse(str, style.Value, provider);
            else
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="numberType">目标类型</param>
    /// <param name="style">样式</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(
        ReadOnlySpan<char> str,
        NumberType numberType,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
    {
        if (style is null)
        {
            if (numberType is NumberType.SByte)
                return sbyte.Parse(str, provider);
            else if (numberType is NumberType.Byte)
                return byte.Parse(str, provider);
            else if (numberType is NumberType.Int16)
                return short.Parse(str, provider);
            else if (numberType is NumberType.UInt16)
                return ushort.Parse(str, provider);
            else if (numberType is NumberType.Int32)
                return int.Parse(str, provider);
            else if (numberType is NumberType.UInt32)
                return uint.Parse(str, provider);
            else if (numberType is NumberType.Int64)
                return long.Parse(str, provider);
            else if (numberType is NumberType.UInt64)
                return ulong.Parse(str, provider);
            else if (numberType is NumberType.Single)
                return float.Parse(str, provider);
            else if (numberType is NumberType.Double)
                return double.Parse(str, provider);
            else if (numberType is NumberType.Decimal)
                return decimal.Parse(str, provider);
            else
                throw new NotImplementedException();
        }
        else
        {
            if (numberType is NumberType.SByte)
                return sbyte.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Byte)
                return byte.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Int16)
                return short.Parse(str, style.Value, provider);
            else if (numberType is NumberType.UInt16)
                return ushort.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Int32)
                return int.Parse(str, style.Value, provider);
            else if (numberType is NumberType.UInt32)
                return uint.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Int64)
                return long.Parse(str, style.Value, provider);
            else if (numberType is NumberType.UInt64)
                return ulong.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Single)
                return float.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Double)
                return double.Parse(str, style.Value, provider);
            else if (numberType is NumberType.Decimal)
                return decimal.Parse(str, style.Value, provider);
            else
                throw new NotImplementedException();
        }
    }
    #endregion
}
