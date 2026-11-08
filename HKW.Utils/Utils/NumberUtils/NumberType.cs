namespace HKW.HKWUtils;

/// <summary>
/// 数值类型
/// </summary>
public enum NumberType
{
    /// <summary>
    /// 8 位有符号整数 (<see cref="sbyte"/>)
    /// <para>范围: -128 到 127</para>
    /// <para>大小: 1 个字节</para>
    /// </summary>
    SByte,

    /// <summary>
    /// 8 位无符号整数 (<see cref="byte"/>)
    /// <para>范围: 0 到 255</para>
    /// <para>大小: 1 个字节</para>
    /// </summary>
    Byte,

    /// <summary>
    /// 16 位有符号整数 (<see cref="short"/>)
    /// <para>范围: -32,768 到 32,767</para>
    /// <para>大小: 2 个字节</para>
    /// </summary>
    Int16,

    /// <summary>
    /// 16 位无符号整数 (<see cref="ushort"/>)
    /// <para>范围: 0 到 65,535</para>
    /// <para>大小: 2 个字节</para>
    /// </summary>
    UInt16,

    /// <summary>
    /// 32 位有符号整数 (<see cref="int"/>)
    /// <para>范围: -2,147,483,648 到 2,147,483,647</para>
    /// <para>大小: 4 个字节</para>
    /// </summary>
    Int32,

    /// <summary>
    /// 32 位无符号整数 (<see cref="uint"/>)
    /// <para>范围: 0 到 4,294,967,295</para>
    /// <para>大小: 4 个字节</para>
    /// </summary>
    UInt32,

    /// <summary>
    /// 64 位有符号整数 (<see cref="long"/>)
    /// <para>范围: -9,223,372,036,854,775,808 到 9,223,372,036,854,775,807</para>
    /// <para>大小: 8 个字节</para>
    /// </summary>
    Int64,

    /// <summary>
    /// 64 位无符号整数 (<see cref="ulong"/>)
    /// <para>范围: 0 到 18,446,744,073,709,551,615</para>
    /// <para>大小: 8 个字节</para>
    /// </summary>
    UInt64,

    /// <summary>
    /// 单精度 浮点数 (<see cref="float"/>)
    /// <para>范围: ±1.5 x 10 ^ -45 到 ±3.4 x 10 ^ 38</para>
    /// <para>大小: 4 个字节</para>
    /// <para>精度: 大约 6-9 位数</para>
    /// </summary>
    Single,

    /// <summary>
    /// 双精度 浮点数 (<see cref="double"/>)
    /// <para>范围: ±5.0 × 10 ^ −324 到 ±1.7 × 10 ^ 308</para>
    /// <para>大小: 8 个字节</para>
    /// <para>精度: 大约 15-17 位数</para>
    /// </summary>
    Double,

    /// <summary>
    /// 高精度 十进制浮点数 (<see cref="decimal"/>)
    /// <para>范围: ±1.0 x 10 ^ -28 到 ±7.9228 x 10 ^ 28</para>
    /// <para>大小: 16 个字节</para>
    /// <para>精度: 大约 28-29 位数</para>
    /// </summary>
    Decimal,

    /// <summary>
    /// 原生大小有符号整数 (<see cref="nint"/>), 位宽由当前进程决定.
    /// <para>范围: 32 位进程为 -2^31 到 2^31 - 1；64 位进程为 -2^63 到 2^63 - 1</para>
    /// <para>大小: 32 位进程为 4 个字节；64 位进程为 8 个字节</para>
    /// </summary>
    IntPtr,

    /// <summary>
    /// 原生大小无符号整数 (<see cref="nuint"/>), 位宽由当前进程决定.
    /// <para>范围: 32 位进程为 0 到 2^32 - 1；64 位进程为 0 到 2^64 - 1</para>
    /// <para>大小: 32 位进程为 4 个字节；64 位进程为 8 个字节</para>
    /// </summary>
    UIntPtr,

    /// <summary>
    /// 128 位有符号整数 (<see cref="System.Int128"/>).
    /// <para>范围: -2^127 到 2^127 - 1</para>
    /// <para>大小: 16 个字节</para>
    /// </summary>
    Int128,

    /// <summary>
    /// 128 位无符号整数 (<see cref="System.UInt128"/>).
    /// <para>范围: 0 到 2^128 - 1</para>
    /// <para>大小: 16 个字节</para>
    /// </summary>
    UInt128,

    /// <summary>
    /// 半精度二进制浮点数 (<see cref="System.Half"/>).
    /// <para>范围: 有限值为 -65,504 到 65,504；最小正非零值约为 5.96 × 10^-8</para>
    /// <para>大小: 2 个字节</para>
    /// <para>精度: 约 3 位十进制有效数字；最多需要 5 位十进制数字保证往返转换</para>
    /// </summary>
    Half,

    /// <summary>
    /// 任意精度整数 (<see cref="System.Numerics.BigInteger"/>).
    /// <para>范围: 没有固定上下界，受内存和实现限制</para>
    /// <para>大小: 可变，取决于数值和内部表示</para>
    /// <para>精度: 整数精度不受固定位数限制</para>
    /// </summary>
    BigInteger,
}
