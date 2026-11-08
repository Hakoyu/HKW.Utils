using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱为指定类型后按字符执行算术运算, 不进行数值转换, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 <typeparamref name="T"/> 完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 <typeparamref name="T"/> 完全一致.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static T ArithmeticF<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return ArithmeticF<T>(value1, value2, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为指定运行时类型后按字符执行算术运算, 不进行数值转换, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object ArithmeticF(object value1, object value2, Type numberType, char @operator)
    {
        return ArithmeticF(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为枚举指定的类型后按字符执行算术运算, 不进行数值转换, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object ArithmeticF(
        object value1,
        object value2,
        NumberType numberType,
        char @operator
    )
    {
        return ArithmeticF(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 精确拆箱为指定类型后执行算术运算, 不进行数值转换, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 <typeparamref name="T"/> 完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 <typeparamref name="T"/> 完全一致.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T ArithmeticF<T>(
        object value1,
        object value2,
        ArithmeticOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return (T)ArithmeticF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 精确拆箱为指定运行时类型后执行算术运算, 不进行数值转换, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object ArithmeticF(
        object value1,
        object value2,
        Type numberType,
        ArithmeticOperatorType operatorType
    )
    {
        return ArithmeticF(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 精确拆箱后执行算术运算, 不进行数值转换. 运算采用原生 unchecked 规则, 小整数先提升为 int 再截断回目标位宽.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    /// <exception cref="OverflowException">目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">目标类型的除法或余数运算不允许除数为零.</exception>
    public static object ArithmeticF(
        object value1,
        object value2,
        NumberType numberType,
        ArithmeticOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)ArithmeticCore((sbyte)value1, (sbyte)value2, operatorType),
            NumberType.Byte => (object)ArithmeticCore((byte)value1, (byte)value2, operatorType),
            NumberType.Int16 => (object)ArithmeticCore((short)value1, (short)value2, operatorType),
            NumberType.UInt16 => (object)ArithmeticCore(
                (ushort)value1,
                (ushort)value2,
                operatorType
            ),
            NumberType.Int32 => (object)ArithmeticCore((int)value1, (int)value2, operatorType),
            NumberType.UInt32 => (object)ArithmeticCore((uint)value1, (uint)value2, operatorType),
            NumberType.Int64 => (object)ArithmeticCore((long)value1, (long)value2, operatorType),
            NumberType.UInt64 => (object)ArithmeticCore((ulong)value1, (ulong)value2, operatorType),
            NumberType.Single => (object)ArithmeticCore((float)value1, (float)value2, operatorType),
            NumberType.Double => (object)ArithmeticCore(
                (double)value1,
                (double)value2,
                operatorType
            ),
            NumberType.Decimal => (object)ArithmeticCore(
                (decimal)value1,
                (decimal)value2,
                operatorType
            ),
            NumberType.IntPtr => (object)ArithmeticCore((nint)value1, (nint)value2, operatorType),
            NumberType.UIntPtr => (object)ArithmeticCore(
                (nuint)value1,
                (nuint)value2,
                operatorType
            ),
            NumberType.Int128 => (object)ArithmeticCore(
                (Int128)value1,
                (Int128)value2,
                operatorType
            ),
            NumberType.UInt128 => (object)ArithmeticCore(
                (UInt128)value1,
                (UInt128)value2,
                operatorType
            ),
            NumberType.Half => (object)ArithmeticCore((Half)value1, (Half)value2, operatorType),
            NumberType.BigInteger => (object)ArithmeticCore(
                (BigInteger)value1,
                (BigInteger)value2,
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
