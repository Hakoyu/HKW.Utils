namespace HKW.HKWUtils;

/// <summary>
/// 比较运算符类型
/// </summary>
public enum ComparisonOperatorType
{
    /// <summary>
    /// 相等 (<see langword="=="/>)
    /// </summary>
    Equality,

    /// <summary>
    /// 不相等 (<see langword="!="/>)
    /// </summary>
    Inequality,

    /// <summary>
    /// 小于 (<see langword="&lt;"/>)
    /// </summary>
    LessThan,

    /// <summary>
    /// 大于 (<see langword="&gt;"/>)
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 小于或等于 (<see langword="&lt;="/>)
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// 大于或等于 (<see langword="&gt;="/>)
    /// </summary>
    GreaterThanOrEqual,
}
