using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 位运算符类型
    /// <para>(OperatorChar, BitwiseOperatorType)</para>
    /// </summary>
    public static FrozenBidirectionalDictionary<
        char,
        BitwiseOperatorType
    > BitwiseOperatorTypeByChar { get; } =
        FrozenBidirectionalDictionary.Create<char, BitwiseOperatorType>(
            [
                ('|', BitwiseOperatorType.Or),
                ('&', BitwiseOperatorType.And),
                ('^', BitwiseOperatorType.LogicalOr),
            ]
        );

    /// <summary>
    /// 获取位运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <returns>运算符</returns>
    public static BitwiseOperatorType GetBitwiseOperatorType(char @operator)
    {
        return BitwiseOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 获取位运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符</returns>
    public static bool TryGetBitwiseOperatorType(
        char @operator,
        out BitwiseOperatorType operatorType
    )
    {
        return BitwiseOperatorTypeByChar.TryGetValue(@operator, out operatorType);
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator<T>(object value1, object value2, char @operator)
        where T : struct, INumber<T>
    {
        return BitwiseOperator<T>(value1, value2, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        Type numberType,
        char @operator
    )
    {
        return BitwiseOperator(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        NumberType numberType,
        char @operator
    )
    {
        return BitwiseOperator(value1, value2, numberType, GetBitwiseOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator<T>(
        object value1,
        object value2,
        BitwiseOperatorType operatorType
    )
        where T : struct, INumber<T>
    {
        var type = typeof(T);
        if (operatorType is BitwiseOperatorType.Or)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) | Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) | Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) | Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) | Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) | Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) | Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) | Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) | Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.And)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) & Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) & Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) & Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) & Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) & Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) & Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) & Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) & Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.LogicalOr)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) ^ Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) ^ Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) ^ Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) ^ Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) ^ Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) ^ Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) ^ Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) ^ Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        Type numberType,
        BitwiseOperatorType operatorType
    )
    {
        if (operatorType is BitwiseOperatorType.Or)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) | Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) | Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) | Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) | Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) | Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) | Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) | Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) | Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.And)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) & Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) & Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) & Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) & Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) & Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) & Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) & Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) & Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.LogicalOr)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) ^ Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) ^ Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) ^ Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) ^ Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) ^ Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) ^ Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) ^ Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) ^ Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object BitwiseOperator(
        object value1,
        object value2,
        NumberType numberType,
        BitwiseOperatorType operatorType
    )
    {
        if (operatorType is BitwiseOperatorType.Or)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) | Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) | Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) | Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) | Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) | Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) | Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) | Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) | Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.And)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) & Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) & Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) & Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) & Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) & Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) & Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) & Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) & Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else if (operatorType is BitwiseOperatorType.LogicalOr)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) ^ Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) ^ Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) ^ Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) ^ Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) ^ Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) ^ Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) ^ Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) ^ Convert.ToUInt64(value2);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
