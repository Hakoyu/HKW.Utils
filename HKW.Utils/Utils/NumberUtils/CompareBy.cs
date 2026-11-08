using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 关系运算符字符串与类型的不可变双向映射.
    /// </summary>
    public static FrozenBidirectionalDictionary<
        string,
        ComparisonOperatorType
    > ComparisonOperatorTypeByString { get; } =
        new([
            KeyValuePair.Create("==", ComparisonOperatorType.Equality),
            KeyValuePair.Create("!=", ComparisonOperatorType.Inequality),
            KeyValuePair.Create("<=", ComparisonOperatorType.LessThanOrEqual),
            KeyValuePair.Create(">=", ComparisonOperatorType.GreaterThanOrEqual),
            KeyValuePair.Create("<", ComparisonOperatorType.LessThan),
            KeyValuePair.Create(">", ComparisonOperatorType.GreaterThan),
        ]);

    /// <summary>
    /// 查找与完整符号对应的关系运算符枚举值.
    /// </summary>
    /// <param name="str">要完整匹配的运算符字符串, 不忽略空白.</param>
    /// <returns>与符号对应的运算符枚举值.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="str"/> 不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <see langword="null"/>.</exception>
    public static ComparisonOperatorType GetComparisonOperatorType(string str)
    {
        return ComparisonOperatorTypeByString[str];
    }

    /// <summary>
    /// 匹配字符串开头的最长关系运算符, 优先匹配两个字符, 不验证后续表达式或忽略前导空白.
    /// </summary>
    /// <param name="str">要匹配运算符前缀的字符串.</param>
    /// <param name="operatorLength">匹配的符号长度; 未匹配时为零.</param>
    /// <returns>匹配的关系运算符枚举值; 未匹配时为枚举默认值, 应通过符号长度判断是否成功.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <see langword="null"/>.</exception>
    public static ComparisonOperatorType GetComparisonOperatorType(
        string str,
        out int operatorLength
    )
    {
        ArgumentNullException.ThrowIfNull(str);
        var (operatorType, length) = str.AsSpan() switch
        {
            ['=', '=', ..] => (ComparisonOperatorType.Equality, 2),
            ['!', '=', ..] => (ComparisonOperatorType.Inequality, 2),
            ['<', '=', ..] => (ComparisonOperatorType.LessThanOrEqual, 2),
            ['>', '=', ..] => (ComparisonOperatorType.GreaterThanOrEqual, 2),
            ['<', ..] => (ComparisonOperatorType.LessThan, 1),
            ['>', ..] => (ComparisonOperatorType.GreaterThan, 1),
            _ => (default(ComparisonOperatorType), 0),
        };
        operatorLength = length;
        return operatorType;
    }

    /// <summary>
    /// 转换为指定类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareBy<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareBy<T>(value1, value2, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 转换为指定运行时类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareBy(object value1, object value2, Type numberType, string @operator)
    {
        return CompareBy(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 转换为枚举指定的类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareBy(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return CompareBy(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 转换为指定类型后执行关系比较; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    public static bool CompareBy<T>(
        object value1,
        object value2,
        ComparisonOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return CompareBy(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 转换为指定运行时类型后执行关系比较; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static bool CompareBy(
        object value1,
        object value2,
        Type numberType,
        ComparisonOperatorType operatorType
    )
    {
        return CompareBy(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 转换后直接使用关系运算符比较, 不通过 CompareTo 推导结果; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <param name="value1">左操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="value2">右操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    public static bool CompareBy(
        object value1,
        object value2,
        NumberType numberType,
        ComparisonOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => CompareByCore(
                ConvertNumber<sbyte>(value1),
                ConvertNumber<sbyte>(value2),
                operatorType
            ),
            NumberType.Byte => CompareByCore(
                ConvertNumber<byte>(value1),
                ConvertNumber<byte>(value2),
                operatorType
            ),
            NumberType.Int16 => CompareByCore(
                ConvertNumber<short>(value1),
                ConvertNumber<short>(value2),
                operatorType
            ),
            NumberType.UInt16 => CompareByCore(
                ConvertNumber<ushort>(value1),
                ConvertNumber<ushort>(value2),
                operatorType
            ),
            NumberType.Int32 => CompareByCore(
                ConvertNumber<int>(value1),
                ConvertNumber<int>(value2),
                operatorType
            ),
            NumberType.UInt32 => CompareByCore(
                ConvertNumber<uint>(value1),
                ConvertNumber<uint>(value2),
                operatorType
            ),
            NumberType.Int64 => CompareByCore(
                ConvertNumber<long>(value1),
                ConvertNumber<long>(value2),
                operatorType
            ),
            NumberType.UInt64 => CompareByCore(
                ConvertNumber<ulong>(value1),
                ConvertNumber<ulong>(value2),
                operatorType
            ),
            NumberType.Single => CompareByCore(
                ConvertNumber<float>(value1),
                ConvertNumber<float>(value2),
                operatorType
            ),
            NumberType.Double => CompareByCore(
                ConvertNumber<double>(value1),
                ConvertNumber<double>(value2),
                operatorType
            ),
            NumberType.Decimal => CompareByCore(
                ConvertNumber<decimal>(value1),
                ConvertNumber<decimal>(value2),
                operatorType
            ),
            NumberType.IntPtr => CompareByCore(
                ConvertNumber<nint>(value1),
                ConvertNumber<nint>(value2),
                operatorType
            ),
            NumberType.UIntPtr => CompareByCore(
                ConvertNumber<nuint>(value1),
                ConvertNumber<nuint>(value2),
                operatorType
            ),
            NumberType.Int128 => CompareByCore(
                ConvertNumber<Int128>(value1),
                ConvertNumber<Int128>(value2),
                operatorType
            ),
            NumberType.UInt128 => CompareByCore(
                ConvertNumber<UInt128>(value1),
                ConvertNumber<UInt128>(value2),
                operatorType
            ),
            NumberType.Half => CompareByCore(
                ConvertNumber<Half>(value1),
                ConvertNumber<Half>(value2),
                operatorType
            ),
            NumberType.BigInteger => CompareByCore(
                ConvertNumber<BigInteger>(value1),
                ConvertNumber<BigInteger>(value2),
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
