using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 移位运算符字符串与类型的不可变双向映射.
    /// </summary>
    public static FrozenBidirectionalDictionary<
        string,
        BitwiseShiftType
    > BitwiseShiftTypeByString { get; } =
        new([
            KeyValuePair.Create("<<", BitwiseShiftType.Left),
            KeyValuePair.Create(">>", BitwiseShiftType.Right),
            KeyValuePair.Create(">>>", BitwiseShiftType.UnsignedRight),
        ]);

    /// <summary>
    /// 查找与完整符号对应的移位运算符枚举值.
    /// </summary>
    /// <param name="operator">要完整匹配的运算符字符串, 不忽略空白.</param>
    /// <returns>与符号对应的运算符枚举值.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="operator"/> 不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static BitwiseShiftType GetBitwiseShiftType(string @operator)
    {
        return BitwiseShiftTypeByString[@operator];
    }

    /// <summary>
    /// 尝试查找与完整符号对应的移位运算符枚举值.
    /// </summary>
    /// <param name="operator">要完整匹配的运算符字符串, 不忽略空白.</param>
    /// <param name="operatorType">返回时包含匹配的枚举值; 未匹配时为枚举默认值.</param>
    /// <returns>找到运算符时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool TryGetBitwiseShiftType(string @operator, out BitwiseShiftType operatorType)
    {
        return BitwiseShiftTypeByString.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 转换整数及移位计数后按字符串执行指定类型的移位运算, 计数遵循目标类型的原生规则.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">待移位的源对象, 转换为 T; null 按零处理.</param>
    /// <param name="value2">移位计数, 转换为 int; null 按零处理.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数或移位计数不能转换为所需类型.</exception>
    /// <exception cref="FormatException">操作数或移位计数的文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或移位结果超出目标类型支持的范围.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static T BitwiseShift<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return BitwiseShift<T>(value1, value2, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 转换整数及移位计数后按字符串执行指定运行时类型的移位运算, 计数遵循目标类型的原生规则.
    /// </summary>
    /// <param name="value1">待移位的源对象, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">移位计数, 转换为 int; null 按零处理.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数或移位计数不能转换为所需类型.</exception>
    /// <exception cref="FormatException">操作数或移位计数的文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或移位结果超出目标类型支持的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        Type numberType,
        string @operator
    )
    {
        return BitwiseShift(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 转换整数及移位计数后按字符串执行枚举指定类型的移位运算, 计数遵循目标类型的原生规则.
    /// </summary>
    /// <param name="value1">待移位的源对象, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">移位计数, 转换为 int; null 按零处理.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数或移位计数不能转换为所需类型.</exception>
    /// <exception cref="FormatException">操作数或移位计数的文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或移位结果超出目标类型支持的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return BitwiseShift(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 转换整数及移位计数后执行指定类型的移位运算; 小整数先按 32 位规则移位再截断.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">待移位的源对象, 转换为 T; null 按零处理.</param>
    /// <param name="value2">转换为 int 的移位计数, 按原生规则处理, 不额外限制正负或范围; null 按零处理.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数或移位计数不能转换为所需类型.</exception>
    /// <exception cref="FormatException">操作数或移位计数的文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或移位结果超出目标类型支持的范围.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseShift<T>(object value1, object value2, BitwiseShiftType operatorType)
        where T : struct, INumber<T>
    {
        return (T)BitwiseShift(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 转换整数及移位计数后执行指定运行时类型的移位运算; 小整数先按 32 位规则移位再截断.
    /// </summary>
    /// <param name="value1">待移位的源对象, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="value2">转换为 int 的移位计数, 按原生规则处理, 不额外限制正负或范围; null 按零处理.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数或移位计数不能转换为所需类型.</exception>
    /// <exception cref="FormatException">操作数或移位计数的文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或移位结果超出目标类型支持的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        Type numberType,
        BitwiseShiftType operatorType
    )
    {
        return BitwiseShift(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 转换后执行移位运算; 小整数先按 32 位规则移位再截断, 其他定宽整数使用自身位宽, BigInteger 不套用固定宽度掩码.
    /// </summary>
    /// <param name="value1">待移位的源对象, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="value2">按相同规则转换为 int 的移位计数, 不额外限制正负或范围; null 按零处理.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <returns>已装箱且保持目标整数类型的移位结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        var shift = ConvertNumber<int>(value2);
        return numberType switch
        {
            NumberType.SByte => ShiftCore(ConvertNumber<sbyte>(value1), shift, operatorType),
            NumberType.Byte => ShiftCore(ConvertNumber<byte>(value1), shift, operatorType),
            NumberType.Int16 => ShiftCore(ConvertNumber<short>(value1), shift, operatorType),
            NumberType.UInt16 => ShiftCore(ConvertNumber<ushort>(value1), shift, operatorType),
            NumberType.Int32 => ShiftCore(ConvertNumber<int>(value1), shift, operatorType),
            NumberType.UInt32 => ShiftCore(ConvertNumber<uint>(value1), shift, operatorType),
            NumberType.Int64 => ShiftCore(ConvertNumber<long>(value1), shift, operatorType),
            NumberType.UInt64 => ShiftCore(ConvertNumber<ulong>(value1), shift, operatorType),
            NumberType.IntPtr => ShiftCore(ConvertNumber<nint>(value1), shift, operatorType),
            NumberType.UIntPtr => ShiftCore(ConvertNumber<nuint>(value1), shift, operatorType),
            NumberType.Int128 => ShiftCore(ConvertNumber<Int128>(value1), shift, operatorType),
            NumberType.UInt128 => ShiftCore(ConvertNumber<UInt128>(value1), shift, operatorType),
            NumberType.BigInteger => ShiftCore(
                ConvertNumber<BigInteger>(value1),
                shift,
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
