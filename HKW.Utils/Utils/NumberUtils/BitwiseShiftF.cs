using System.Numerics;

namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return BitwiseShiftF<T>(value1, value2, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        Type numberType,
        string @operator
    )
    {
        return BitwiseShiftF(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return BitwiseShiftF(value1, value2, numberType, GetBitwiseShiftType(@operator));
    }

    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF<T>(
        object value1,
        object value2,
        BitwiseShiftType operatorType
    )
        where T : struct, INumber<T>
    {
        return BitwiseShiftF(value1, value2, typeof(T), operatorType);
    }

    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        Type numberType,
        BitwiseShiftType operatorType
    )
    {
        return BitwiseShiftF(value1, value2, GetNumberType(numberType), operatorType);
    }

    /// <summary>
    /// 位移运算 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        var shift = (int)value2;
        switch (operatorType)
        {
            case BitwiseShiftType.Left:
                return numberType switch
                {
                    NumberType.SByte => (int)(sbyte)value1 << shift,
                    NumberType.Byte => (int)(byte)value1 << shift,
                    NumberType.Int16 => (int)(short)value1 << shift,
                    NumberType.UInt16 => (uint)(ushort)value1 << shift,
                    NumberType.Int32 => (int)value1 << shift,
                    NumberType.UInt32 => (uint)value1 << shift,
                    NumberType.Int64 => (long)value1 << shift,
                    NumberType.UInt64 => (ulong)value1 << shift,
                    _ => throw CreateUnsupportedNumberTypeException(numberType),
                };
            case BitwiseShiftType.Right:
                return numberType switch
                {
                    NumberType.SByte => (int)(sbyte)value1 >> shift,
                    NumberType.Byte => (int)(byte)value1 >> shift,
                    NumberType.Int16 => (int)(short)value1 >> shift,
                    NumberType.UInt16 => (uint)(ushort)value1 >> shift,
                    NumberType.Int32 => (int)value1 >> shift,
                    NumberType.UInt32 => (uint)value1 >> shift,
                    NumberType.Int64 => (long)value1 >> shift,
                    NumberType.UInt64 => (ulong)value1 >> shift,
                    _ => throw CreateUnsupportedNumberTypeException(numberType),
                };
            case BitwiseShiftType.UnsignedRight:
                return numberType switch
                {
                    NumberType.SByte => (int)(sbyte)value1 >>> shift,
                    NumberType.Byte => (int)(byte)value1 >>> shift,
                    NumberType.Int16 => (int)(short)value1 >>> shift,
                    NumberType.UInt16 => (uint)(ushort)value1 >>> shift,
                    NumberType.Int32 => (int)value1 >>> shift,
                    NumberType.UInt32 => (uint)value1 >>> shift,
                    NumberType.Int64 => (long)value1 >>> shift,
                    NumberType.UInt64 => (ulong)value1 >>> shift,
                    _ => throw CreateUnsupportedNumberTypeException(numberType),
                };
            default:
                throw CreateUnsupportedOperatorException(operatorType);
        }
    }
}
