using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 计算 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperatorF<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return BitwiseOperatorF<T>(value1, value2, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 计算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperatorF<T>(
        object value1,
        object value2,
        BitwiseOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return BitwiseOperatorF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 计算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 计算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static object BitwiseOperatorF(
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
                var left = (sbyte)value1;
                var right = (sbyte)value2;
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
                var left = (byte)value1;
                var right = (byte)value2;
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
                var left = (short)value1;
                var right = (short)value2;
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
                var left = (ushort)value1;
                var right = (ushort)value2;
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
                var left = (int)value1;
                var right = (int)value2;
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
                var left = (uint)value1;
                var right = (uint)value2;
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
                var left = (long)value1;
                var right = (long)value2;
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
                var left = (ulong)value1;
                var right = (ulong)value2;
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
