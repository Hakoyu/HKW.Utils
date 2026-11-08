using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱为指定类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareByF<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareByF<T>(value1, value2, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 精确拆箱为指定运行时类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareByF(object value1, object value2, Type numberType, string @operator)
    {
        return CompareByF(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 精确拆箱为枚举指定的类型后按字符串执行关系比较, 不通过 CompareTo 推导结果.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operator">完整的关系运算符字符串, 不忽略空白.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operator"/> 为 <see langword="null"/>.</exception>
    public static bool CompareByF(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return CompareByF(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 精确拆箱为指定类型后执行关系比较; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    public static bool CompareByF<T>(
        object value1,
        object value2,
        ComparisonOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return CompareByF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 精确拆箱为指定运行时类型后执行关系比较; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static bool CompareByF(
        object value1,
        object value2,
        Type numberType,
        ComparisonOperatorType operatorType
    )
    {
        return CompareByF(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 精确拆箱后直接使用关系运算符比较, 不通过 CompareTo 推导结果; 浮点 NaN 参与比较时仅不等关系成立.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operatorType">关系运算符枚举值.</param>
    /// <returns>指定关系成立时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    public static bool CompareByF(
        object value1,
        object value2,
        NumberType numberType,
        ComparisonOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => CompareByCore((sbyte)value1, (sbyte)value2, operatorType),
            NumberType.Byte => CompareByCore((byte)value1, (byte)value2, operatorType),
            NumberType.Int16 => CompareByCore((short)value1, (short)value2, operatorType),
            NumberType.UInt16 => CompareByCore((ushort)value1, (ushort)value2, operatorType),
            NumberType.Int32 => CompareByCore((int)value1, (int)value2, operatorType),
            NumberType.UInt32 => CompareByCore((uint)value1, (uint)value2, operatorType),
            NumberType.Int64 => CompareByCore((long)value1, (long)value2, operatorType),
            NumberType.UInt64 => CompareByCore((ulong)value1, (ulong)value2, operatorType),
            NumberType.Single => CompareByCore((float)value1, (float)value2, operatorType),
            NumberType.Double => CompareByCore((double)value1, (double)value2, operatorType),
            NumberType.Decimal => CompareByCore((decimal)value1, (decimal)value2, operatorType),
            NumberType.IntPtr => CompareByCore((nint)value1, (nint)value2, operatorType),
            NumberType.UIntPtr => CompareByCore((nuint)value1, (nuint)value2, operatorType),
            NumberType.Int128 => CompareByCore((Int128)value1, (Int128)value2, operatorType),
            NumberType.UInt128 => CompareByCore((UInt128)value1, (UInt128)value2, operatorType),
            NumberType.Half => CompareByCore((Half)value1, (Half)value2, operatorType),
            NumberType.BigInteger => CompareByCore(
                (BigInteger)value1,
                (BigInteger)value2,
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
