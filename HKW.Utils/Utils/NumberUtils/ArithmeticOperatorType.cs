namespace HKW.HKWUtils.Utils;

/// <summary>
/// 计算运算符类型
/// </summary>
public enum ArithmeticOperatorType
{
    /// <summary>
    /// 加法
    /// </summary>
    Addition,

    /// <summary>
    /// 减法
    /// </summary>
    Subtraction,

    /// <summary>
    /// 乘法
    /// </summary>
    Multiply,

    /// <summary>
    /// 除法
    /// </summary>
    Division,

    /// <summary>
    /// 取余
    /// </summary>
    Modulus,

    /// <summary>
    /// 按位或
    /// <para>只支持整数</para>
    /// </summary>
    BitwiseOr,

    /// <summary>
    /// 按位与
    /// <para>只支持整数</para>
    /// </summary>
    BitwiseAnd
}
