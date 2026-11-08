using System.Numerics;

namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱整数及 int 计数后按字符串执行指定类型的移位运算, 不进行数值转换.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">待移位的已装箱整数, 类型必须与 T 完全一致.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 遵循目标类型的原生移位规则.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">整数或移位计数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">整数或移位计数为 null.</exception>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static T BitwiseShiftF<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return BitwiseShiftF<T>(value1, value2, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 精确拆箱整数及 int 计数后按字符串执行指定运行时类型的移位运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">待移位的已装箱整数, 类型必须与目标类型完全一致.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 遵循目标类型的原生移位规则.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">整数或移位计数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">整数或移位计数为 null.</exception>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        Type numberType,
        string @operator
    )
    {
        return BitwiseShiftF(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 精确拆箱整数及 int 计数后按字符串执行枚举指定类型的移位运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">待移位的已装箱整数, 类型必须与目标类型完全一致.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 遵循目标类型的原生移位规则.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">完整的移位运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">整数或移位计数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">整数或移位计数为 null.</exception>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return BitwiseShiftF(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 精确拆箱整数及 int 计数后执行指定类型的移位运算; 小整数先按 32 位规则移位再截断.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">待移位的已装箱整数, 类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 按原生规则处理, 不额外限制正负或范围.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">整数或移位计数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">整数或移位计数为 null.</exception>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseShiftF<T>(object value1, object value2, BitwiseShiftType operatorType)
        where T : struct, INumber<T>
    {
        return (T)BitwiseShiftF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 精确拆箱整数及 int 计数后执行指定运行时类型的移位运算; 小整数先按 32 位规则移位再截断.
    /// </summary>
    /// <param name="value1">待移位的已装箱整数, 类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 按原生规则处理, 不额外限制正负或范围.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">整数或移位计数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">整数或移位计数为 null.</exception>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        Type numberType,
        BitwiseShiftType operatorType
    )
    {
        return BitwiseShiftF(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 精确拆箱后执行移位运算; 小整数先按 32 位规则移位再截断, 其他定宽整数使用自身位宽, BigInteger 不套用固定宽度掩码.
    /// </summary>
    /// <param name="value1">待移位的已装箱整数, 类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">必须为已装箱 int 的移位计数, 按原生规则处理, 不额外限制正负或范围.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">左移, 右移或无符号右移运算符枚举值.</param>
    /// <returns>已装箱且保持目标整数类型的移位结果.</returns>
    /// <exception cref="OverflowException">BigInteger 移位结果超出其支持的大小.</exception>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        var shift = (int)value2;
        if (
            operatorType
            is not (
                BitwiseShiftType.Left
                or BitwiseShiftType.Right
                or BitwiseShiftType.UnsignedRight
            )
        )
            throw CreateUnsupportedOperatorException(operatorType);
        return numberType switch
        {
            NumberType.SByte => ShiftCore((sbyte)value1, shift, operatorType),
            NumberType.Byte => ShiftCore((byte)value1, shift, operatorType),
            NumberType.Int16 => ShiftCore((short)value1, shift, operatorType),
            NumberType.UInt16 => ShiftCore((ushort)value1, shift, operatorType),
            NumberType.Int32 => ShiftCore((int)value1, shift, operatorType),
            NumberType.UInt32 => ShiftCore((uint)value1, shift, operatorType),
            NumberType.Int64 => ShiftCore((long)value1, shift, operatorType),
            NumberType.UInt64 => ShiftCore((ulong)value1, shift, operatorType),
            NumberType.IntPtr => ShiftCore((nint)value1, shift, operatorType),
            NumberType.UIntPtr => ShiftCore((nuint)value1, shift, operatorType),
            NumberType.Int128 => ShiftCore((Int128)value1, shift, operatorType),
            NumberType.UInt128 => ShiftCore((UInt128)value1, shift, operatorType),
            NumberType.BigInteger => ShiftCore((BigInteger)value1, shift, operatorType),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
