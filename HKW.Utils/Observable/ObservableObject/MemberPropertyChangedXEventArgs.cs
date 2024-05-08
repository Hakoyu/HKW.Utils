namespace HKW.HKWUtils.Observable;

/// <summary>
/// 成员属性改变后事件参数
/// </summary>
public class MemberPropertyChangedXEventArgs
{
    /// <summary>
    /// 成员名称
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// 属性名称
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// 旧值
    /// </summary>
    public object? OldValue { get; }

    /// <summary>
    /// 新值
    /// </summary>
    public object? NewValue { get; }

    /// <inheritdoc/>
    /// <param name="memberName">成员名称</param>
    /// <param name="memberPropertyName">成员改变的属性名称</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    public MemberPropertyChangedXEventArgs(
        string memberName,
        string memberPropertyName,
        object? oldValue,
        object? newValue
    )
    {
        MemberName = memberName;
        PropertyName = memberPropertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
