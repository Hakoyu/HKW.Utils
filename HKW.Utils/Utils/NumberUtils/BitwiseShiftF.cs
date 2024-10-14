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
    /// 位移运算 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseShiftF<T>(object? value1, object? value2, string @operator)
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
        object? value1,
        object? value2,
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
        object? value1,
        object? value2,
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
        object? value1,
        object? value2,
        BitwiseShiftType operatorType
    )
        where T : struct, INumber<T>
    {
        var type = typeof(T);
        if (operatorType is BitwiseShiftType.Left)
        {
            if (type == typeof(sbyte))
                return (Int32)value1! << (Int32)value2!;
            else if (type == typeof(byte))
                return (Int32)value1! << (Int32)value2!;
            else if (type == typeof(short))
                return (Int32)value1! << (Int32)value2!;
            else if (type == typeof(ushort))
                return (UInt32)value1! << (Int32)value2!;
            else if (type == typeof(int))
                return (Int32)value1! << (Int32)value2!;
            else if (type == typeof(uint))
                return (UInt32)value1! << (Int32)value2!;
            else if (type == typeof(long))
                return (Int64)value1! << (Int32)value2!;
            else if (type == typeof(ulong))
                return (UInt64)value1! << (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (type == typeof(sbyte))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(byte))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(short))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(ushort))
                return (UInt32)value1! >> (Int32)value2!;
            else if (type == typeof(int))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(uint))
                return (UInt32)value1! >> (Int32)value2!;
            else if (type == typeof(long))
                return (Int64)value1! >> (Int32)value2!;
            else if (type == typeof(ulong))
                return (UInt64)value1! >> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (type == typeof(sbyte))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(byte))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(short))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(ushort))
                return (UInt32)value1! >> (Int32)value2!;
            else if (type == typeof(int))
                return (Int32)value1! >> (Int32)value2!;
            else if (type == typeof(uint))
                return (UInt32)value1! >> (Int32)value2!;
            else if (type == typeof(long))
                return (Int64)value1! >> (Int32)value2!;
            else if (type == typeof(ulong))
                return (UInt64)value1! >> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
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
        object? value1,
        object? value2,
        Type numberType,
        BitwiseShiftType operatorType
    )
    {
        if (operatorType is BitwiseShiftType.Left)
        {
            if (numberType == typeof(sbyte))
                return (Int32)value1! << (Int32)value2!;
            else if (numberType == typeof(byte))
                return (Int32)value1! << (Int32)value2!;
            else if (numberType == typeof(short))
                return (Int32)value1! << (Int32)value2!;
            else if (numberType == typeof(ushort))
                return (UInt32)value1! << (Int32)value2!;
            else if (numberType == typeof(int))
                return (Int32)value1! << (Int32)value2!;
            else if (numberType == typeof(uint))
                return (UInt32)value1! << (Int32)value2!;
            else if (numberType == typeof(long))
                return (Int64)value1! << (Int32)value2!;
            else if (numberType == typeof(ulong))
                return (UInt64)value1! << (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (numberType == typeof(sbyte))
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType == typeof(byte))
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType == typeof(short))
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType == typeof(ushort))
                return (UInt32)value1! >> (Int32)value2!;
            else if (numberType == typeof(int))
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType == typeof(uint))
                return (UInt32)value1! >> (Int32)value2!;
            else if (numberType == typeof(long))
                return (Int64)value1! >> (Int32)value2!;
            else if (numberType == typeof(ulong))
                return (UInt64)value1! >> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (numberType == typeof(sbyte))
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(byte))
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(short))
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(ushort))
                return (UInt32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(int))
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(uint))
                return (UInt32)value1! >>> (Int32)value2!;
            else if (numberType == typeof(long))
                return (Int64)value1! >>> (Int32)value2!;
            else if (numberType == typeof(ulong))
                return (UInt64)value1! >>> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
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
        object? value1,
        object? value2,
        NumberType numberType,
        BitwiseShiftType operatorType
    )
    {
        if (operatorType is BitwiseShiftType.Left)
        {
            if (numberType is NumberType.SByte)
                return (Int32)value1! << (Int32)value2!;
            else if (numberType is NumberType.Byte)
                return (Int32)value1! << (Int32)value2!;
            else if (numberType is NumberType.Int16)
                return (Int32)value1! << (Int32)value2!;
            else if (numberType is NumberType.UInt16)
                return (UInt32)value1! << (Int32)value2!;
            else if (numberType is NumberType.Int32)
                return (Int32)value1! << (Int32)value2!;
            else if (numberType is NumberType.UInt32)
                return (UInt32)value1! << (Int32)value2!;
            else if (numberType is NumberType.Int64)
                return (Int64)value1! << (Int32)value2!;
            else if (numberType is NumberType.UInt64)
                return (UInt64)value1! << (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.Right)
        {
            if (numberType is NumberType.SByte)
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.Byte)
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.Int16)
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.UInt16)
                return (UInt32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.Int32)
                return (Int32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.UInt32)
                return (UInt32)value1! >> (Int32)value2!;
            else if (numberType is NumberType.Int64)
                return (Int64)value1! >> (Int32)value2!;
            else if (numberType is NumberType.UInt64)
                return (UInt64)value1! >> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseShiftType.UnsignedRight)
        {
            if (numberType is NumberType.SByte)
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.Byte)
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.Int16)
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.UInt16)
                return (UInt32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.Int32)
                return (Int32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.UInt32)
                return (UInt32)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.Int64)
                return (Int64)value1! >>> (Int32)value2!;
            else if (numberType is NumberType.UInt64)
                return (UInt64)value1! >>> (Int32)value2!;
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
