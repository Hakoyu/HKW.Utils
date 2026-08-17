using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 位运算符类型
    /// <para>(OperatorChar, BitwiseOperatorType)</para>
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
    /// 获取位运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <returns>运算符</returns>
    public static BitwiseOperatorType GetBitwiseOperatorType(char @operator)
    {
        return BitwiseOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 获取位运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符</returns>
    public static bool TryGetBitwiseOperatorType(
        char @operator,
        out BitwiseOperatorType operatorType
    )
    {
        return BitwiseOperatorTypeByChar.TryGetValue(@operator, out operatorType);
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
    public static object BitwiseOperator<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return BitwiseOperator<T>(value1, value2, GetBitwiseOperatorType(@operator));
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
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator<T>(
        object value1,
        object value2,
        BitwiseOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return BitwiseOperator(value1, value2, typeof(T), operatorType);
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
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static object BitwiseOperator(
#pragma warning restore S3776
        object value1,
        object value2,
        NumberType numberType,
        BitwiseOperatorType operatorType
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
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Byte:
            {
                var left = Convert.ToByte(value1);
                var right = Convert.ToByte(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int16:
            {
                var left = Convert.ToInt16(value1);
                var right = Convert.ToInt16(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt16:
            {
                var left = Convert.ToUInt16(value1);
                var right = Convert.ToUInt16(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int32:
            {
                var left = Convert.ToInt32(value1);
                var right = Convert.ToInt32(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt32:
            {
                var left = Convert.ToUInt32(value1);
                var right = Convert.ToUInt32(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int64:
            {
                var left = Convert.ToInt64(value1);
                var right = Convert.ToInt64(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt64:
            {
                var left = Convert.ToUInt64(value1);
                var right = Convert.ToUInt64(value2);
                return operatorType switch
                {
                    BitwiseOperatorType.Or => left | right,
                    BitwiseOperatorType.And => left & right,
                    BitwiseOperatorType.LogicalOr => left ^ right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }
}
