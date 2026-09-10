using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 计算运算符类型
    /// <para>(OperatorChar, ArithmeticOperatorType)</para>
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
    /// 获取计算运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <returns>运算符</returns>
    public static ArithmeticOperatorType GetArithmeticOperatorType(char @operator)
    {
        return ArithmeticOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 获取计算运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符</returns>
    public static bool TryGetArithmeticOperatorType(
        char @operator,
        out ArithmeticOperatorType operatorType
    )
    {
        return ArithmeticOperatorTypeByChar.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static T Arithmetic<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return Arithmetic<T>(value1, value2, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Arithmetic(object value1, object value2, Type numberType, char @operator)
    {
        return Arithmetic(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static T Arithmetic<T>(object value1, object value2, ArithmeticOperatorType operatorType)
        where T : struct, INumber<T>
    {
        return (T)Arithmetic(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static object Arithmetic(
#pragma warning restore S3776
        object value1,
        object value2,
        NumberType numberType,
        ArithmeticOperatorType operatorType
    )
    {
        switch (numberType)
        {
            case NumberType.SByte:
            {
                var left = Convert.ToSByte(value1);
                var right = Convert.ToSByte(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Byte:
            {
                var left = Convert.ToByte(value1);
                var right = Convert.ToByte(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int16:
            {
                var left = Convert.ToInt16(value1);
                var right = Convert.ToInt16(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt16:
            {
                var left = Convert.ToUInt16(value1);
                var right = Convert.ToUInt16(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int32:
            {
                var left = Convert.ToInt32(value1);
                var right = Convert.ToInt32(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt32:
            {
                var left = Convert.ToUInt32(value1);
                var right = Convert.ToUInt32(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int64:
            {
                var left = Convert.ToInt64(value1);
                var right = Convert.ToInt64(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt64:
            {
                var left = Convert.ToUInt64(value1);
                var right = Convert.ToUInt64(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Single:
            {
                var left = Convert.ToSingle(value1);
                var right = Convert.ToSingle(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Double:
            {
                var left = Convert.ToDouble(value1);
                var right = Convert.ToDouble(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Decimal:
            {
                var left = Convert.ToDecimal(value1);
                var right = Convert.ToDecimal(value2);
                return operatorType switch
                {
                    ArithmeticOperatorType.Addition => left + right,
                    ArithmeticOperatorType.Subtraction => left - right,
                    ArithmeticOperatorType.Multiply => left * right,
                    ArithmeticOperatorType.Division => left / right,
                    ArithmeticOperatorType.Modulus => left % right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }
}
