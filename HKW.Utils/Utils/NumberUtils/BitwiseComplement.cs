namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 位运算取反
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static T BitwiseComplement<T>(object value)
    {
        return (T)BitwiseComplement(value, typeof(T));
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
        return BitwiseComplement(value, GetNumberType(numberType));
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
        return numberType switch
        {
            NumberType.SByte => ~Convert.ToSByte(value),
            NumberType.Byte => ~Convert.ToByte(value),
            NumberType.Int16 => ~Convert.ToInt16(value),
            NumberType.UInt16 => ~Convert.ToUInt16(value),
            NumberType.Int32 => ~Convert.ToInt32(value),
            NumberType.UInt32 => ~Convert.ToUInt32(value),
            NumberType.Int64 => ~Convert.ToInt64(value),
            NumberType.UInt64 => ~Convert.ToUInt64(value),
            _ => throw CreateUnsupportedOperatorException(numberType),
        };
    }
}
