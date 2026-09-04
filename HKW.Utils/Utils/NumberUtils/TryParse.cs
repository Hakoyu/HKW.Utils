using System;
using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 尝试从 UTF-8 文本解析数值
    /// </summary>
    public static bool TryParse<T>(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider,
        out T result
    )
        where T : struct, INumberBase<T>
    {
        return T.TryParse(utf8Text, style, provider, out result);
    }

    /// <summary>
    /// 尝试从 UTF-8 文本解析数值
    /// </summary>
    public static bool TryParse<T>(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out T result
    )
        where T : struct, INumberBase<T>
    {
        return T.TryParse(utf8Text, provider, out result);
    }

    /// <summary>
    /// 尝试从 UTF-8 文本解析数值
    /// </summary>
    public static bool TryParse<T>(ReadOnlySpan<byte> utf8Text, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(utf8Text, provider: null, out result);
    }

    /// <summary>
    /// 尝试从字符文本解析数值
    /// </summary>
    public static bool TryParse<T>(
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        out T result
    )
        where T : struct, INumberBase<T>
    {
        return T.TryParse(s, style, provider, out result);
    }

    /// <summary>
    /// 尝试从字符文本解析数值
    /// </summary>
    public static bool TryParse<T>(ReadOnlySpan<char> s, IFormatProvider? provider, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(s, provider, out result);
    }

    /// <summary>
    /// 尝试从字符文本解析数值
    /// </summary>
    public static bool TryParse<T>(ReadOnlySpan<char> s, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(s, provider: null, out result);
    }

    /// <summary>
    /// 尝试从字符串解析数值
    /// </summary>
    public static bool TryParse<T>(string? s, IFormatProvider? provider, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(s, provider, out result);
    }

    /// <summary>
    /// 尝试从字符串解析数值
    /// </summary>
    public static bool TryParse<T>(string? s, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(s, provider: null, out result);
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为指定运行时类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="style">允许的数值格式样式。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), utf8Text, style, provider, out result);
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为指定运行时类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), utf8Text, provider, out result);
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为指定运行时类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值的运行时类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(Type numberType, ReadOnlySpan<byte> utf8Text, out object result)
    {
        return TryParse(GetNumberType(numberType), utf8Text, out result);
    }

    /// <summary>
    /// 尝试将字符文本解析为指定数值类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="style">允许的数值格式样式。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), s, style, provider, out result);
    }

    /// <summary>
    /// 尝试将字符文本解析为指定数值类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), s, provider, out result);
    }

    /// <summary>
    /// 尝试将字符文本解析为指定数值类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(Type numberType, ReadOnlySpan<char> s, out object result)
    {
        return TryParse(GetNumberType(numberType), s, out result);
    }

    /// <summary>
    /// 尝试将字符串解析为指定数值类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        Type numberType,
        string? s,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), s, provider, out result);
    }

    /// <summary>
    /// 尝试将字符串解析为指定数值类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(Type numberType, string? s, out object result)
    {
        return TryParse(GetNumberType(numberType), s, out result);
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="style">允许的数值格式样式。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(utf8Text, style, provider, out result),
            NumberType.Byte => TryParseValue<byte>(utf8Text, style, provider, out result),
            NumberType.Int16 => TryParseValue<short>(utf8Text, style, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(utf8Text, style, provider, out result),
            NumberType.Int32 => TryParseValue<int>(utf8Text, style, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(utf8Text, style, provider, out result),
            NumberType.Int64 => TryParseValue<long>(utf8Text, style, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(utf8Text, style, provider, out result),
            NumberType.Single => TryParseValue<float>(utf8Text, style, provider, out result),
            NumberType.Double => TryParseValue<double>(utf8Text, style, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(utf8Text, style, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(utf8Text, provider, out result),
            NumberType.Byte => TryParseValue<byte>(utf8Text, provider, out result),
            NumberType.Int16 => TryParseValue<short>(utf8Text, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(utf8Text, provider, out result),
            NumberType.Int32 => TryParseValue<int>(utf8Text, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(utf8Text, provider, out result),
            NumberType.Int64 => TryParseValue<long>(utf8Text, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(utf8Text, provider, out result),
            NumberType.Single => TryParseValue<float>(utf8Text, provider, out result),
            NumberType.Double => TryParseValue<double>(utf8Text, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(utf8Text, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试将 UTF-8 文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="utf8Text">UTF-8 编码的数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        out object result
    )
    {
        return TryParse(numberType, utf8Text, provider: null, out result);
    }

    /// <summary>
    /// 尝试将字符文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="style">允许的数值格式样式。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(s, style, provider, out result),
            NumberType.Byte => TryParseValue<byte>(s, style, provider, out result),
            NumberType.Int16 => TryParseValue<short>(s, style, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(s, style, provider, out result),
            NumberType.Int32 => TryParseValue<int>(s, style, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(s, style, provider, out result),
            NumberType.Int64 => TryParseValue<long>(s, style, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(s, style, provider, out result),
            NumberType.Single => TryParseValue<float>(s, style, provider, out result),
            NumberType.Double => TryParseValue<double>(s, style, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(s, style, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试将字符文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(s, provider, out result),
            NumberType.Byte => TryParseValue<byte>(s, provider, out result),
            NumberType.Int16 => TryParseValue<short>(s, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(s, provider, out result),
            NumberType.Int32 => TryParseValue<int>(s, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(s, provider, out result),
            NumberType.Int64 => TryParseValue<long>(s, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(s, provider, out result),
            NumberType.Single => TryParseValue<float>(s, provider, out result),
            NumberType.Double => TryParseValue<double>(s, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(s, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试将字符文本解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(NumberType numberType, ReadOnlySpan<char> s, out object result)
    {
        return TryParse(numberType, s, provider: null, out result);
    }

    /// <summary>
    /// 尝试将字符串解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="provider">提供区域性相关格式信息的对象。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(
        NumberType numberType,
        string? s,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(s, provider, out result),
            NumberType.Byte => TryParseValue<byte>(s, provider, out result),
            NumberType.Int16 => TryParseValue<short>(s, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(s, provider, out result),
            NumberType.Int32 => TryParseValue<int>(s, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(s, provider, out result),
            NumberType.Int64 => TryParseValue<long>(s, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(s, provider, out result),
            NumberType.Single => TryParseValue<float>(s, provider, out result),
            NumberType.Double => TryParseValue<double>(s, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(s, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试将字符串解析为 <paramref name="numberType"/> 指定类型的数值。
    /// </summary>
    /// <param name="numberType">目标数值类型。</param>
    /// <param name="s">数值文本。</param>
    /// <param name="result">解析结果；解析失败时为目标类型的默认值。</param>
    /// <returns>解析成功时为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public static bool TryParse(NumberType numberType, string? s, out object result)
    {
        return TryParse(numberType, s, provider: null, out result);
    }

    private static bool TryParseValue<T>(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(utf8Text, style, provider, out T value);
        result = value;
        return success;
    }

    private static bool TryParseValue<T>(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(utf8Text, provider, out T value);
        result = value;
        return success;
    }

    private static bool TryParseValue<T>(
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(s, style, provider, out T value);
        result = value;
        return success;
    }

    private static bool TryParseValue<T>(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(s, provider, out T value);
        result = value;
        return success;
    }

    private static bool TryParseValue<T>(string? s, IFormatProvider? provider, out object result)
        where T : struct, INumberBase<T>
    {
        var success = TryParse(s, provider, out T value);
        result = value;
        return success;
    }
}
