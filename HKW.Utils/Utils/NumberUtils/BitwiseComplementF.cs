namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF<T>(object value)
    {
        return BitwiseComplementF(value, typeof(T));
    }

    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF(object value, Type numberType)
    {
        return BitwiseComplementF(value, GetNumberType(numberType));
    }

    /// <summary>
    /// 位运算取反 (性能特化)
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="numberType">数值类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseComplementF(object value, NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => ~(sbyte)value,
            NumberType.Byte => ~(byte)value,
            NumberType.Int16 => ~(short)value,
            NumberType.UInt16 => ~(ushort)value,
            NumberType.Int32 => ~(int)value,
            NumberType.UInt32 => ~(uint)value,
            NumberType.Int64 => ~(long)value,
            NumberType.UInt64 => ~(ulong)value,
            _ => throw CreateUnsupportedOperatorException(numberType),
        };
    }
}
