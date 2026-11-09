using System;
using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 按指定样式将 UTF-8 文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">目标类型支持的数值格式样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
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
    /// 按目标类型的默认样式将 UTF-8 文本解析为数值.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T Parse<T>(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(utf8Text, provider);
    }

    /// <summary>
    /// 按目标类型的默认样式和当前区域性解析 UTF-8 文本.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T Parse<T>(ReadOnlySpan<byte> utf8Text)
        where T : struct, INumberBase<T>
    {
        return T.Parse(utf8Text, provider: null);
    }

    /// <summary>
    /// 按指定样式将字符文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="style">目标类型支持的数值格式样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T Parse<T>(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(text, style, provider);
    }

    /// <summary>
    /// 按目标类型的默认样式将字符文本解析为数值.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T Parse<T>(ReadOnlySpan<char> text, IFormatProvider? provider)
        where T : struct, INumberBase<T>
    {
        return T.Parse(text, provider);
    }

    /// <summary>
    /// 按目标类型的默认样式和当前区域性解析字符文本.
    /// </summary>
    /// <typeparam name="T">实现泛型数值解析接口的目标值类型.</typeparam>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T Parse<T>(ReadOnlySpan<char> text)
        where T : struct, INumberBase<T>
    {
        return T.Parse(text, provider: null);
    }

    /// <summary>
    /// 按指定样式将 UTF-8 文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">目标类型支持的数值格式样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
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
    /// 按默认样式将 UTF-8 文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        Type numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider
    )
    {
        return Parse(GetNumberType(numberType), utf8Text, provider);
    }

    /// <summary>
    /// 按默认样式和当前区域性将 UTF-8 文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(Type numberType, ReadOnlySpan<byte> utf8Text)
    {
        return Parse(GetNumberType(numberType), utf8Text);
    }

    /// <summary>
    /// 按指定样式将字符文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="style">目标类型支持的数值格式样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        Type numberType,
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return Parse(GetNumberType(numberType), text, style, provider);
    }

    /// <summary>
    /// 按默认样式将字符文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(Type numberType, ReadOnlySpan<char> text, IFormatProvider? provider)
    {
        return Parse(GetNumberType(numberType), text, provider);
    }

    /// <summary>
    /// 按默认样式和当前区域性将字符文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(Type numberType, ReadOnlySpan<char> text)
    {
        return Parse(GetNumberType(numberType), text);
    }

    /// <summary>
    /// 按指定样式将 UTF-8 文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)Parse<sbyte>(utf8Text, style, provider),
            NumberType.Byte => (object)Parse<byte>(utf8Text, style, provider),
            NumberType.Int16 => (object)Parse<short>(utf8Text, style, provider),
            NumberType.UInt16 => (object)Parse<ushort>(utf8Text, style, provider),
            NumberType.Int32 => (object)Parse<int>(utf8Text, style, provider),
            NumberType.UInt32 => (object)Parse<uint>(utf8Text, style, provider),
            NumberType.Int64 => (object)Parse<long>(utf8Text, style, provider),
            NumberType.UInt64 => (object)Parse<ulong>(utf8Text, style, provider),
            NumberType.Single => (object)Parse<float>(utf8Text, style, provider),
            NumberType.Double => (object)Parse<double>(utf8Text, style, provider),
            NumberType.Decimal => (object)Parse<decimal>(utf8Text, style, provider),
            NumberType.IntPtr => (object)Parse<nint>(utf8Text, style, provider),
            NumberType.UIntPtr => (object)Parse<nuint>(utf8Text, style, provider),
            NumberType.Int128 => (object)Parse<Int128>(utf8Text, style, provider),
            NumberType.UInt128 => (object)Parse<UInt128>(utf8Text, style, provider),
            NumberType.Half => (object)Parse<Half>(utf8Text, style, provider),
            NumberType.BigInteger => (object)Parse<BigInteger>(utf8Text, style, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 按默认样式将 UTF-8 文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <param name="provider">格式提供者, 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)Parse<sbyte>(utf8Text, provider),
            NumberType.Byte => (object)Parse<byte>(utf8Text, provider),
            NumberType.Int16 => (object)Parse<short>(utf8Text, provider),
            NumberType.UInt16 => (object)Parse<ushort>(utf8Text, provider),
            NumberType.Int32 => (object)Parse<int>(utf8Text, provider),
            NumberType.UInt32 => (object)Parse<uint>(utf8Text, provider),
            NumberType.Int64 => (object)Parse<long>(utf8Text, provider),
            NumberType.UInt64 => (object)Parse<ulong>(utf8Text, provider),
            NumberType.Single => (object)Parse<float>(utf8Text, provider),
            NumberType.Double => (object)Parse<double>(utf8Text, provider),
            NumberType.Decimal => (object)Parse<decimal>(utf8Text, provider),
            NumberType.IntPtr => (object)Parse<nint>(utf8Text, provider),
            NumberType.UIntPtr => (object)Parse<nuint>(utf8Text, provider),
            NumberType.Int128 => (object)Parse<Int128>(utf8Text, provider),
            NumberType.UInt128 => (object)Parse<UInt128>(utf8Text, provider),
            NumberType.Half => (object)Parse<Half>(utf8Text, provider),
            NumberType.BigInteger => (object)Parse<BigInteger>(utf8Text, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 按默认样式和当前区域性将 UTF-8 文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="utf8Text">待解析的 UTF-8 字节文本.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(NumberType numberType, ReadOnlySpan<byte> utf8Text)
    {
        return Parse(numberType, utf8Text, provider: null);
    }

    /// <summary>
    /// 按指定样式将字符文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="style">允许的数值格式样式.</param>
    /// <param name="provider">格式提供者, 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<char> text,
        NumberStyles style,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)Parse<sbyte>(text, style, provider),
            NumberType.Byte => (object)Parse<byte>(text, style, provider),
            NumberType.Int16 => (object)Parse<short>(text, style, provider),
            NumberType.UInt16 => (object)Parse<ushort>(text, style, provider),
            NumberType.Int32 => (object)Parse<int>(text, style, provider),
            NumberType.UInt32 => (object)Parse<uint>(text, style, provider),
            NumberType.Int64 => (object)Parse<long>(text, style, provider),
            NumberType.UInt64 => (object)Parse<ulong>(text, style, provider),
            NumberType.Single => (object)Parse<float>(text, style, provider),
            NumberType.Double => (object)Parse<double>(text, style, provider),
            NumberType.Decimal => (object)Parse<decimal>(text, style, provider),
            NumberType.IntPtr => (object)Parse<nint>(text, style, provider),
            NumberType.UIntPtr => (object)Parse<nuint>(text, style, provider),
            NumberType.Int128 => (object)Parse<Int128>(text, style, provider),
            NumberType.UInt128 => (object)Parse<UInt128>(text, style, provider),
            NumberType.Half => (object)Parse<Half>(text, style, provider),
            NumberType.BigInteger => (object)Parse<BigInteger>(text, style, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 按默认样式将字符文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="provider">格式提供者, 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(
        NumberType numberType,
        ReadOnlySpan<char> text,
        IFormatProvider? provider
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)Parse<sbyte>(text, provider),
            NumberType.Byte => (object)Parse<byte>(text, provider),
            NumberType.Int16 => (object)Parse<short>(text, provider),
            NumberType.UInt16 => (object)Parse<ushort>(text, provider),
            NumberType.Int32 => (object)Parse<int>(text, provider),
            NumberType.UInt32 => (object)Parse<uint>(text, provider),
            NumberType.Int64 => (object)Parse<long>(text, provider),
            NumberType.UInt64 => (object)Parse<ulong>(text, provider),
            NumberType.Single => (object)Parse<float>(text, provider),
            NumberType.Double => (object)Parse<double>(text, provider),
            NumberType.Decimal => (object)Parse<decimal>(text, provider),
            NumberType.IntPtr => (object)Parse<nint>(text, provider),
            NumberType.UIntPtr => (object)Parse<nuint>(text, provider),
            NumberType.Int128 => (object)Parse<Int128>(text, provider),
            NumberType.UInt128 => (object)Parse<UInt128>(text, provider),
            NumberType.Half => (object)Parse<Half>(text, provider),
            NumberType.BigInteger => (object)Parse<BigInteger>(text, provider),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 按默认样式和当前区域性将字符文本解析为指定数值类型.
    /// </summary>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object Parse(NumberType numberType, ReadOnlySpan<char> text)
    {
        return Parse(numberType, text, provider: null);
    }
}
