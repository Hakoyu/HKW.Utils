using System;
using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 从 UTF-8 文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider
    )
        where T : struct, INumberBase<T>
    {
        return T.Parse(utf8Text, style, provider);
    }

    /// <summary>
    /// 从 UTF-8 文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(utf8Text, provider);
    }

    /// <summary>
    /// 从 UTF-8 文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(ReadOnlySpan<byte> utf8Text)
        where T : struct, INumberBase<T>
    {
        return T.Parse(utf8Text, provider: null);
    }

    /// <summary>
    /// 从字符文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(s, style, provider);
    }

    /// <summary>
    /// 从字符文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(s, provider);
    }

    /// <summary>
    /// 从字符文本解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="s">要解析的字符文本</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(ReadOnlySpan<char> s)
        where T : struct, INumberBase<T>
    {
        return T.Parse(s, provider: null);
    }

    /// <summary>
    /// 从字符串解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="s">要解析的字符串</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(string s, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(s, provider);
    }

    /// <summary>
    /// 从字符串解析数值
    /// </summary>
    /// <typeparam name="T">目标数值类型</typeparam>
    /// <param name="s">要解析的字符串</param>
    /// <returns>解析后的数值</returns>
    public static T Parse<T>(string s)
        where T : struct, INumberBase<T>
    {
        return T.Parse(s, provider: null);
    }

    /// <summary>
    /// 将 UTF-8 文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        Type numberType,
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return Parse(GetNumberType(numberType), utf8Text, style, provider);
    }

    /// <summary>
    /// 将 UTF-8 文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        Type numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider
    )
    {
        return Parse(GetNumberType(numberType), utf8Text, provider);
    }

    /// <summary>
    /// 将 UTF-8 文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(Type numberType, ReadOnlySpan<byte> utf8Text)
    {
        return Parse(GetNumberType(numberType), utf8Text);
    }

    /// <summary>
    /// 将字符文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        Type numberType,
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return Parse(GetNumberType(numberType), s, style, provider);
    }

    /// <summary>
    /// 将字符文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(Type numberType, ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return Parse(GetNumberType(numberType), s, provider);
    }

    /// <summary>
    /// 将字符文本解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="s">要解析的字符文本</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(Type numberType, ReadOnlySpan<char> s)
    {
        return Parse(GetNumberType(numberType), s);
    }

    /// <summary>
    /// 将字符串解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="s">要解析的字符串</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(Type numberType, string s, IFormatProvider? provider)
    {
        return Parse(GetNumberType(numberType), s, provider);
    }

    /// <summary>
    /// 将字符串解析为指定运行时类型的数值
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型</param>
    /// <param name="s">要解析的字符串</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(Type numberType, string s)
    {
        return Parse(GetNumberType(numberType), s);
    }

    /// <summary>
    /// 将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => Parse<sbyte>(utf8Text, style, provider),
            NumberType.Byte => Parse<byte>(utf8Text, style, provider),
            NumberType.Int16 => Parse<short>(utf8Text, style, provider),
            NumberType.UInt16 => Parse<ushort>(utf8Text, style, provider),
            NumberType.Int32 => Parse<int>(utf8Text, style, provider),
            NumberType.UInt32 => Parse<uint>(utf8Text, style, provider),
            NumberType.Int64 => Parse<long>(utf8Text, style, provider),
            NumberType.UInt64 => Parse<ulong>(utf8Text, style, provider),
            NumberType.Single => Parse<float>(utf8Text, style, provider),
            NumberType.Double => Parse<double>(utf8Text, style, provider),
            NumberType.Decimal => Parse<decimal>(utf8Text, style, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => Parse<sbyte>(utf8Text, provider),
            NumberType.Byte => Parse<byte>(utf8Text, provider),
            NumberType.Int16 => Parse<short>(utf8Text, provider),
            NumberType.UInt16 => Parse<ushort>(utf8Text, provider),
            NumberType.Int32 => Parse<int>(utf8Text, provider),
            NumberType.UInt32 => Parse<uint>(utf8Text, provider),
            NumberType.Int64 => Parse<long>(utf8Text, provider),
            NumberType.UInt64 => Parse<ulong>(utf8Text, provider),
            NumberType.Single => Parse<float>(utf8Text, provider),
            NumberType.Double => Parse<double>(utf8Text, provider),
            NumberType.Decimal => Parse<decimal>(utf8Text, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="utf8Text">要解析的 UTF-8 文本</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(NumberType numberType, ReadOnlySpan<byte> utf8Text)
    {
        return Parse(numberType, utf8Text, provider: null);
    }

    /// <summary>
    /// 将字符文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="style">允许的数字格式样式</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => Parse<sbyte>(s, style, provider),
            NumberType.Byte => Parse<byte>(s, style, provider),
            NumberType.Int16 => Parse<short>(s, style, provider),
            NumberType.UInt16 => Parse<ushort>(s, style, provider),
            NumberType.Int32 => Parse<int>(s, style, provider),
            NumberType.UInt32 => Parse<uint>(s, style, provider),
            NumberType.Int64 => Parse<long>(s, style, provider),
            NumberType.UInt64 => Parse<ulong>(s, style, provider),
            NumberType.Single => Parse<float>(s, style, provider),
            NumberType.Double => Parse<double>(s, style, provider),
            NumberType.Decimal => Parse<decimal>(s, style, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 将字符文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="s">要解析的字符文本</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<char> s,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => Parse<sbyte>(s, provider),
            NumberType.Byte => Parse<byte>(s, provider),
            NumberType.Int16 => Parse<short>(s, provider),
            NumberType.UInt16 => Parse<ushort>(s, provider),
            NumberType.Int32 => Parse<int>(s, provider),
            NumberType.UInt32 => Parse<uint>(s, provider),
            NumberType.Int64 => Parse<long>(s, provider),
            NumberType.UInt64 => Parse<ulong>(s, provider),
            NumberType.Single => Parse<float>(s, provider),
            NumberType.Double => Parse<double>(s, provider),
            NumberType.Decimal => Parse<decimal>(s, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 将字符文本解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="s">要解析的字符文本</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(NumberType numberType, ReadOnlySpan<char> s)
    {
        return Parse(numberType, s, provider: null);
    }

    /// <summary>
    /// 将字符串解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="s">要解析的字符串</param>
    /// <param name="provider">用于区域性格式的提供程序</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(NumberType numberType, string s, IFormatProvider? provider)
    {
        return numberType switch
        {
            NumberType.SByte => Parse<sbyte>(s, provider),
            NumberType.Byte => Parse<byte>(s, provider),
            NumberType.Int16 => Parse<short>(s, provider),
            NumberType.UInt16 => Parse<ushort>(s, provider),
            NumberType.Int32 => Parse<int>(s, provider),
            NumberType.UInt32 => Parse<uint>(s, provider),
            NumberType.Int64 => Parse<long>(s, provider),
            NumberType.UInt64 => Parse<ulong>(s, provider),
            NumberType.Single => Parse<float>(s, provider),
            NumberType.Double => Parse<double>(s, provider),
            NumberType.Decimal => Parse<decimal>(s, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 将字符串解析为 <paramref name="numberType"/> 指定类型的数值
    /// </summary>
    /// <param name="numberType">目标数值类型枚举</param>
    /// <param name="s">要解析的字符串</param>
    /// <returns>解析后的数值对象</returns>
    public static object Parse(NumberType numberType, string s)
    {
        return Parse(numberType, s, provider: null);
    }
}
