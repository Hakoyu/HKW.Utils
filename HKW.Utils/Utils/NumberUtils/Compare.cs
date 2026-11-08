using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    #region Compare

    /// <summary>
    /// 转换为指定类型后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>负数表示左值在目标类型的排序规则中位于右值之前, 零表示顺序相等, 正数表示之后; 不保证幅值为一.</returns>
    public static int Compare<T>(object value1, object value2)
        where T : struct, INumber<T>
    {
        return Compare(value1, value2, typeof(T));
    }

    /// <summary>
    /// 转换为指定运行时类型后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前.
    /// </summary>
    /// <param name="value1">左操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="value2">右操作数, 转换为目标类型; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数不能转换为目标类型.</exception>
    /// <exception cref="FormatException">操作数文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标类型的范围.</exception>
    /// <returns>负数表示左值在目标类型的排序规则中位于右值之前, 零表示顺序相等, 正数表示之后; 不保证幅值为一.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static int Compare(object value1, object value2, Type numberType)
    {
        return Compare(value1, value2, GetNumberType(numberType));
    }

    /// <summary>
    /// 转换操作数后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前, 不采用关系运算符语义.
    /// </summary>
    /// <param name="value1">左操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="value2">右操作数, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <returns>负数表示左值排序在前, 零表示排序相等, 正数表示排序在后; 不保证幅值为一.</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    public static int Compare(object value1, object value2, NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => ConvertNumber<sbyte>(value1)
                .CompareTo(ConvertNumber<sbyte>(value2)),
            NumberType.Byte => ConvertNumber<byte>(value1).CompareTo(ConvertNumber<byte>(value2)),
            NumberType.Int16 => ConvertNumber<short>(value1)
                .CompareTo(ConvertNumber<short>(value2)),
            NumberType.UInt16 => ConvertNumber<ushort>(value1)
                .CompareTo(ConvertNumber<ushort>(value2)),
            NumberType.Int32 => ConvertNumber<int>(value1).CompareTo(ConvertNumber<int>(value2)),
            NumberType.UInt32 => ConvertNumber<uint>(value1).CompareTo(ConvertNumber<uint>(value2)),
            NumberType.Int64 => ConvertNumber<long>(value1).CompareTo(ConvertNumber<long>(value2)),
            NumberType.UInt64 => ConvertNumber<ulong>(value1)
                .CompareTo(ConvertNumber<ulong>(value2)),
            NumberType.Single => ConvertNumber<float>(value1)
                .CompareTo(ConvertNumber<float>(value2)),
            NumberType.Double => ConvertNumber<double>(value1)
                .CompareTo(ConvertNumber<double>(value2)),
            NumberType.Decimal => ConvertNumber<decimal>(value1)
                .CompareTo(ConvertNumber<decimal>(value2)),
            NumberType.IntPtr => ConvertNumber<nint>(value1).CompareTo(ConvertNumber<nint>(value2)),
            NumberType.UIntPtr => ConvertNumber<nuint>(value1)
                .CompareTo(ConvertNumber<nuint>(value2)),
            NumberType.Int128 => ConvertNumber<Int128>(value1)
                .CompareTo(ConvertNumber<Int128>(value2)),
            NumberType.UInt128 => ConvertNumber<UInt128>(value1)
                .CompareTo(ConvertNumber<UInt128>(value2)),
            NumberType.Half => ConvertNumber<Half>(value1).CompareTo(ConvertNumber<Half>(value2)),
            NumberType.BigInteger => ConvertNumber<BigInteger>(value1)
                .CompareTo(ConvertNumber<BigInteger>(value2)),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    #endregion
}
