using System.Numerics;

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
    /// <exception cref="NotImplementedException">不支持的数值类型</exception>
    public static T CompareF<T>(object value1, object value2)
        where T : struct, INumber<T>
    {
        return (T)CompareF(value1, value2, typeof(T));
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的数值类型</exception>
    public static object CompareF(object value1, object value2, Type numberType)
    {
        return CompareF(value1, value2, GetNumberType(numberType));
    }

    /// <summary>
    /// 比较 (性能特化)
    /// </summary>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="numberType">类型</param>
    /// <returns>结果</returns>
    /// <exception cref="NotImplementedException">不支持的数值类型</exception>
    public static object CompareF(object value1, object value2, NumberType numberType)
    {
        switch (numberType)
        {
            case NumberType.SByte:
            {
                var left = (sbyte)value1;
                var right = (sbyte)value2;
                return left.CompareTo(right);
            }
            case NumberType.Byte:
            {
                var left = (byte)value1;
                var right = (byte)value2;
                return left.CompareTo(right);
            }
            case NumberType.Int16:
            {
                var left = (short)value1;
                var right = (short)value2;
                return left.CompareTo(right);
            }
            case NumberType.UInt16:
            {
                var left = (ushort)value1;
                var right = (ushort)value2;
                return left.CompareTo(right);
            }
            case NumberType.Int32:
            {
                var left = (int)value1;
                var right = (int)value2;
                return left.CompareTo(right);
            }
            case NumberType.UInt32:
            {
                var left = (uint)value1;
                var right = (uint)value2;
                return left.CompareTo(right);
            }
            case NumberType.Int64:
            {
                var left = (long)value1;
                var right = (long)value2;
                return left.CompareTo(right);
            }
            case NumberType.UInt64:
            {
                var left = (ulong)value1;
                var right = (ulong)value2;
                return left.CompareTo(right);
            }
            case NumberType.Single:
            {
                var left = (float)value1;
                var right = (float)value2;
                return left.CompareTo(right);
            }
            case NumberType.Double:
            {
                var left = (double)value1;
                var right = (double)value2;
                return left.CompareTo(right);
            }
            case NumberType.Decimal:
            {
                var left = (decimal)value1;
                var right = (decimal)value2;
                return left.CompareTo(right);
            }
            default:
                throw CreateUnsupportedNumberTypeException(numberType);
        }
    }
}
