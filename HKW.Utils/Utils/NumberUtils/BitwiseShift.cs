using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 位运算位移运算符类型
    /// <para>(OperatorString, BitwiseShiftType)</para>
    /// </summary>
    public static FrozenBidirectionalDictionary<
        string,
        BitwiseShiftType
    > BitwiseShiftTypeByString { get; } =
        new([
            KeyValuePair.Create("<<", BitwiseShiftType.Left),
            KeyValuePair.Create(">>", BitwiseShiftType.Right),
            KeyValuePair.Create(">>>", BitwiseShiftType.UnsignedRight),
        ]);

    /// <summary>
    /// 获取位运算位移符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <returns>运算符</returns>
    public static BitwiseShiftType GetBitwiseShiftType(string @operator)
    {
        return BitwiseShiftTypeByString[@operator];
    }

    /// <summary>
    /// 获取位运算位移符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符</returns>
    public static bool TryGetBitwiseShiftType(string @operator, out BitwiseShiftType operatorType)
    {
        return BitwiseShiftTypeByString.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShift<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return BitwiseShift<T>(value1, value2, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        Type numberType,
        string @operator
    )
    {
        return BitwiseShift(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return BitwiseShift(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShift<T>(
        object value1,
        object value2,
        BitwiseShiftType operatorType
    )
        where T : struct, INumber<T>
    {
        return BitwiseShift(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShift(
        object value1,
        object value2,
        Type numberType,
        BitwiseShiftType operatorType
    )
    {
        return BitwiseShift(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 位移运算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
#pragma warning disable S3776
    public static object BitwiseShift(
#pragma warning restore S3776
        object value1,
        object value2,
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        var shift = Convert.ToInt32(value2);
        switch (numberType)
        {
            case NumberType.SByte:
            {
                var left = Convert.ToInt32(Convert.ToSByte(value1));
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Byte:
            {
                var left = Convert.ToInt32(Convert.ToByte(value1));
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int16:
            {
                var left = Convert.ToInt32(Convert.ToInt16(value1));
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt16:
            {
                var left = Convert.ToUInt32(value1);
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int32:
            {
                var left = Convert.ToInt32(value1);
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt32:
            {
                var left = Convert.ToUInt32(value1);
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.Int64:
            {
                var left = Convert.ToInt64(value1);
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            case NumberType.UInt64:
            {
                var left = Convert.ToUInt64(value1);
                return operatorType switch
                {
                    BitwiseShiftType.Left => left << shift,
                    BitwiseShiftType.Right => left >> shift,
                    BitwiseShiftType.UnsignedRight => left >>> shift,
                    _ => throw CreateUnsupportedOperatorException(operatorType),
                };
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }
}
