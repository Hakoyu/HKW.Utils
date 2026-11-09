using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 按位运算符字符与类型的不可变双向映射, LogicalOr 对应异或符号 ^.
    /// </summary>
    public static FrozenBidirectionalDictionary<
        char,
        BitwiseOperatorType
    > BitwiseOperatorTypeByChar { get; } =
        new([
            KeyValuePair.Create('|', BitwiseOperatorType.Or),
            KeyValuePair.Create('&', BitwiseOperatorType.And),
            KeyValuePair.Create('^', BitwiseOperatorType.LogicalOr),
        ]);

    /// <summary>
    /// 查找与完整符号对应的按位运算符枚举值.
    /// </summary>
    /// <param name="operator">要匹配的运算符字符.</param>
    /// <returns>与符号对应的运算符枚举值.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="operator"/> 不是受支持的完整运算符.</exception>
    public static BitwiseOperatorType GetBitwiseOperatorType(char @operator)
    {
        return BitwiseOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 尝试查找与完整符号对应的按位运算符枚举值.
    /// </summary>
    /// <param name="operator">要匹配的运算符字符.</param>
    /// <param name="operatorType">返回时包含匹配的枚举值; 未匹配时为枚举默认值.</param>
    /// <returns>找到运算符时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    public static bool TryGetBitwiseOperatorType(
        char @operator,
        out BitwiseOperatorType operatorType
    )
    {
        return BitwiseOperatorTypeByChar.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 转换为指定整数类型后按字符执行按位与, 或或异或运算.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">左操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static T BitwiseOperator<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return BitwiseOperator<T>(value1, value2, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 转换为指定运行时整数类型后按字符执行按位与, 或或异或运算.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        Type numberType,
        char @operator
    )
    {
        return BitwiseOperator(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 转换为枚举指定的整数类型后按字符执行按位与, 或或异或运算.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        NumberType numberType,
        char @operator
    )
    {
        return BitwiseOperator(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 转换为指定整数类型后执行按位与, 或或异或运算.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">左操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseOperator<T>(
        object value1,
        object value2,
        BitwiseOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return (T)BitwiseOperator(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 转换为指定运行时整数类型后执行按位与, 或或异或运算.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        Type numberType,
        BitwiseOperatorType operatorType
    )
    {
        return BitwiseOperator(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 转换整数操作数后执行按位与, 或或异或运算.
    /// </summary>
    /// <param name="value1">左操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="value2">右操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <returns>已装箱且保持目标整数类型的运算结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)BitwiseCore(
                ConvertNumber<sbyte>(value1),
                ConvertNumber<sbyte>(value2),
                operatorType
            ),
            NumberType.Byte => (object)BitwiseCore(
                ConvertNumber<byte>(value1),
                ConvertNumber<byte>(value2),
                operatorType
            ),
            NumberType.Int16 => (object)BitwiseCore(
                ConvertNumber<short>(value1),
                ConvertNumber<short>(value2),
                operatorType
            ),
            NumberType.UInt16 => (object)BitwiseCore(
                ConvertNumber<ushort>(value1),
                ConvertNumber<ushort>(value2),
                operatorType
            ),
            NumberType.Int32 => (object)BitwiseCore(
                ConvertNumber<int>(value1),
                ConvertNumber<int>(value2),
                operatorType
            ),
            NumberType.UInt32 => (object)BitwiseCore(
                ConvertNumber<uint>(value1),
                ConvertNumber<uint>(value2),
                operatorType
            ),
            NumberType.Int64 => (object)BitwiseCore(
                ConvertNumber<long>(value1),
                ConvertNumber<long>(value2),
                operatorType
            ),
            NumberType.UInt64 => (object)BitwiseCore(
                ConvertNumber<ulong>(value1),
                ConvertNumber<ulong>(value2),
                operatorType
            ),
            NumberType.IntPtr => (object)BitwiseCore(
                ConvertNumber<nint>(value1),
                ConvertNumber<nint>(value2),
                operatorType
            ),
            NumberType.UIntPtr => (object)BitwiseCore(
                ConvertNumber<nuint>(value1),
                ConvertNumber<nuint>(value2),
                operatorType
            ),
            NumberType.Int128 => (object)BitwiseCore(
                ConvertNumber<Int128>(value1),
                ConvertNumber<Int128>(value2),
                operatorType
            ),
            NumberType.UInt128 => (object)BitwiseCore(
                ConvertNumber<UInt128>(value1),
                ConvertNumber<UInt128>(value2),
                operatorType
            ),
            NumberType.BigInteger => (object)BitwiseCore(
                ConvertNumber<BigInteger>(value1),
                ConvertNumber<BigInteger>(value2),
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
