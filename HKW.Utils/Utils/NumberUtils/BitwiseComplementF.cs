using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF<T>(object? value)
    {
        var type = typeof(T);
        if (type == typeof(sbyte))
            return ~(SByte)value!;
        else if (type == typeof(byte))
            return ~(Byte)value!;
        else if (type == typeof(short))
            return ~(Int16)value!;
        else if (type == typeof(ushort))
            return ~(UInt16)value!;
        else if (type == typeof(int))
            return ~(Int32)value!;
        else if (type == typeof(uint))
            return ~(UInt32)value!;
        else if (type == typeof(long))
            return ~(Int64)value!;
        else if (type == typeof(ulong))
            return ~(UInt64)value!;
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF(object? value, Type numberType)
    {
        if (numberType == typeof(sbyte))
            return ~(SByte)value!;
        else if (numberType == typeof(byte))
            return ~(Byte)value!;
        else if (numberType == typeof(short))
            return ~(Int16)value!;
        else if (numberType == typeof(ushort))
            return ~(UInt16)value!;
        else if (numberType == typeof(int))
            return ~(Int32)value!;
        else if (numberType == typeof(uint))
            return ~(UInt32)value!;
        else if (numberType == typeof(long))
            return ~(Int64)value!;
        else if (numberType == typeof(ulong))
            return ~(UInt64)value!;
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF(object? value, NumberType numberType)
    {
        if (numberType is NumberType.SByte)
            return ~(SByte)value!;
        else if (numberType is NumberType.Byte)
            return ~(Byte)value!;
        else if (numberType is NumberType.Int16)
            return ~(Int16)value!;
        else if (numberType is NumberType.UInt16)
            return ~(UInt16)value!;
        else if (numberType is NumberType.Int32)
            return ~(Int32)value!;
        else if (numberType is NumberType.UInt32)
            return ~(UInt32)value!;
        else if (numberType is NumberType.Int64)
            return ~(Int64)value!;
        else if (numberType is NumberType.UInt64)
            return ~(UInt64)value!;
        else
            throw new NotImplementedException();
    }
}
