namespace HKW.HKWUtils;

/// <summary>
/// 计算运算符类型
/// </summary>
public enum ArithmeticOperatorType
{
    /// <summary>
    /// 加法 (<see langword="+"/>)
    /// </summary>
    Addition,

    /// <summary>
    /// 减法 (<see langword="-"/>)
    /// </summary>
    Subtraction,

    /// <summary>
    /// 乘法 (<see langword="*"/>)
    /// </summary>
    Multiply,

    /// <summary>
    /// 除法 (<see langword="/"/>)
    /// </summary>
    Division,

    /// <summary>
    /// 取余 (<see langword="%"/>)
    /// </summary>
    Modulus,

    /// <summary>
    /// 按位或 (<see langword="|"/>)
    /// <para>只支持整数</para>
    /// </summary>
    BitwiseOr,

    /// <summary>
    /// 按位与 (<see langword="&amp;"/>)
    /// <para>只支持整数</para>
    /// </summary>
    BitwiseAnd
}
