using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 计算运算符类型
    /// <para>(OperatorChar, ArithmeticOperatorType)</para>
    /// </summary>
    public static FrozenDictionary<
        char,
        ArithmeticOperatorType
    > ArithmeticOperatorTypeByChar { get; } =
        FrozenDictionary.ToFrozenDictionary<char, ArithmeticOperatorType>(
            [
                KeyValuePair.Create('+', ArithmeticOperatorType.Addition),
                KeyValuePair.Create('-', ArithmeticOperatorType.Subtraction),
                KeyValuePair.Create('*', ArithmeticOperatorType.Multiply),
                KeyValuePair.Create('/', ArithmeticOperatorType.Division),
                KeyValuePair.Create('|', ArithmeticOperatorType.BitwiseOr),
                KeyValuePair.Create('&', ArithmeticOperatorType.BitwiseAnd),
            ]
        );

    /// <summary>
    /// 获取计算运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <returns>运算符类型</returns>
    public static ArithmeticOperatorType GetArithmeticOperatorType(char @operator)
    {
        return ArithmeticOperatorTypeByChar[@operator];
    }

    /// <summary>
    /// 获取计算运算符类型
    /// </summary>
    /// <param name="operator">运算符字符</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符类型</returns>
    public static bool TryGetArithmeticOperatorType(
        char @operator,
        out ArithmeticOperatorType operatorType
    )
    {
        return ArithmeticOperatorTypeByChar.TryGetValue(@operator, out operatorType);
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
    public static object Arithmetic<T>(object? value1, object? value2, char @operator)
        where T : struct, INumber<T>
    {
        return Arithmetic<T>(value1, value2, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Arithmetic<T>(
        object? value1,
        object? value2,
        Type numberType,
        char @operator
    )
        where T : struct, INumber<T>
    {
        return Arithmetic(value1, value2, numberType, GetArithmeticOperatorType(@operator));
    }

    /// <summary>
    /// 计算
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">数值类型</param>
    /// <param name="operator">运算符</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Arithmetic<T>(
        object? value1,
        object? value2,
        NumberType numberType,
        char @operator
    )
        where T : struct, INumber<T>
    {
        return Arithmetic(value1, value2, numberType, GetArithmeticOperatorType(@operator));
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
    public static object Arithmetic<T>(
        object? value1,
        object? value2,
        ArithmeticOperatorType @operator
    )
        where T : struct, INumber<T>
    {
        var type = typeof(T);
        if (@operator is ArithmeticOperatorType.Addition)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) + Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) + Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) + Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) + Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) + Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) + Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) + Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) + Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) + Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) + Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) + Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Subtraction)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) - Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) - Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) - Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) - Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) - Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) - Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) - Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) - Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) - Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) - Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) - Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Multiply)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) * Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) * Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) * Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) * Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) * Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) * Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) * Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) * Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) * Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) * Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) * Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Division)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) / Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) / Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) / Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) / Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) / Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) / Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) / Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) / Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) / Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) / Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) / Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Modulus)
        {
            if (type == typeof(sbyte))
                return Convert.ToSByte(value1) % Convert.ToSByte(value2);
            else if (type == typeof(byte))
                return Convert.ToByte(value1) % Convert.ToByte(value2);
            else if (type == typeof(short))
                return Convert.ToInt16(value1) % Convert.ToInt16(value2);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value1) % Convert.ToUInt16(value2);
            else if (type == typeof(int))
                return Convert.ToInt32(value1) % Convert.ToInt32(value2);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value1) % Convert.ToUInt32(value2);
            else if (type == typeof(long))
                return Convert.ToInt64(value1) % Convert.ToInt64(value2);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value1) % Convert.ToUInt64(value2);
            else if (type == typeof(float))
                return Convert.ToSingle(value1) % Convert.ToSingle(value2);
            else if (type == typeof(double))
                return Convert.ToDouble(value1) % Convert.ToDouble(value2);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value1) % Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.BitwiseOr)
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
        else if (@operator is ArithmeticOperatorType.BitwiseAnd)
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
        else
            throw new NotImplementedException();
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
    public static object Arithmetic(
        object? value1,
        object? value2,
        Type numberType,
        ArithmeticOperatorType @operator
    )
    {
        if (@operator is ArithmeticOperatorType.Addition)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) + Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) + Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) + Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) + Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) + Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) + Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) + Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) + Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) + Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) + Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) + Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Subtraction)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) - Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) - Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) - Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) - Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) - Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) - Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) - Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) - Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) - Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) - Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) - Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Multiply)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) * Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) * Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) * Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) * Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) * Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) * Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) * Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) * Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) * Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) * Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) * Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Division)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) / Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) / Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) / Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) / Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) / Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) / Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) / Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) / Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) / Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) / Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) / Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Modulus)
        {
            if (numberType == typeof(sbyte))
                return Convert.ToSByte(value1) % Convert.ToSByte(value2);
            else if (numberType == typeof(byte))
                return Convert.ToByte(value1) % Convert.ToByte(value2);
            else if (numberType == typeof(short))
                return Convert.ToInt16(value1) % Convert.ToInt16(value2);
            else if (numberType == typeof(ushort))
                return Convert.ToUInt16(value1) % Convert.ToUInt16(value2);
            else if (numberType == typeof(int))
                return Convert.ToInt32(value1) % Convert.ToInt32(value2);
            else if (numberType == typeof(uint))
                return Convert.ToUInt32(value1) % Convert.ToUInt32(value2);
            else if (numberType == typeof(long))
                return Convert.ToInt64(value1) % Convert.ToInt64(value2);
            else if (numberType == typeof(ulong))
                return Convert.ToUInt64(value1) % Convert.ToUInt64(value2);
            else if (numberType == typeof(float))
                return Convert.ToSingle(value1) % Convert.ToSingle(value2);
            else if (numberType == typeof(double))
                return Convert.ToDouble(value1) % Convert.ToDouble(value2);
            else if (numberType == typeof(decimal))
                return Convert.ToDecimal(value1) % Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.BitwiseOr)
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
        else if (@operator is ArithmeticOperatorType.BitwiseAnd)
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
        else
            throw new NotImplementedException();
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
    public static object Arithmetic(
        object? value1,
        object? value2,
        NumberType numberType,
        ArithmeticOperatorType @operator
    )
    {
        if (@operator is ArithmeticOperatorType.Addition)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) + Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) + Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) + Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) + Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) + Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) + Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) + Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) + Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) + Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) + Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) + Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Subtraction)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) - Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) - Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) - Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) - Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) - Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) - Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) - Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) - Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) - Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) - Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) - Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Multiply)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) * Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) * Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) * Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) * Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) * Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) * Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) * Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) * Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) * Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) * Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) * Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Division)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) / Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) / Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) / Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) / Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) / Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) / Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) / Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) / Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) / Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) / Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) / Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.Modulus)
        {
            if (numberType is NumberType.SByte)
                return Convert.ToSByte(value1) % Convert.ToSByte(value2);
            else if (numberType is NumberType.Byte)
                return Convert.ToByte(value1) % Convert.ToByte(value2);
            else if (numberType is NumberType.Int16)
                return Convert.ToInt16(value1) % Convert.ToInt16(value2);
            else if (numberType is NumberType.UInt16)
                return Convert.ToUInt16(value1) % Convert.ToUInt16(value2);
            else if (numberType is NumberType.Int32)
                return Convert.ToInt32(value1) % Convert.ToInt32(value2);
            else if (numberType is NumberType.UInt32)
                return Convert.ToUInt32(value1) % Convert.ToUInt32(value2);
            else if (numberType is NumberType.Int64)
                return Convert.ToInt64(value1) % Convert.ToInt64(value2);
            else if (numberType is NumberType.UInt64)
                return Convert.ToUInt64(value1) % Convert.ToUInt64(value2);
            else if (numberType is NumberType.Single)
                return Convert.ToSingle(value1) % Convert.ToSingle(value2);
            else if (numberType is NumberType.Double)
                return Convert.ToDouble(value1) % Convert.ToDouble(value2);
            else if (numberType is NumberType.Decimal)
                return Convert.ToDecimal(value1) % Convert.ToDecimal(value2);
            else
                throw new NotImplementedException();
        }
        else if (@operator is ArithmeticOperatorType.BitwiseOr)
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
        else if (@operator is ArithmeticOperatorType.BitwiseAnd)
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
        else
            throw new NotImplementedException();
    }
}
