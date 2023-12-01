namespace HKW.HKWUtils.Observable;

/// <summary>
/// 命令参数事件参数
/// </summary>
/// <typeparam name="T"></typeparam>
public class CommandParameterArgs<T> : EventArgs
{
    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; }

    /// <inheritdoc/>
    /// <param name="value">值</param>
    public CommandParameterArgs(T value)
    {
        Value = value;
    }
}
