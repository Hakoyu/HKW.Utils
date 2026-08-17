using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 比较运算符类型
    /// <para>(OperatorChar, ComparisonOperatorType)</para>
    /// </summary>
    public static FrozenBidirectionalDictionary<
        string,
        ComparisonOperatorType
    > ComparisonOperatorTypeByString { get; } =
        new([
            KeyValuePair.Create("==", ComparisonOperatorType.Equality),
            KeyValuePair.Create("!=", ComparisonOperatorType.Inequality),
            KeyValuePair.Create("<", ComparisonOperatorType.LessThan),
            KeyValuePair.Create(">", ComparisonOperatorType.GreaterThan),
            KeyValuePair.Create("<=", ComparisonOperatorType.LessThanOrEqual),
            KeyValuePair.Create(">=", ComparisonOperatorType.GreaterThanOrEqual),
        ]);

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">比较运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareBy<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareBy<T>(value1, value2, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">比较运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareBy(object value1, object value2, Type numberType, string @operator)
    {
        return CompareBy(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">比较运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareBy(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return CompareBy(value1, value2, numberType, ComparisonOperatorTypeByString[@operator]);
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">比较运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareBy<T>(
        object value1,
        object value2,
        ComparisonOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        return CompareBy(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <param name="operatorType">比较运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareBy(
        object value1,
        object value2,
        Type numberType,
        ComparisonOperatorType operatorType
    )
    {
        return CompareBy(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <param name="operatorType">比较运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static bool CompareBy(
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
                var left = Convert.ToSByte(value1);
                var right = Convert.ToSByte(value2);
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
                var left = Convert.ToByte(value1);
                var right = Convert.ToByte(value2);
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
                var left = Convert.ToInt16(value1);
                var right = Convert.ToInt16(value2);
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
                var left = Convert.ToUInt16(value1);
                var right = Convert.ToUInt16(value2);
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
                var left = Convert.ToInt32(value1);
                var right = Convert.ToInt32(value2);
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
                var left = Convert.ToUInt32(value1);
                var right = Convert.ToUInt32(value2);
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
                var left = Convert.ToInt64(value1);
                var right = Convert.ToInt64(value2);
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
                var left = Convert.ToUInt64(value1);
                var right = Convert.ToUInt64(value2);
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
                var left = Convert.ToSingle(value1);
                var right = Convert.ToSingle(value2);
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
                var left = Convert.ToDouble(value1);
                var right = Convert.ToDouble(value2);
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
                var left = Convert.ToDecimal(value1);
                var right = Convert.ToDecimal(value2);
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
