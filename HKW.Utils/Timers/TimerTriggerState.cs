using System.ComponentModel;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 定时器状态
/// </summary>
public class TimerTriggerState : EventArgs
{
    /// <summary>
    /// 触发次数
    /// </summary>
    public int Count { get; internal set; } = 0;

    /// <summary>
    /// 重置
    /// </summary>
    internal void Reset()
    {
        Count = 0;
    }
}
