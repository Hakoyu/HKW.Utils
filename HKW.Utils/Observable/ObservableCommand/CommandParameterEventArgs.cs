namespace HKW.HKWUtils.Observable;

/// <summary>
/// 命令参数事件参数
/// </summary>
/// <typeparam name="T"></typeparam>
public class CommandParameterEventArgs<T> : EventArgs
{
    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; }

    /// <inheritdoc/>
    /// <param name="value">值</param>
    public CommandParameterEventArgs(T value)
    {
        Value = value;
    }
}
