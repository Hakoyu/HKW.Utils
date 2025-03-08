namespace HKW.HKWUtils;

public partial class NumberUtils
{
    /// <summary>
    /// 位运算取反
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplement<T>(object value)
    {
        var type = typeof(T);
        if (type == typeof(sbyte))
            return ~Convert.ToSByte(value);
        else if (type == typeof(byte))
            return ~Convert.ToByte(value);
        else if (type == typeof(short))
            return ~Convert.ToInt16(value);
        else if (type == typeof(ushort))
            return ~Convert.ToUInt16(value);
        else if (type == typeof(int))
            return ~Convert.ToInt32(value);
        else if (type == typeof(uint))
            return ~Convert.ToUInt32(value);
        else if (type == typeof(long))
            return ~Convert.ToInt64(value);
        else if (type == typeof(ulong))
            return ~Convert.ToUInt64(value);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 位运算取反
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplement(object value, Type numberType)
    {
        if (numberType == typeof(sbyte))
            return ~Convert.ToSByte(value);
        else if (numberType == typeof(byte))
            return ~Convert.ToByte(value);
        else if (numberType == typeof(short))
            return ~Convert.ToInt16(value);
        else if (numberType == typeof(ushort))
            return ~Convert.ToUInt16(value);
        else if (numberType == typeof(int))
            return ~Convert.ToInt32(value);
        else if (numberType == typeof(uint))
            return ~Convert.ToUInt32(value);
        else if (numberType == typeof(long))
            return ~Convert.ToInt64(value);
        else if (numberType == typeof(ulong))
            return ~Convert.ToUInt64(value);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 位运算取反
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplement(object value, NumberType numberType)
    {
        if (numberType is NumberType.SByte)
            return ~Convert.ToSByte(value);
        else if (numberType is NumberType.Byte)
            return ~Convert.ToByte(value);
        else if (numberType is NumberType.Int16)
            return ~Convert.ToInt16(value);
        else if (numberType is NumberType.UInt16)
            return ~Convert.ToUInt16(value);
        else if (numberType is NumberType.Int32)
            return ~Convert.ToInt32(value);
        else if (numberType is NumberType.UInt32)
            return ~Convert.ToUInt32(value);
        else if (numberType is NumberType.Int64)
            return ~Convert.ToInt64(value);
        else if (numberType is NumberType.UInt64)
            return ~Convert.ToUInt64(value);
        else
            throw new NotImplementedException();
    }
}
