using System.ComponentModel;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 定时器状态
/// </summary>
public class TimerTriggerState
{
    [DefaultValue(0)]
    internal int _counter;

    /// <summary>
    /// 触发次数
    /// </summary>
    public int Counter => _counter;

    /// <summary>
    /// 重置
    /// </summary>
    internal void Reset()
    {
        _counter = 0;
    }
}
