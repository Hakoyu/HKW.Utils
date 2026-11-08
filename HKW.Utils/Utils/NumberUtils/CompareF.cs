using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    /// <summary>
    /// 精确拆箱为指定类型后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前.
    /// </summary>
    /// <typeparam name="T">本工具支持的目标数值类型.</typeparam>
    /// <param name="value1">左操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与 T 完全一致, 不进行数值转换.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>负数表示左值在目标类型的排序规则中位于右值之前, 零表示顺序相等, 正数表示之后; 不保证幅值为一.</returns>
    public static int CompareF<T>(object value1, object value2)
        where T : struct, INumber<T>
    {
        return CompareF(value1, value2, typeof(T));
    }

    /// <summary>
    /// 精确拆箱为指定运行时类型后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值运行时类型.</param>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型与目标类型不一致.</exception>
    /// <exception cref="NullReferenceException">操作数为 null.</exception>
    /// <returns>负数表示左值在目标类型的排序规则中位于右值之前, 零表示顺序相等, 正数表示之后; 不保证幅值为一.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numberType"/> 为 <see langword="null"/>.</exception>
    public static int CompareF(object value1, object value2, Type numberType)
    {
        return CompareF(value1, value2, GetNumberType(numberType));
    }

    /// <summary>
    /// 精确拆箱后按 CompareTo 比较顺序; 浮点 NaN 与自身排序相等且排在其他值之前, 不采用关系运算符语义.
    /// </summary>
    /// <param name="value1">左操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="value2">右操作数, 装箱类型必须与目标类型完全一致, 不进行数值转换.</param>
    /// <param name="numberType">受支持的目标数值类型枚举值.</param>
    /// <returns>负数表示左值排序在前, 零表示排序相等, 正数表示排序在后; 不保证幅值为一.</returns>
    /// <exception cref="NotImplementedException">目标数值类型不受支持.</exception>
    /// <exception cref="InvalidCastException">操作数的装箱类型不符合要求.</exception>
    /// <exception cref="NullReferenceException">需要拆箱的操作数为 <see langword="null"/>.</exception>
    public static int CompareF(object value1, object value2, NumberType numberType)
    {
        return numberType switch
        {
            NumberType.SByte => ((sbyte)value1).CompareTo((sbyte)value2),
            NumberType.Byte => ((byte)value1).CompareTo((byte)value2),
            NumberType.Int16 => ((short)value1).CompareTo((short)value2),
            NumberType.UInt16 => ((ushort)value1).CompareTo((ushort)value2),
            NumberType.Int32 => ((int)value1).CompareTo((int)value2),
            NumberType.UInt32 => ((uint)value1).CompareTo((uint)value2),
            NumberType.Int64 => ((long)value1).CompareTo((long)value2),
            NumberType.UInt64 => ((ulong)value1).CompareTo((ulong)value2),
            NumberType.Single => ((float)value1).CompareTo((float)value2),
            NumberType.Double => ((double)value1).CompareTo((double)value2),
            NumberType.Decimal => ((decimal)value1).CompareTo((decimal)value2),
            NumberType.IntPtr => ((nint)value1).CompareTo((nint)value2),
            NumberType.UIntPtr => ((nuint)value1).CompareTo((nuint)value2),
            NumberType.Int128 => ((Int128)value1).CompareTo((Int128)value2),
            NumberType.UInt128 => ((UInt128)value1).CompareTo((UInt128)value2),
            NumberType.Half => ((Half)value1).CompareTo((Half)value2),
            NumberType.BigInteger => ((BigInteger)value1).CompareTo((BigInteger)value2),
            _ => throw CreateUnsupportedNumberTypeException(numberType),
        };
    }
}
