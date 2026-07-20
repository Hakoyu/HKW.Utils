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
    public static object ConvertTo<T>(object value, IFormatProvider? provider = null)
        where T : struct, INumber<T>
    {
        return ConvertTo(value, typeof(T), provider);
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(object value, Type numberType, IFormatProvider? provider = null)
    {
        return ConvertTo(value, GetNumberType(numberType), provider);
    }

    /// <summary>
    /// 转换至目标类型
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="type">目标类型</param>
    /// <param name="provider">格式提供者</param>
    /// <returns>转换后的数值</returns>
    /// <exception cref="NotImplementedException">不支持的类型</exception>
    public static object ConvertTo(object value, NumberType type, IFormatProvider? provider = null)
    {
        if (value is IConvertible ic)
        {
            return type switch
            {
                NumberType.SByte => ic.ToSByte(provider),
                NumberType.Byte => ic.ToByte(provider),
                NumberType.Int16 => ic.ToInt16(provider),
                NumberType.UInt16 => ic.ToUInt16(provider),
                NumberType.Int32 => ic.ToInt32(provider),
                NumberType.UInt32 => ic.ToUInt32(provider),
                NumberType.Int64 => ic.ToInt64(provider),
                NumberType.UInt64 => ic.ToUInt64(provider),
                NumberType.Single => ic.ToSingle(provider),
                NumberType.Double => ic.ToDouble(provider),
                NumberType.Decimal => ic.ToDecimal(provider),
                _ => throw CreateUnsupportedNumberTypeException(type),
            };
        }

        return type switch
        {
            NumberType.SByte => Convert.ToSByte(value, provider),
            NumberType.Byte => Convert.ToByte(value, provider),
            NumberType.Int16 => Convert.ToInt16(value, provider),
            NumberType.UInt16 => Convert.ToUInt16(value, provider),
            NumberType.Int32 => Convert.ToInt32(value, provider),
            NumberType.UInt32 => Convert.ToUInt32(value, provider),
            NumberType.Int64 => Convert.ToInt64(value, provider),
            NumberType.UInt64 => Convert.ToUInt64(value, provider),
            NumberType.Single => Convert.ToSingle(value, provider),
            NumberType.Double => Convert.ToDouble(value, provider),
            NumberType.Decimal => Convert.ToDecimal(value, provider),
            _ => throw CreateUnsupportedNumberTypeException(type),
        };
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
#pragma warning disable S3776
    public static object ConvertTo(
#pragma warning restore S3776
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
#pragma warning disable S3776
    public static object ConvertTo(
#pragma warning restore S3776
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
