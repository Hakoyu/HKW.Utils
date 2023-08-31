using HKW.HKWUtils.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Utils;

/// <summary>
/// 定时触发器
/// </summary>
public class TimerTrigger
{
    /// <summary>
    /// 触发器间隔
    /// </summary>
    [DefaultValue(typeof(TimeSpan), nameof(TimeSpan.Zero))]
    public TimeSpan Period { get; set; }

    /// <summary>
    /// 触发器启动延迟
    /// </summary>

    [DefaultValue(typeof(TimeSpan), nameof(TimeSpan.Zero))]
    public TimeSpan DueTime { get; set; }

    /// <summary>
    /// 触发器状态
    /// </summary>
    public TimerState CurrentState { get; private set; } = new();

    /// <summary>
    /// 已停止
    /// </summary>
    [DefaultValue(false)]
    public bool IsStop { get; private set; }

    private Timer? _timer;

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="dueTime">启动延迟 (单位: ms)</param>
    /// <param name="period">触发间隔 (单位: ms)</param>
    public void Start(int dueTime, int period)
    {
        Reset();
        _timer = new Timer(new TimerCallback(TimerTask), CurrentState, dueTime, period);
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        IsStop = true;
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        IsStop = false;
    }

    private void TimerTask(object? timerState)
    {
        if (IsStop)
            _timer?.Dispose();
        if (timerState is not TimerState state)
            return;
        Interlocked.Increment(ref state._counter);
        var cancelEventArgs = new CancelEventArgs();
        TimedTrigger?.Invoke(this, cancelEventArgs);
        if (cancelEventArgs.Cancel)
            _timer?.Dispose();
    }

    /// <summary>
    /// 定时触发事件
    /// </summary>
    public event XCancelEventHandler<TimerTrigger, CancelEventArgs>? TimedTrigger;

    /// <summary>
    /// 定时器状态
    /// </summary>
    public class TimerState
    {
        /// <summary>
        /// 触发次数
        /// </summary>
        public int Counter => _counter;

        [DefaultValue(0)]
        internal int _counter;
    }
}
