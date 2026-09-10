using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <returns>默认值</returns>
    public static T GetDefault<T>()
        where T : struct, INumber<T>
    {
        return (T)GetDefault(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>默认值</returns>
    public static object GetDefault(Type numberType)
    {
        return GetDefault(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>默认值</returns>
    public static object GetDefault(NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => default(sbyte),
            NumberType.Byte => default(byte),
            NumberType.Int16 => default(short),
            NumberType.UInt16 => default(ushort),
            NumberType.Int32 => default(int),
            NumberType.UInt32 => default(uint),
            NumberType.Int64 => default(long),
            NumberType.UInt64 => default(ulong),
            NumberType.Single => default(float),
            NumberType.Double => default(double),
            NumberType.Decimal => default(decimal),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 获取最小值
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <returns>最小值</returns>
    public static T GetMinValue<T>()
        where T : struct, INumber<T>
    {
        return (T)GetMinValue(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取最小值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>最小值</returns>
    public static object GetMinValue(Type numberType)
    {
        return GetMinValue(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取最小值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>最小值</returns>
    public static object GetMinValue(NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => sbyte.MinValue,
            NumberType.Byte => byte.MinValue,
            NumberType.Int16 => short.MinValue,
            NumberType.UInt16 => ushort.MinValue,
            NumberType.Int32 => int.MinValue,
            NumberType.UInt32 => uint.MinValue,
            NumberType.Int64 => long.MinValue,
            NumberType.UInt64 => ulong.MinValue,
            NumberType.Single => float.MinValue,
            NumberType.Double => double.MinValue,
            NumberType.Decimal => decimal.MinValue,
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 获取最大值
    /// </summary>
    /// <typeparam name="T">数值类型</typeparam>
    /// <returns>最大值</returns>
    public static T GetMaxValue<T>()
        where T : struct, INumber<T>
    {
        return (T)GetMaxValue(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取最大值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>最大值</returns>
    public static object GetMaxValue(Type numberType)
    {
        return GetMaxValue(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取最大值
    /// </summary>
    /// <param name="numberType">数值类型</param>
    /// <returns>最大值</returns>
    public static object GetMaxValue(NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => sbyte.MaxValue,
            NumberType.Byte => byte.MaxValue,
            NumberType.Int16 => short.MaxValue,
            NumberType.UInt16 => ushort.MaxValue,
            NumberType.Int32 => int.MaxValue,
            NumberType.UInt32 => uint.MaxValue,
            NumberType.Int64 => long.MaxValue,
            NumberType.UInt64 => ulong.MaxValue,
            NumberType.Single => float.MaxValue,
            NumberType.Double => double.MaxValue,
            NumberType.Decimal => decimal.MaxValue,
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
