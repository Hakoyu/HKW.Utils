using System.Globalization;
using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 提供数值转换, 比较及运算工具.
/// </summary>
public static partial class NumberUtils
{
    #region object
    /// <summary>
    /// 将对象转换为目标数值类型, 数值输入使用 CreateChecked 转换.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value">待转换的对象; 为 <see langword="null"/> 时返回零.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>转换后的数值; 数值的小数部分转整数时向零截断, 浮点转换遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><typeparamref name="T"/> 不是受支持的数值类型.</exception>
    /// <exception cref="InvalidCastException">对象不支持到目标类型的转换.</exception>
    /// <exception cref="FormatException">文本格式不符合目标类型要求.</exception>
    /// <exception cref="OverflowException">目标类型的解析或检查转换报告溢出.</exception>
    public static T ConvertTo<T>(object value, IFormatProvider? provider = null)
        where T : struct, INumber<T>
    {
        return (T)ConvertTo(value, typeof(T), provider);
    }

    /// <summary>
    /// 将对象转换为指定运行时数值类型, 数值输入使用 CreateChecked 转换.
    /// </summary>
    /// <param name="value">待转换的对象; 为 <see langword="null"/> 时返回零.</param>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 数值的小数部分转整数时向零截断, 浮点转换遵循目标类型规则.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="InvalidCastException">对象不支持到目标类型的转换.</exception>
    /// <exception cref="FormatException">文本格式不符合目标类型要求.</exception>
    /// <exception cref="OverflowException">目标类型的解析或检查转换报告溢出.</exception>
    public static object ConvertTo(object value, Type numberType, IFormatProvider? provider = null)
    {
        return ConvertTo(value, GetNumberType(numberType), provider);
    }

    /// <summary>
    /// 将对象转换为指定数值类型, 数值输入使用 CreateChecked 转换.
    /// </summary>
    /// <param name="value">待转换的对象; 为 <see langword="null"/> 时返回零.</param>
    /// <param name="type">本工具支持的目标数值类型枚举值.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 数值的小数部分转整数时向零截断, 浮点转换遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="type"/> 不是受支持的数值类型.</exception>
    /// <exception cref="InvalidCastException">对象不支持到目标类型的转换.</exception>
    /// <exception cref="FormatException">文本格式不符合目标类型要求.</exception>
    /// <exception cref="OverflowException">目标类型的解析或检查转换报告溢出.</exception>
    public static object ConvertTo(object value, NumberType type, IFormatProvider? provider = null)
    {
        return type switch
        {
            NumberType.SByte => ConvertNumber<sbyte>(value, provider),
            NumberType.Byte => ConvertNumber<byte>(value, provider),
            NumberType.Int16 => ConvertNumber<short>(value, provider),
            NumberType.UInt16 => ConvertNumber<ushort>(value, provider),
            NumberType.Int32 => ConvertNumber<int>(value, provider),
            NumberType.UInt32 => ConvertNumber<uint>(value, provider),
            NumberType.Int64 => ConvertNumber<long>(value, provider),
            NumberType.UInt64 => ConvertNumber<ulong>(value, provider),
            NumberType.Single => ConvertNumber<float>(value, provider),
            NumberType.Double => ConvertNumber<double>(value, provider),
            NumberType.Decimal => ConvertNumber<decimal>(value, provider),
            NumberType.IntPtr => ConvertNumber<nint>(value, provider),
            NumberType.UIntPtr => ConvertNumber<nuint>(value, provider),
            NumberType.Int128 => ConvertNumber<Int128>(value, provider),
            NumberType.UInt128 => ConvertNumber<UInt128>(value, provider),
            NumberType.Half => ConvertNumber<Half>(value, provider),
            NumberType.BigInteger => ConvertNumber<BigInteger>(value, provider),
            _ => throw CreateUnsupportedNumberTypeException(type),
        };
    }
    #endregion

    #region ReadOnlySpan
    /// <summary>
    /// 将字符文本解析为目标数值类型.
    /// </summary>
    /// <typeparam name="T">实现泛型数值接口的目标值类型.</typeparam>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="style">允许的格式样式; 为 <see langword="null"/> 时使用目标类型的默认样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>解析得到的数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static T ConvertTo<T>(
        ReadOnlySpan<char> text,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
        where T : struct, INumber<T>
    {
        if (style is null)
            return T.Parse(text, provider);
        else
            return T.Parse(text, style.Value, provider);
    }

    /// <summary>
    /// 将字符文本解析为指定运行时数值类型.
    /// </summary>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="numberType">本工具支持的目标数值运行时类型.</param>
    /// <param name="style">允许的格式样式; 为 <see langword="null"/> 时使用目标类型的默认样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 为 <see langword="null"/> 或不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object ConvertTo(
        ReadOnlySpan<char> text,
        Type numberType,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
    {
        if (numberType is null)
            throw new NotImplementedException();
        return ConvertTo(text, GetNumberType(numberType), style, provider);
    }

    /// <summary>
    /// 将字符文本解析为指定数值类型.
    /// </summary>
    /// <param name="text">待解析的字符文本; 空字符串和 null 字符串转换为此参数时均为空跨度.</param>
    /// <param name="numberType">本工具支持的目标数值类型枚举值.</param>
    /// <param name="style">允许的格式样式; 为 <see langword="null"/> 时使用目标类型的默认样式.</param>
    /// <param name="provider">格式提供程序; 为 <see langword="null"/> 时使用当前区域性.</param>
    /// <returns>已装箱的目标类型数值; 浮点溢出遵循目标类型规则.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    /// <exception cref="ArgumentException"><paramref name="style"/> 不是目标类型支持的有效样式组合.</exception>
    /// <exception cref="FormatException">目标类型报告文本为空或格式无效.</exception>
    /// <exception cref="OverflowException">目标类型报告数值超出可表示范围.</exception>
    public static object ConvertTo(
        ReadOnlySpan<char> text,
        NumberType numberType,
        NumberStyles? style = null,
        IFormatProvider? provider = null
    )
    {
        return style is null
            ? Parse(numberType, text, provider)
            : Parse(numberType, text, style.Value, provider);
    }
    #endregion
}
