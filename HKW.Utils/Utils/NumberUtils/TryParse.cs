using System;
using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 尝试按指定样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
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
    /// 尝试按默认样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
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
    /// 尝试按默认样式和当前区域性将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    public static bool TryParse<T>(ReadOnlySpan<byte> utf8Text, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(utf8Text, provider: null, out result);
    }

    /// <summary>
    /// 尝试按指定样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider,
        out T result
    )
        where T : struct, INumberBase<T>
    {
        return T.TryParse(text, style, provider, out result);
    }

    /// <summary>
    /// 尝试按默认样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    public static bool TryParse<T>(ReadOnlySpan<char> text, IFormatProvider? provider, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(text, provider, out result);
    }

    /// <summary>
    /// 尝试按默认样式和当前区域性将字符文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="result">解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    public static bool TryParse<T>(ReadOnlySpan<char> text, out T result)
        where T : struct, INumberBase<T>
    {
        return T.TryParse(text, provider: null, out result);
    }

    /// <summary>
    /// 尝试按指定样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
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
    /// 尝试按默认样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
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
    /// 尝试按默认样式和当前区域性将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(Type numberType, ReadOnlySpan<byte> utf8Text, out object result)
    {
        return TryParse(GetNumberType(numberType), utf8Text, out result);
    }

    /// <summary>
    /// 尝试按指定样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), text, style, provider, out result);
    }

    /// <summary>
    /// 尝试按默认样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(
        Type numberType,
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        out object result
    )
    {
        return TryParse(GetNumberType(numberType), text, provider, out result);
    }

    /// <summary>
    /// 尝试按默认样式和当前区域性将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">目标类型为 null.</exception>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(Type numberType, ReadOnlySpan<char> text, out object result)
    {
        return TryParse(GetNumberType(numberType), text, out result);
    }

    /// <summary>
    /// 尝试按指定样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
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
            NumberType.IntPtr => TryParseValue<nint>(utf8Text, style, provider, out result),
            NumberType.UIntPtr => TryParseValue<nuint>(utf8Text, style, provider, out result),
            NumberType.Int128 => TryParseValue<Int128>(utf8Text, style, provider, out result),
            NumberType.UInt128 => TryParseValue<UInt128>(utf8Text, style, provider, out result),
            NumberType.Half => TryParseValue<Half>(utf8Text, style, provider, out result),
            NumberType.BigInteger => TryParseValue<BigInteger>(
                utf8Text,
                style,
                provider,
                out result
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试按默认样式将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
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
            NumberType.IntPtr => TryParseValue<nint>(utf8Text, provider, out result),
            NumberType.UIntPtr => TryParseValue<nuint>(utf8Text, provider, out result),
            NumberType.Int128 => TryParseValue<Int128>(utf8Text, provider, out result),
            NumberType.UInt128 => TryParseValue<UInt128>(utf8Text, provider, out result),
            NumberType.Half => TryParseValue<Half>(utf8Text, provider, out result),
            NumberType.BigInteger => TryParseValue<BigInteger>(utf8Text, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试按默认样式和当前区域性将 UTF-8 字节文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        out object result
    )
    {
        return TryParse(numberType, utf8Text, provider: null, out result);
    }

    /// <summary>
    /// 尝试按指定样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(text, style, provider, out result),
            NumberType.Byte => TryParseValue<byte>(text, style, provider, out result),
            NumberType.Int16 => TryParseValue<short>(text, style, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(text, style, provider, out result),
            NumberType.Int32 => TryParseValue<int>(text, style, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(text, style, provider, out result),
            NumberType.Int64 => TryParseValue<long>(text, style, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(text, style, provider, out result),
            NumberType.Single => TryParseValue<float>(text, style, provider, out result),
            NumberType.Double => TryParseValue<double>(text, style, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(text, style, provider, out result),
            NumberType.IntPtr => TryParseValue<nint>(text, style, provider, out result),
            NumberType.UIntPtr => TryParseValue<nuint>(text, style, provider, out result),
            NumberType.Int128 => TryParseValue<Int128>(text, style, provider, out result),
            NumberType.UInt128 => TryParseValue<UInt128>(text, style, provider, out result),
            NumberType.Half => TryParseValue<Half>(text, style, provider, out result),
            NumberType.BigInteger => TryParseValue<BigInteger>(text, style, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试按默认样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(
        NumberType numberType,
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        out object result
    )
    {
        return numberType switch
        {
            NumberType.SByte => TryParseValue<sbyte>(text, provider, out result),
            NumberType.Byte => TryParseValue<byte>(text, provider, out result),
            NumberType.Int16 => TryParseValue<short>(text, provider, out result),
            NumberType.UInt16 => TryParseValue<ushort>(text, provider, out result),
            NumberType.Int32 => TryParseValue<int>(text, provider, out result),
            NumberType.UInt32 => TryParseValue<uint>(text, provider, out result),
            NumberType.Int64 => TryParseValue<long>(text, provider, out result),
            NumberType.UInt64 => TryParseValue<ulong>(text, provider, out result),
            NumberType.Single => TryParseValue<float>(text, provider, out result),
            NumberType.Double => TryParseValue<double>(text, provider, out result),
            NumberType.Decimal => TryParseValue<decimal>(text, provider, out result),
            NumberType.IntPtr => TryParseValue<nint>(text, provider, out result),
            NumberType.UIntPtr => TryParseValue<nuint>(text, provider, out result),
            NumberType.Int128 => TryParseValue<Int128>(text, provider, out result),
            NumberType.UInt128 => TryParseValue<UInt128>(text, provider, out result),
            NumberType.Half => TryParseValue<Half>(text, provider, out result),
            NumberType.BigInteger => TryParseValue<BigInteger>(text, provider, out result),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 尝试按默认样式和当前区域性将字符文本解析为目标数值类型.
    /// </summary>
    /// <param name="numberType">目标数值类型.</param>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="result">装箱后的解析结果, 失败时为目标类型的零.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    public static bool TryParse(NumberType numberType, ReadOnlySpan<char> text, out object result)
    {
        return TryParse(numberType, text, provider: null, out result);
    }

    /// <summary>
    /// 尝试按指定样式解析 UTF-8 字节文本并装箱结果.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
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

    /// <summary>
    /// 尝试按默认样式解析 UTF-8 字节文本并装箱结果.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
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

    /// <summary>
    /// 尝试按指定样式解析字符文本并装箱结果.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentException">目标类型不支持指定的样式组合.</exception>
    private static bool TryParseValue<T>(
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(text, style, provider, out T value);
        result = value;
        return success;
    }

    /// <summary>
    /// 尝试按默认样式解析字符文本并装箱结果.
    /// </summary>
    /// <typeparam name="T">目标数值类型.</typeparam>
    /// <param name="text">待解析的字符文本.</param>
    /// <param name="provider">格式提供者, 为 null 时使用当前区域性.</param>
    /// <param name="result">装箱后的解析结果, 失败值由目标类型决定.</param>
    /// <returns>是否成功</returns>
    private static bool TryParseValue<T>(
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        out object result
    )
        where T : struct, INumberBase<T>
    {
        var success = TryParse(text, provider, out T value);
        result = value;
        return success;
    }
}
