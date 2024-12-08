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
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareXF<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareXF<T>(value1, value2, GetComparisonOperatorType(@operator));
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
    public static bool CompareXF(object value1, object value2, Type numberType, string @operator)
    {
        return CompareXF(value1, value2, numberType, GetComparisonOperatorType(@operator));
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
    public static bool CompareXF(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return CompareXF(value1, value2, numberType, GetComparisonOperatorType(@operator));
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
    public static bool CompareXF<T>(
        object value1,
        object value2,
        ComparisonOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        var type = typeof(T);
        if (operatorType is ComparisonOperatorType.Equality)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) == ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) == ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) == ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) == ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) == ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) == ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) == ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) == ((UInt64)value2);
            else if (type == typeof(float))
                return ((Single)value1) == ((Single)value2);
            else if (type == typeof(double))
                return ((Double)value1) == ((Double)value2);
            else if (type == typeof(decimal))
                return ((Decimal)value1) == ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) != ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) != ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) != ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) != ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) != ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) != ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) != ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) != ((UInt64)value2);
            else if (type == typeof(float))
                return ((Single)value1) != ((Single)value2);
            else if (type == typeof(double))
                return ((Double)value1) != ((Double)value2);
            else if (type == typeof(decimal))
                return ((Decimal)value1) != ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) < ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) < ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) < ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) < ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) < ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) < ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) < ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) < ((UInt64)value2);
            else if (type == typeof(float))
                return ((Single)value1) < ((Single)value2);
            else if (type == typeof(double))
                return ((Double)value1) < ((Double)value2);
            else if (type == typeof(decimal))
                return ((Decimal)value1) < ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) > ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) > ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) > ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) > ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) > ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) > ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) > ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) > ((UInt64)value2);
            else if (type == typeof(float))
                return ((Single)value1) > ((Single)value2);
            else if (type == typeof(double))
                return ((Double)value1) > ((Double)value2);
            else if (type == typeof(decimal))
                return ((Decimal)value1) > ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) <= ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) <= ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) <= ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) <= ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) <= ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) <= ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) <= ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) <= ((UInt64)value2);
            else if (type == typeof(float))
                return ((Single)value1) <= ((Single)value2);
            else if (type == typeof(double))
                return ((Double)value1) <= ((Double)value2);
            else if (type == typeof(decimal))
                return ((Decimal)value1) <= ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (type == typeof(sbyte))
                return ((SByte)value1) >= ((SByte)value2);
            else if (type == typeof(byte))
                return ((Byte)value1) >= ((Byte)value2);
            else if (type == typeof(short))
                return ((Int16)value1) >= ((Int16)value2);
            else if (type == typeof(ushort))
                return ((UInt16)value1) >= ((UInt16)value2);
            else if (type == typeof(int))
                return ((Int32)value1) >= ((Int32)value2);
            else if (type == typeof(uint))
                return ((UInt32)value1) >= ((UInt32)value2);
            else if (type == typeof(long))
                return ((Int64)value1) >= ((Int64)value2);
            else if (type == typeof(ulong))
                return ((UInt64)value1) >= ((UInt64)value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数字类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareXF(
        object value1,
        object value2,
        Type numberType,
        ComparisonOperatorType operatorType
    )
    {
        if (operatorType is ComparisonOperatorType.Equality)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) == ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) == ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) == ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) == ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) == ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) == ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) == ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) == ((UInt64)value2);
            else if (numberType == typeof(float))
                return ((Single)value1) == ((Single)value2);
            else if (numberType == typeof(double))
                return ((Double)value1) == ((Double)value2);
            else if (numberType == typeof(decimal))
                return ((Decimal)value1) == ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) != ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) != ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) != ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) != ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) != ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) != ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) != ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) != ((UInt64)value2);
            else if (numberType == typeof(float))
                return ((Single)value1) != ((Single)value2);
            else if (numberType == typeof(double))
                return ((Double)value1) != ((Double)value2);
            else if (numberType == typeof(decimal))
                return ((Decimal)value1) != ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) < ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) < ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) < ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) < ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) < ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) < ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) < ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) < ((UInt64)value2);
            else if (numberType == typeof(float))
                return ((Single)value1) < ((Single)value2);
            else if (numberType == typeof(double))
                return ((Double)value1) < ((Double)value2);
            else if (numberType == typeof(decimal))
                return ((Decimal)value1) < ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) > ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) > ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) > ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) > ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) > ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) > ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) > ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) > ((UInt64)value2);
            else if (numberType == typeof(float))
                return ((Single)value1) > ((Single)value2);
            else if (numberType == typeof(double))
                return ((Double)value1) > ((Double)value2);
            else if (numberType == typeof(decimal))
                return ((Decimal)value1) > ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) <= ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) <= ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) <= ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) <= ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) <= ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) <= ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) <= ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) <= ((UInt64)value2);
            else if (numberType == typeof(float))
                return ((Single)value1) <= ((Single)value2);
            else if (numberType == typeof(double))
                return ((Double)value1) <= ((Double)value2);
            else if (numberType == typeof(decimal))
                return ((Decimal)value1) <= ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (numberType == typeof(sbyte))
                return ((SByte)value1) >= ((SByte)value2);
            else if (numberType == typeof(byte))
                return ((Byte)value1) >= ((Byte)value2);
            else if (numberType == typeof(short))
                return ((Int16)value1) >= ((Int16)value2);
            else if (numberType == typeof(ushort))
                return ((UInt16)value1) >= ((UInt16)value2);
            else if (numberType == typeof(int))
                return ((Int32)value1) >= ((Int32)value2);
            else if (numberType == typeof(uint))
                return ((UInt32)value1) >= ((UInt32)value2);
            else if (numberType == typeof(long))
                return ((Int64)value1) >= ((Int64)value2);
            else if (numberType == typeof(ulong))
                return ((UInt64)value1) >= ((UInt64)value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数字类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareXF(
        object value1,
        object value2,
        NumberType numberType,
        ComparisonOperatorType operatorType
    )
    {
        if (operatorType is ComparisonOperatorType.Equality)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) == ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) == ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) == ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) == ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) == ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) == ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) == ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) == ((UInt64)value2);
            else if (numberType is NumberType.Single)
                return ((Single)value1) == ((Single)value2);
            else if (numberType is NumberType.Double)
                return ((Double)value1) == ((Double)value2);
            else if (numberType is NumberType.Decimal)
                return ((Decimal)value1) == ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) != ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) != ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) != ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) != ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) != ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) != ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) != ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) != ((UInt64)value2);
            else if (numberType is NumberType.Single)
                return ((Single)value1) != ((Single)value2);
            else if (numberType is NumberType.Double)
                return ((Double)value1) != ((Double)value2);
            else if (numberType is NumberType.Decimal)
                return ((Decimal)value1) != ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) < ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) < ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) < ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) < ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) < ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) < ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) < ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) < ((UInt64)value2);
            else if (numberType is NumberType.Single)
                return ((Single)value1) < ((Single)value2);
            else if (numberType is NumberType.Double)
                return ((Double)value1) < ((Double)value2);
            else if (numberType is NumberType.Decimal)
                return ((Decimal)value1) < ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) > ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) > ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) > ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) > ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) > ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) > ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) > ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) > ((UInt64)value2);
            else if (numberType is NumberType.Single)
                return ((Single)value1) > ((Single)value2);
            else if (numberType is NumberType.Double)
                return ((Double)value1) > ((Double)value2);
            else if (numberType is NumberType.Decimal)
                return ((Decimal)value1) > ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) <= ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) <= ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) <= ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) <= ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) <= ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) <= ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) <= ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) <= ((UInt64)value2);
            else if (numberType is NumberType.Single)
                return ((Single)value1) <= ((Single)value2);
            else if (numberType is NumberType.Double)
                return ((Double)value1) <= ((Double)value2);
            else if (numberType is NumberType.Decimal)
                return ((Decimal)value1) <= ((Decimal)value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (numberType is NumberType.SByte)
                return ((SByte)value1) >= ((SByte)value2);
            else if (numberType is NumberType.Byte)
                return ((Byte)value1) >= ((Byte)value2);
            else if (numberType is NumberType.Int16)
                return ((Int16)value1) >= ((Int16)value2);
            else if (numberType is NumberType.UInt16)
                return ((UInt16)value1) >= ((UInt16)value2);
            else if (numberType is NumberType.Int32)
                return ((Int32)value1) >= ((Int32)value2);
            else if (numberType is NumberType.UInt32)
                return ((UInt32)value1) >= ((UInt32)value2);
            else if (numberType is NumberType.Int64)
                return ((Int64)value1) >= ((Int64)value2);
            else if (numberType is NumberType.UInt64)
                return ((UInt64)value1) >= ((UInt64)value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
