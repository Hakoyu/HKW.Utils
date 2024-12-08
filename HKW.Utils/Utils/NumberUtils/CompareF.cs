using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object CompareF<T>(object value1, object value2)
        where T : struct, INumber<T>
    {
        var type = typeof(T);

        if (type == typeof(sbyte))
            return ((SByte)value1).CompareTo((SByte)value2);
        else if (type == typeof(byte))
            return ((Byte)value1).CompareTo((Byte)value2);
        else if (type == typeof(short))
            return ((Int16)value1).CompareTo((Int16)value2);
        else if (type == typeof(ushort))
            return ((UInt16)value1).CompareTo((UInt16)value2);
        else if (type == typeof(int))
            return ((Int32)value1).CompareTo((Int32)value2);
        else if (type == typeof(uint))
            return ((UInt32)value1).CompareTo((UInt32)value2);
        else if (type == typeof(long))
            return ((Int64)value1).CompareTo((Int64)value2);
        else if (type == typeof(ulong))
            return ((UInt64)value1).CompareTo((UInt64)value2);
        else if (type == typeof(float))
            return ((Single)value1).CompareTo((Single)value2);
        else if (type == typeof(double))
            return ((Double)value1).CompareTo((Double)value2);
        else if (type == typeof(decimal))
            return ((Decimal)value1).CompareTo((Decimal)value2);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object CompareF(object value1, object value2, Type numberType)
    {
        if (numberType == typeof(sbyte))
            return ((SByte)value1).CompareTo((SByte)value2);
        else if (numberType == typeof(byte))
            return ((Byte)value1).CompareTo((Byte)value2);
        else if (numberType == typeof(short))
            return ((Int16)value1).CompareTo((Int16)value2);
        else if (numberType == typeof(ushort))
            return ((UInt16)value1).CompareTo((UInt16)value2);
        else if (numberType == typeof(int))
            return ((Int32)value1).CompareTo((Int32)value2);
        else if (numberType == typeof(uint))
            return ((UInt32)value1).CompareTo((UInt32)value2);
        else if (numberType == typeof(long))
            return ((Int64)value1).CompareTo((Int64)value2);
        else if (numberType == typeof(ulong))
            return ((UInt64)value1).CompareTo((UInt64)value2);
        else if (numberType == typeof(float))
            return ((Single)value1).CompareTo((Single)value2);
        else if (numberType == typeof(double))
            return ((Double)value1).CompareTo((Double)value2);
        else if (numberType == typeof(decimal))
            return ((Decimal)value1).CompareTo((Decimal)value2);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object CompareF(object value1, object value2, NumberType numberType)
    {
        if (numberType is NumberType.SByte)
            return ((SByte)value1).CompareTo((SByte)value2);
        else if (numberType is NumberType.Byte)
            return ((Byte)value1).CompareTo((Byte)value2);
        else if (numberType is NumberType.Int16)
            return ((Int16)value1).CompareTo((Int16)value2);
        else if (numberType is NumberType.UInt16)
            return ((UInt16)value1).CompareTo((UInt16)value2);
        else if (numberType is NumberType.Int32)
            return ((Int32)value1).CompareTo((Int32)value2);
        else if (numberType is NumberType.UInt32)
            return ((UInt32)value1).CompareTo((UInt32)value2);
        else if (numberType is NumberType.Int64)
            return ((Int64)value1).CompareTo((Int64)value2);
        else if (numberType is NumberType.UInt64)
            return ((UInt64)value1).CompareTo((UInt64)value2);
        else if (numberType is NumberType.Single)
            return ((Single)value1).CompareTo((Single)value2);
        else if (numberType is NumberType.Double)
            return ((Double)value1).CompareTo((Double)value2);
        else if (numberType is NumberType.Decimal)
            return ((Decimal)value1).CompareTo((Decimal)value2);
        else
            throw new NotImplementedException();
    }
}
