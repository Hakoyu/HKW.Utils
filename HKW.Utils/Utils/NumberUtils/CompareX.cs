using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX<T>(object value1, object value2, string @operator)
        where T : struct, INumber<T>
    {
        return CompareX<T>(value1, value2, GetComparisonOperatorType(@operator));
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX(object value1, object value2, Type numberType, string @operator)
    {
        return CompareX(value1, value2, numberType, GetComparisonOperatorType(@operator));
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX(
        object value1,
        object value2,
        NumberType numberType,
        string @operator
    )
    {
        return CompareX(value1, value2, numberType, GetComparisonOperatorType(@operator));
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX<T>(
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
                return Convert.ToSByte(value1) == Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) == Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) == Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) == Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) == Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) == Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) == Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) == Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) == Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) == Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) == Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) != Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) != Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) != Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) != Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) != Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) != Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) != Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) != Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) != Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) != Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) != Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) < Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) < Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) < Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) < Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) < Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) < Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) < Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) < Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) < Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) < Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) < Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) > Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) > Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) > Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) > Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) > Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) > Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) > Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) > Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) > Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) > Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) > Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) <= Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) <= Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) <= Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) <= Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) <= Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) <= Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) <= Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) <= Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) <= Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) <= Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) <= Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) >= Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) >= Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) >= Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) >= Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) >= Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) >= Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) >= Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) >= Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX(
        object value1,
        object value2,
        Type numberType,
        ComparisonOperatorType operatorType
    )
    {
        if (operatorType is ComparisonOperatorType.Equality)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) == Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) == Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) == Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) == Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) == Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) == Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) == Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) == Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) == Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) == Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) == Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) != Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) != Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) != Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) != Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) != Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) != Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) != Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) != Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) != Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) != Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) != Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) < Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) < Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) < Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) < Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) < Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) < Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) < Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) < Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) < Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) < Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) < Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) > Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) > Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) > Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) > Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) > Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) > Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) > Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) > Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) > Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) > Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) > Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) <= Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) <= Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) <= Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) <= Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) <= Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) <= Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) <= Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) <= Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) <= Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) <= Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) <= Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) >= Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) >= Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) >= Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) >= Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) >= Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) >= Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) >= Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) >= Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (返回结果)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>比较的结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static bool CompareX(
        object value1,
        object value2,
        NumberType numberType,
        ComparisonOperatorType operatorType
    )
    {
        if (operatorType is ComparisonOperatorType.Equality)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) == Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) == Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) == Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) == Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) == Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) == Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) == Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) == Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) == Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) == Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) == Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.Inequality)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) != Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) != Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) != Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) != Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) != Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) != Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) != Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) != Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) != Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) != Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) != Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThan)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) < Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) < Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) < Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) < Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) < Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) < Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) < Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) < Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) < Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) < Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) < Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThan)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) > Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) > Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) > Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) > Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) > Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) > Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) > Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) > Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) > Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) > Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) > Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.LessThanOrEqual)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) <= Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) <= Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) <= Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) <= Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) <= Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) <= Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) <= Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) <= Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) <= Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) <= Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) <= Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is ComparisonOperatorType.GreaterThanOrEqual)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) >= Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) >= Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) >= Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) >= Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) >= Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) >= Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) >= Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) >= Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
