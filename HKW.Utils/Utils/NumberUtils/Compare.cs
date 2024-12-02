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
    /// 比较运算符类型
    /// <para>(OperatorChar, ComparisonOperatorType)</para>
    /// </summary>
    public static FrozenBidirectionalDictionary<
        string,
        ComparisonOperatorType
    > ComparisonOperatorTypeByString { get; } =
        FrozenBidirectionalDictionary.Create<string, ComparisonOperatorType>(
            [
                ("==", ComparisonOperatorType.Equality),
                ("!=", ComparisonOperatorType.Inequality),
                ("<", ComparisonOperatorType.LessThan),
                (">", ComparisonOperatorType.GreaterThan),
                ("<=", ComparisonOperatorType.LessThanOrEqual),
                (">=", ComparisonOperatorType.GreaterThanOrEqual),
            ]
        );

    /// <summary>
    /// 获取比较运算符类型
    /// </summary>
    /// <param name="operator">运算符字符串</param>
    /// <returns>运算符</returns>
    public static ComparisonOperatorType GetComparisonOperatorType(string @operator)
    {
        if (@operator.Length <= 2)
            return ComparisonOperatorTypeByString[@operator];

        foreach (var pair in ComparisonOperatorTypeByString)
            if (@operator.StartsWith(pair.Key))
                return pair.Value;
        throw new ArgumentException(nameof(@operator));
    }

    /// <summary>
    /// 获取比较运算符类型
    /// </summary>
    /// <param name="operator">运算符字符串</param>
    /// <param name="operatorType">运算符类型</param>
    /// <returns>运算符</returns>
    public static bool TryGetComparisonOperatorType(
        string @operator,
        [MaybeNullWhen(false)] out ComparisonOperatorType operatorType
    )
    {
        if (@operator.Length <= 2)
            return ComparisonOperatorTypeByString.TryGetValue(@operator, out operatorType);

        foreach (var pair in ComparisonOperatorTypeByString)
        {
            if (@operator.StartsWith(pair.Key))
            {
                operatorType = pair.Value;
                return true;
            }
        }
        operatorType = default;
        return false;
    }

    #region Compare

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <returns>-1 表示value1<see langword="&lt;"/>value2,0 表示value1<see langword="=="/>value2,1 表示value1<see langword="&gt;"/>value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Compare<T>(object? value1, object? value2)
        where T : struct, INumber<T>
    {
        var type = typeof(T);

        if (type == typeof(sbyte))
            return Convert.ToSByte(value1).CompareTo(Convert.ToSByte(value2));
        else if (type == typeof(byte))
            return Convert.ToByte(value1).CompareTo(Convert.ToByte(value2));
        else if (type == typeof(short))
            return Convert.ToInt16(value1).CompareTo(Convert.ToInt16(value2));
        else if (type == typeof(ushort))
            return Convert.ToUInt16(value1).CompareTo(Convert.ToUInt16(value2));
        else if (type == typeof(int))
            return Convert.ToInt32(value1).CompareTo(Convert.ToInt32(value2));
        else if (type == typeof(uint))
            return Convert.ToUInt32(value1).CompareTo(Convert.ToUInt32(value2));
        else if (type == typeof(long))
            return Convert.ToInt64(value1).CompareTo(Convert.ToInt64(value2));
        else if (type == typeof(ulong))
            return Convert.ToUInt64(value1).CompareTo(Convert.ToUInt64(value2));
        else if (type == typeof(float))
            return Convert.ToSingle(value1).CompareTo(Convert.ToSingle(value2));
        else if (type == typeof(double))
            return Convert.ToDouble(value1).CompareTo(Convert.ToDouble(value2));
        else if (type == typeof(decimal))
            return Convert.ToDecimal(value1).CompareTo(Convert.ToDecimal(value2));
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>-1 表示value1<see langword="&lt;"/>value2,0 表示value1<see langword="=="/>value2,1 表示value1<see langword="&gt;"/>value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Compare(object? value1, object? value2, Type numberType)
    {
        if (numberType == typeof(sbyte))
            return Convert.ToSByte(value1).CompareTo(Convert.ToSByte(value2));
        else if (numberType == typeof(byte))
            return Convert.ToByte(value1).CompareTo(Convert.ToByte(value2));
        else if (numberType == typeof(short))
            return Convert.ToInt16(value1).CompareTo(Convert.ToInt16(value2));
        else if (numberType == typeof(ushort))
            return Convert.ToUInt16(value1).CompareTo(Convert.ToUInt16(value2));
        else if (numberType == typeof(int))
            return Convert.ToInt32(value1).CompareTo(Convert.ToInt32(value2));
        else if (numberType == typeof(uint))
            return Convert.ToUInt32(value1).CompareTo(Convert.ToUInt32(value2));
        else if (numberType == typeof(long))
            return Convert.ToInt64(value1).CompareTo(Convert.ToInt64(value2));
        else if (numberType == typeof(ulong))
            return Convert.ToUInt64(value1).CompareTo(Convert.ToUInt64(value2));
        else if (numberType == typeof(float))
            return Convert.ToSingle(value1).CompareTo(Convert.ToSingle(value2));
        else if (numberType == typeof(double))
            return Convert.ToDouble(value1).CompareTo(Convert.ToDouble(value2));
        else if (numberType == typeof(decimal))
            return Convert.ToDecimal(value1).CompareTo(Convert.ToDecimal(value2));
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>-1 表示value1<see langword="&lt;"/>value2,0 表示value1<see langword="=="/>value2,1 表示value1<see langword="&gt;"/>value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Compare(object? value1, object? value2, NumberType numberType)
    {
        if (numberType is NumberType.SByte)
            return Convert.ToSByte(value1).CompareTo(Convert.ToSByte(value2));
        else if (numberType is NumberType.Byte)
            return Convert.ToByte(value1).CompareTo(Convert.ToByte(value2));
        else if (numberType is NumberType.Int16)
            return Convert.ToInt16(value1).CompareTo(Convert.ToInt16(value2));
        else if (numberType is NumberType.UInt16)
            return Convert.ToUInt16(value1).CompareTo(Convert.ToUInt16(value2));
        else if (numberType is NumberType.Int32)
            return Convert.ToInt32(value1).CompareTo(Convert.ToInt32(value2));
        else if (numberType is NumberType.UInt32)
            return Convert.ToUInt32(value1).CompareTo(Convert.ToUInt32(value2));
        else if (numberType is NumberType.Int64)
            return Convert.ToInt64(value1).CompareTo(Convert.ToInt64(value2));
        else if (numberType is NumberType.UInt64)
            return Convert.ToUInt64(value1).CompareTo(Convert.ToUInt64(value2));
        else if (numberType is NumberType.Single)
            return Convert.ToSingle(value1).CompareTo(Convert.ToSingle(value2));
        else if (numberType is NumberType.Double)
            return Convert.ToDouble(value1).CompareTo(Convert.ToDouble(value2));
        else if (numberType is NumberType.Decimal)
            return Convert.ToDecimal(value1).CompareTo(Convert.ToDecimal(value2));
        else
            throw new NotImplementedException();
    }
    #endregion
}
