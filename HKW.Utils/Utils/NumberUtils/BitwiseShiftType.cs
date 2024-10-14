namespace HKW.HKWUtils;

/// <summary>
/// 二进制位移类型
/// </summary>
public enum BitwiseShiftType
{
    /// <summary>
    /// 左位移 (<see langword="&lt;&lt;"/>)
    /// </summary>
    Left,

    /// <summary>
    /// 右位移 (<see langword="&gt;&gt;"/>)
    /// </summary>
    Right,

    /// <summary>
    /// 无符号右位移 (<see langword="&gt;&gt;&gt;"/>)
    /// </summary>
    UnsignedRight
}
