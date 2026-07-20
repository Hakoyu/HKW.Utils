using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareByF<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareByF<T>(value1, value2, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareByF(object value1, object value2, Type numberType, string @operator)
    {
        return CompareByF(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 比较 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
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
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static bool CompareByF(
#pragma warning restore S3776
        object value1,
        object value2,
        NumberType numberType,
        ComparisonOperatorType operatorType
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
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Byte:
            {
                var left = (byte)value1;
                var right = (byte)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int16:
            {
                var left = (short)value1;
                var right = (short)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt16:
            {
                var left = (ushort)value1;
                var right = (ushort)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int32:
            {
                var left = (int)value1;
                var right = (int)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt32:
            {
                var left = (uint)value1;
                var right = (uint)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int64:
            {
                var left = (long)value1;
                var right = (long)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt64:
            {
                var left = (ulong)value1;
                var right = (ulong)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
#pragma warning disable S1244
            case NumberType.Single:
            {
                var left = (float)value1;
                var right = (float)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Double:
            {
                var left = (double)value1;
                var right = (double)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
#pragma warning restore S1244
            case NumberType.Decimal:
            {
                var left = (decimal)value1;
                var right = (decimal)value2;
                return operatorType switch
                {
                    ComparisonOperatorType.Equality => left == right,
                    ComparisonOperatorType.Inequality => left != right,
                    ComparisonOperatorType.LessThan => left < right,
                    ComparisonOperatorType.GreaterThan => left > right,
                    ComparisonOperatorType.LessThanOrEqual => left <= right,
                    ComparisonOperatorType.GreaterThanOrEqual => left >= right,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }
}
