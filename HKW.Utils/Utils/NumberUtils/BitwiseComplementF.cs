using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱为指定整数类型后按位取反, 不进行数值转换.
    /// </summary>
    /// <typeparam name="T">受支持的整数类型, 包括 BigInteger, 不支持浮点类型和 decimal.</typeparam>
    /// <param name="value">待取反的已装箱整数, 类型必须与 T 完全一致.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">源对象的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">源对象为 null.</exception>
    /// <returns>保持 <typeparamref name="T"/> 类型的运算结果.</returns>
    public static T BitwiseComplementF<T>(object value)
    {
        return (T)BitwiseComplementF(value, typeof(T));
    }

    /// <summary>
    /// 精确拆箱为指定运行时整数类型后按位取反, 不进行数值转换.
    /// </summary>
    /// <param name="value">待取反的已装箱整数, 类型必须与目标类型完全一致.</param>
    /// <param name="numberType">受支持的整数运行时类型, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <exception cref="NotImplementedException">目标类型不是受支持的整数类型.</exception>
    /// <exception cref="InvalidCastException">源对象的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">源对象为 null.</exception>
    /// <returns>已装箱且保持目标数值类型的运算结果.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static object BitwiseComplementF(object value, Type numberType)
    {
        return BitwiseComplementF(value, GetNumberType(numberType));
    }

    /// <summary>
    /// 精确拆箱整数后执行按位取反运算, 不进行数值转换.
    /// </summary>
    /// <param name="value">必须与目标整数类型完全一致的已装箱数值.</param>
    /// <param name="numberType">受支持的整数类型枚举值, 包括 BigInteger, 不支持浮点类型和 decimal.</param>
    /// <returns>已装箱且保持目标整数类型的取反结果.</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    public static object BitwiseComplementF(object value, NumberType numberType)
    {
#pragma warning disable S1905
        return numberType switch
        {
            NumberType.SByte => (object)unchecked((sbyte)~(sbyte)value),
            NumberType.Byte => (object)unchecked((byte)~(byte)value),
            NumberType.Int16 => (object)unchecked((short)~(short)value),
            NumberType.UInt16 => (object)unchecked((ushort)~(ushort)value),
            NumberType.Int32 => (object)unchecked((int)~(int)value),
            NumberType.UInt32 => (object)unchecked((uint)~(uint)value),
            NumberType.Int64 => (object)unchecked((long)~(long)value),
            NumberType.UInt64 => (object)unchecked((ulong)~(ulong)value),
            NumberType.IntPtr => (object)unchecked((nint)~(nint)value),
            NumberType.UIntPtr => (object)unchecked((nuint)~(nuint)value),
            NumberType.Int128 => (object)unchecked((Int128)~(Int128)value),
            NumberType.UInt128 => (object)unchecked((UInt128)~(UInt128)value),
            NumberType.BigInteger => (object)unchecked((BigInteger)~(BigInteger)value),
            _ => throw CreateUnsupportedOperatorException(numberType),
        };
#pragma warning restore S1905
    }
}
