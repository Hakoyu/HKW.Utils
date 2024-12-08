using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        FrozenBidirectionalDictionary.Create<string, BitwiseShiftType>(
            [
                ("<<", BitwiseShiftType.Left),
                (">>", BitwiseShiftType.Right),
                (">>>", BitwiseShiftType.UnsignedRight),
            ]
        );

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
        var type = typeof(T);
        if (operatorType is BitwiseShiftType.Left)
        {
            if (type == typeof(sbyte))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(byte))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(short))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) << Convert.ToInt32(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) << Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (type == typeof(sbyte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(byte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(short))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) >> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (type == typeof(sbyte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(byte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(short))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) >> Convert.ToInt32(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) >> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
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
        if (operatorType is BitwiseShiftType.Left)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(byte))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) << Convert.ToInt32(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) << Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(byte))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) >> Convert.ToInt32(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) >> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(byte))
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) >>> Convert.ToInt32(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) >>> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
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
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        if (operatorType is BitwiseShiftType.Left)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) << Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) << Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) >> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) >> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) >>> Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) >>> Convert.ToInt32(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
