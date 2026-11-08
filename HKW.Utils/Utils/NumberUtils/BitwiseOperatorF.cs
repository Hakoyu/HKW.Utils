using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱为指定整数类型后按字符执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 T 完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 T 完全一致.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static T BitwiseOperatorF<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return BitwiseOperatorF<T>(value1, value2, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为指定运行时整数类型后按字符执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object BitwiseOperatorF(
        object value1,
        object value2,
        Type numberType,
        char @operator
    )
    {
        return BitwiseOperatorF(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为枚举指定的整数类型后按字符执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operator">按位运算符字符, 支持 &amp;, | 和 ^.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object BitwiseOperatorF(
        object value1,
        object value2,
        NumberType numberType,
        char @operator
    )
    {
        return BitwiseOperatorF(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为指定整数类型后执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 T 完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 T 完全一致.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseOperatorF<T>(
        object value1,
        object value2,
        BitwiseOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return (T)BitwiseOperatorF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 精确拆箱为指定运行时整数类型后执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <exception cref="NotImplementedException">目标整数类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseOperatorF(
        object value1,
        object value2,
        Type numberType,
        BitwiseOperatorType operatorType
    )
    {
        return BitwiseOperatorF(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 精确拆箱整数操作数后执行按位与, 或或异或运算, 不进行数值转换.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <param name="operatorType">按位运算符枚举值; LogicalOr 表示异或.</param>
    /// <returns>已装箱且保持目标整数类型的运算结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    public static object BitwiseOperatorF(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => BitwiseCore((sbyte)value1, (sbyte)value2, operatorType),
            NumberType.Byte => BitwiseCore((byte)value1, (byte)value2, operatorType),
            NumberType.Int16 => BitwiseCore((short)value1, (short)value2, operatorType),
            NumberType.UInt16 => BitwiseCore((ushort)value1, (ushort)value2, operatorType),
            NumberType.Int32 => BitwiseCore((int)value1, (int)value2, operatorType),
            NumberType.UInt32 => BitwiseCore((uint)value1, (uint)value2, operatorType),
            NumberType.Int64 => BitwiseCore((long)value1, (long)value2, operatorType),
            NumberType.UInt64 => BitwiseCore((ulong)value1, (ulong)value2, operatorType),
            NumberType.IntPtr => BitwiseCore((nint)value1, (nint)value2, operatorType),
            NumberType.UIntPtr => BitwiseCore((nuint)value1, (nuint)value2, operatorType),
            NumberType.Int128 => BitwiseCore((Int128)value1, (Int128)value2, operatorType),
            NumberType.UInt128 => BitwiseCore((UInt128)value1, (UInt128)value2, operatorType),
            NumberType.BigInteger => BitwiseCore(
                (BigInteger)value1,
                (BigInteger)value2,
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
