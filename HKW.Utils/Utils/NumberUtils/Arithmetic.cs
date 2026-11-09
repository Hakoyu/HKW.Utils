using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 算术运算符字符与类型的双向映射.
    /// </summary>
    public static FrozenBidirectionalDictionary<
        char,
        ArithmeticOperatorType
    > ArithmeticOperatorTypeByChar { get; } =
        new([
            KeyValuePair.Create('+', ArithmeticOperatorType.Addition),
            KeyValuePair.Create('-', ArithmeticOperatorType.Subtraction),
            KeyValuePair.Create('*', ArithmeticOperatorType.Multiply),
            KeyValuePair.Create('/', ArithmeticOperatorType.Division),
            KeyValuePair.Create('%', ArithmeticOperatorType.Modulus),
        ]);

    /// <summary>
    /// 查找与完整符号对应的算术运算符枚举值.
    /// </summary>
    /// <param name="operator">要匹配的运算符字符.</param>
    /// <returns>与符号对应的运算符枚举值.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="operator"/> 不是受支持的完整运算符.</exception>
    public static ArithmeticOperatorType GetArithmeticOperatorType(char @operator)
    {
        return ArithmeticOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 尝试查找与完整符号对应的算术运算符枚举值.
    /// </summary>
    /// <param name="operator">要匹配的运算符字符.</param>
    /// <param name="operatorType">返回时包含匹配的枚举值; 未匹配时为枚举默认值.</param>
    /// <returns>找到运算符时为 <see langword="true"/>, 否则为 <see langword="false"/>.</returns>
    public static bool TryGetArithmeticOperatorType(
        char @operator,
        out ArithmeticOperatorType operatorType
    )
    {
        return ArithmeticOperatorTypeByChar.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 将操作数转换为指定类型后按字符执行算术运算, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static T Arithmetic<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return Arithmetic<T>(value1, value2, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 将操作数转换为指定运行时类型后按字符执行算术运算, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object Arithmetic(object value1, object value2, Type numberType, char @operator)
    {
        return Arithmetic(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 将操作数转换为枚举指定的类型后按字符执行算术运算, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operator">算术运算符字符, 支持 +, -, *, / 和 %.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="KeyNotFoundException">运算符符号不是受支持的完整运算符.</exception>
    public static object Arithmetic(
        object value1,
        object value2,
        NumberType numberType,
        char @operator
    )
    {
        return Arithmetic(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 将操作数转换为指定类型后执行算术运算, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T Arithmetic<T>(object value1, object value2, ArithmeticOperatorType operatorType)
        where T : struct, INumber<T>
    {
        return (T)Arithmetic(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 将操作数转换为指定运行时类型后执行算术运算, 运算采用原生 unchecked 规则.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换或目标类型的算术运算溢出.</exception>
    /// <exception cref="DivideByZeroException">整数或 decimal 的除法或余数运算的除数为零.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object Arithmetic(
        object value1,
        object value2,
        Type numberType,
        ArithmeticOperatorType operatorType
    )
    {
        return Arithmetic(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 转换操作数后执行算术运算, 运算采用原生 unchecked 规则; 小整数先提升为 int 再截断回目标位宽.
    /// </summary>
    /// <param name="value1">左操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="value2">右操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <param name="operatorType">算术运算符枚举值.</param>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型或运算符枚举值不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    /// <exception cref="DivideByZeroException">目标类型的除法或余数运算不允许除数为零.</exception>
    public static object Arithmetic(
        object value1,
        object value2,
        NumberType numberType,
        ArithmeticOperatorType operatorType
    )
    {
        return numberType switch
        {
            NumberType.SByte => (object)ArithmeticCore(
                ConvertNumber<sbyte>(value1),
                ConvertNumber<sbyte>(value2),
                operatorType
            ),
            NumberType.Byte => (object)ArithmeticCore(
                ConvertNumber<byte>(value1),
                ConvertNumber<byte>(value2),
                operatorType
            ),
            NumberType.Int16 => (object)ArithmeticCore(
                ConvertNumber<short>(value1),
                ConvertNumber<short>(value2),
                operatorType
            ),
            NumberType.UInt16 => (object)ArithmeticCore(
                ConvertNumber<ushort>(value1),
                ConvertNumber<ushort>(value2),
                operatorType
            ),
            NumberType.Int32 => (object)ArithmeticCore(
                ConvertNumber<int>(value1),
                ConvertNumber<int>(value2),
                operatorType
            ),
            NumberType.UInt32 => (object)ArithmeticCore(
                ConvertNumber<uint>(value1),
                ConvertNumber<uint>(value2),
                operatorType
            ),
            NumberType.Int64 => (object)ArithmeticCore(
                ConvertNumber<long>(value1),
                ConvertNumber<long>(value2),
                operatorType
            ),
            NumberType.UInt64 => (object)ArithmeticCore(
                ConvertNumber<ulong>(value1),
                ConvertNumber<ulong>(value2),
                operatorType
            ),
            NumberType.Single => (object)ArithmeticCore(
                ConvertNumber<float>(value1),
                ConvertNumber<float>(value2),
                operatorType
            ),
            NumberType.Double => (object)ArithmeticCore(
                ConvertNumber<double>(value1),
                ConvertNumber<double>(value2),
                operatorType
            ),
            NumberType.Decimal => (object)ArithmeticCore(
                ConvertNumber<decimal>(value1),
                ConvertNumber<decimal>(value2),
                operatorType
            ),
            NumberType.IntPtr => (object)ArithmeticCore(
                ConvertNumber<nint>(value1),
                ConvertNumber<nint>(value2),
                operatorType
            ),
            NumberType.UIntPtr => (object)ArithmeticCore(
                ConvertNumber<nuint>(value1),
                ConvertNumber<nuint>(value2),
                operatorType
            ),
            NumberType.Int128 => (object)ArithmeticCore(
                ConvertNumber<Int128>(value1),
                ConvertNumber<Int128>(value2),
                operatorType
            ),
            NumberType.UInt128 => (object)ArithmeticCore(
                ConvertNumber<UInt128>(value1),
                ConvertNumber<UInt128>(value2),
                operatorType
            ),
            NumberType.Half => (object)ArithmeticCore(
                ConvertNumber<Half>(value1),
                ConvertNumber<Half>(value2),
                operatorType
            ),
            NumberType.BigInteger => (object)ArithmeticCore(
                ConvertNumber<BigInteger>(value1),
                ConvertNumber<BigInteger>(value2),
                operatorType
            ),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
