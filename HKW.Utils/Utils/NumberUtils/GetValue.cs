using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 获取目标数值类型的零值.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <returns>目标类型的零.</returns>
    /// <exception cref="NotImplementedException"><typeparamref name="T"/> 不是受支持的数值类型.</exception>
    public static T GetDefault<T>()
        where T : struct, INumber<T>
    {
        return (T)GetDefault(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取指定运行时数值类型的零值.
    /// </summary>
    /// <param name="numberType">本工具支持的数值运行时类型.</param>
    /// <returns>已装箱的目标类型零, 包括 <see cref="BigInteger.Zero"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
    public static object GetDefault(Type numberType)
    {
        return GetDefault(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取指定数值类型的零值.
    /// </summary>
    /// <param name="numberType">本工具支持的数值类型枚举值.</param>
    /// <returns>已装箱的目标类型零, 包括 <see cref="BigInteger.Zero"/>.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不是受支持的数值类型.</exception>
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
            NumberType.IntPtr => default(nint),
            NumberType.UIntPtr => default(nuint),
            NumberType.Int128 => default(Int128),
            NumberType.UInt128 => default(UInt128),
            NumberType.Half => default(Half),
            NumberType.BigInteger => default(BigInteger),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 获取目标数值类型的有限最小值.
    /// </summary>
    /// <typeparam name="T">本工具支持且具有有限边界的数值类型.</typeparam>
    /// <returns>目标类型的有限最小值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="NotImplementedException"><typeparamref name="T"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
    public static T GetMinValue<T>()
        where T : struct, INumber<T>
    {
        return (T)GetMinValue(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取指定运行时数值类型的有限最小值.
    /// </summary>
    /// <param name="numberType">本工具支持且具有有限边界的数值运行时类型.</param>
    /// <returns>已装箱的有限最小值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
    public static object GetMinValue(Type numberType)
    {
        return GetMinValue(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取指定数值类型的有限最小值.
    /// </summary>
    /// <param name="numberType">本工具支持且具有有限边界的数值类型枚举值.</param>
    /// <returns>已装箱的有限最小值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
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
            NumberType.IntPtr => nint.MinValue,
            NumberType.UIntPtr => nuint.MinValue,
            NumberType.Int128 => Int128.MinValue,
            NumberType.UInt128 => UInt128.MinValue,
            NumberType.Half => Half.MinValue,
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }

    /// <summary>
    /// 获取目标数值类型的有限最大值.
    /// </summary>
    /// <typeparam name="T">本工具支持且具有有限边界的数值类型.</typeparam>
    /// <returns>目标类型的有限最大值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="NotImplementedException"><typeparamref name="T"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
    public static T GetMaxValue<T>()
        where T : struct, INumber<T>
    {
        return (T)GetMaxValue(GetNumberType(typeof(T)));
    }

    /// <summary>
    /// 获取指定运行时数值类型的有限最大值.
    /// </summary>
    /// <param name="numberType">本工具支持且具有有限边界的数值运行时类型.</param>
    /// <returns>已装箱的有限最大值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
    public static object GetMaxValue(Type numberType)
    {
        return GetMaxValue(GetNumberType(numberType));
    }

    /// <summary>
    /// 获取指定数值类型的有限最大值.
    /// </summary>
    /// <param name="numberType">本工具支持且具有有限边界的数值类型枚举值.</param>
    /// <returns>已装箱的有限最大值; 原生整数边界取决于当前进程位宽.</returns>
    /// <exception cref="NotImplementedException"><paramref name="numberType"/> 不受支持或为没有有限边界的 <see cref="BigInteger"/>.</exception>
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
            NumberType.IntPtr => nint.MaxValue,
            NumberType.UIntPtr => nuint.MaxValue,
            NumberType.Int128 => Int128.MaxValue,
            NumberType.UInt128 => UInt128.MaxValue,
            NumberType.Half => Half.MaxValue,
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
