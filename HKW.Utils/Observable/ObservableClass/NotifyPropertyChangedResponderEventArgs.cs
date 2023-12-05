namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知属性更改至事件参数
/// </summary>
public class NotifyPropertyChangedResponderEventArgs : EventArgs
{
    /// <summary>
    /// 通知的属性名称
    /// </summary>
    public HashSet<string> PropertyNames { get; }

    /// <inheritdoc/>
    public NotifyPropertyChangedResponderEventArgs(HashSet<string> propertyNames)
    {
        PropertyNames = propertyNames;
    }
}
