using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    #region Compare

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <returns>-1 表示value1<see langword="&lt;"/>value2,0 表示value1<see langword="=="/>value2,1 表示value1<see langword="&gt;"/>value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static T Compare<T>(object value1, object value2)
        where T : struct, INumber<T>
    {
        return (T)Compare(value1, value2, typeof(T));
    }

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>-1 表示value1<see langword="&lt;"/>value2,0 表示value1<see langword="=="/>value2,1 表示value1<see langword="&gt;"/>value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Compare(object value1, object value2, Type numberType)
    {
        return Compare(value1, value2, GetNumberType(numberType));
    }

    /// <summary>
    /// 比较 (返回数值)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>小于 0 表示 value1 <see langword="&lt;"/> value2, 0 表示 value1 <see langword="=="/> value2, 大于 0 表示value1 <see langword="&gt;"/> value2</returns>
    /// <exception cref="NotImplementedException">不支持的操作</exception>
    public static object Compare(object value1, object value2, NumberType numberType)
    {
        switch (numberType)
        {
            case NumberType.SByte:
            {
                var left = Convert.ToSByte(value1);
                var right = Convert.ToSByte(value2);
                return left.CompareTo(right);
            }
            case NumberType.Byte:
            {
                var left = Convert.ToByte(value1);
                var right = Convert.ToByte(value2);
                return left.CompareTo(right);
            }
            case NumberType.Int16:
            {
                var left = Convert.ToInt16(value1);
                var right = Convert.ToInt16(value2);
                return left.CompareTo(right);
            }
            case NumberType.UInt16:
            {
                var left = Convert.ToUInt16(value1);
                var right = Convert.ToUInt16(value2);
                return left.CompareTo(right);
            }
            case NumberType.Int32:
            {
                var left = Convert.ToInt32(value1);
                var right = Convert.ToInt32(value2);
                return left.CompareTo(right);
            }
            case NumberType.UInt32:
            {
                var left = Convert.ToUInt32(value1);
                var right = Convert.ToUInt32(value2);
                return left.CompareTo(right);
            }
            case NumberType.Int64:
            {
                var left = Convert.ToInt64(value1);
                var right = Convert.ToInt64(value2);
                return left.CompareTo(right);
            }
            case NumberType.UInt64:
            {
                var left = Convert.ToUInt64(value1);
                var right = Convert.ToUInt64(value2);
                return left.CompareTo(right);
            }
            case NumberType.Single:
            {
                var left = Convert.ToSingle(value1);
                var right = Convert.ToSingle(value2);
                return left.CompareTo(right);
            }
            case NumberType.Double:
            {
                var left = Convert.ToDouble(value1);
                var right = Convert.ToDouble(value2);
                return left.CompareTo(right);
            }
            case NumberType.Decimal:
            {
                var left = Convert.ToDecimal(value1);
                var right = Convert.ToDecimal(value2);
                return left.CompareTo(right);
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }

    #endregion
}
