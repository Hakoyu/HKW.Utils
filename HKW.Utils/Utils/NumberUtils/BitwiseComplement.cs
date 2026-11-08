using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 转换为指定整数类型后按位取反.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value">待取反的源对象, 转换为目标整数类型; null 按零处理.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">源文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseComplement<T>(object value)
    {
        return (T)BitwiseComplement(value, typeof(T));
    }

    /// <summary>
    /// 转换为指定运行时整数类型后按位取反.
    /// </summary>
    /// <param name="value">待取反的源对象, 转换为目标整数类型; null 按零处理.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为目标整数类型.</exception>
    /// <exception cref="FormatException">源文本格式无效.</exception>
    /// <exception cref="OverflowException">数值转换超出目标整数类型的范围.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseComplement(object value, Type numberType)
    {
        return BitwiseComplement(value, GetNumberType(numberType));
    }

    /// <summary>
    /// 转换整数后执行按位取反运算.
    /// </summary>
    /// <param name="value">待取反的源对象, 数值按 CreateChecked 转换, 其他对象按兼容规则和当前区域性转换; null 按零处理.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <returns>已装箱且保持目标整数类型的取反结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">源对象不能转换为所需数值类型.</exception>
    /// <exception cref="FormatException">源文本不符合所需数值类型的格式.</exception>
    /// <exception cref="OverflowException">目标类型报告转换或运算结果超出可表示范围.</exception>
    public static object BitwiseComplement(object value, NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => unchecked(~ConvertNumber<sbyte>(value)),
            NumberType.Byte => unchecked(~ConvertNumber<byte>(value)),
            NumberType.Int16 => unchecked(~ConvertNumber<short>(value)),
            NumberType.UInt16 => unchecked(~ConvertNumber<ushort>(value)),
            NumberType.Int32 => unchecked(~ConvertNumber<int>(value)),
            NumberType.UInt32 => unchecked(~ConvertNumber<uint>(value)),
            NumberType.Int64 => unchecked(~ConvertNumber<long>(value)),
            NumberType.UInt64 => unchecked(~ConvertNumber<ulong>(value)),
            NumberType.IntPtr => unchecked(~ConvertNumber<nint>(value)),
            NumberType.UIntPtr => unchecked(~ConvertNumber<nuint>(value)),
            NumberType.Int128 => unchecked(~ConvertNumber<Int128>(value)),
            NumberType.UInt128 => unchecked(~ConvertNumber<UInt128>(value)),
            NumberType.BigInteger => unchecked(~ConvertNumber<BigInteger>(value)),
            _ => throw CreateUnsupportedOperatorException(numberType),
        };
    }
}
