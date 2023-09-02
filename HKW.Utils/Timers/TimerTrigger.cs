using HKW.HKWUtils.Events;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 定时触发器
/// </summary>
public class TimerTrigger
{
    /// <summary>
    /// 触发器间隔
    /// </summary>
    public TimeSpan Period { get; set; }

    /// <summary>
    /// 触发器启动延迟
    /// </summary>
    public TimeSpan DueTime { get; set; }

    /// <summary>
    /// 上一次运行的延迟
    /// </summary>
    public TimeSpan? LastPeriod { get; private set; }

    /// <summary>
    /// 上一次运行的间隔
    /// </summary>
    public TimeSpan? LastDueTime { get; private set; }

    /// <summary>
    /// 运行时间
    /// </summary>
    public TimeSpan Elapsed => r_stopWatch.Elapsed;

    /// <summary>
    /// 触发器状态
    /// </summary>
    public TimerTriggerState State { get; private set; } = new();

    /// <summary>
    /// 正在运行
    /// </summary>
    [DefaultValue(false)]
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// 核心定时器
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Timer? _timer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Stopwatch r_stopWatch = new();

    #region Ctor
    /// <inheritdoc/>
    public TimerTrigger() { }

    /// <inheritdoc/>
    /// <param name="dueTime">启动延迟 (单位: ms)</param>
    /// <param name="period">触发间隔 (单位: ms)</param>
    public TimerTrigger(int dueTime, int period)
        : this(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period)) { }

    /// <inheritdoc/>
    /// <param name="dueTime">启动延迟</param>
    /// <param name="period">触发间隔</param>
    public TimerTrigger(TimeSpan dueTime, TimeSpan period)
    {
        DueTime = dueTime;
        Period = period;
    }
    #endregion

    /// <summary>
    /// 按设定的 <see cref="DueTime"/> 和 <see cref="Period"/> 启动
    /// </summary>
    /// <exception cref="Exception">TimerTrigger not stopped</exception>
    public void Start()
    {
        Start(DueTime, Period);
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="dueTime">启动延迟 (单位: ms)</param>
    /// <param name="period">触发间隔 (单位: ms)</param>
    /// <exception cref="Exception">TimerTrigger not stopped</exception>
    public void Start(int dueTime, int period)
    {
        Start(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="dueTime">启动延迟</param>
    /// <param name="period">触发间隔</param>
    /// <exception cref="Exception">TimerTrigger not stopped</exception>
    public void Start(TimeSpan dueTime, TimeSpan period)
    {
        if (period == TimeSpan.Zero)
            throw new ArgumentException("Cannot be Zero", nameof(period));
        if (IsRunning)
            throw new Exception("TimerTrigger not stopped");
        Reset();
        IsRunning = true;
        LastDueTime = dueTime;
        LastPeriod = period;
        r_stopWatch.Start();
        _timer = new Timer(new TimerCallback(TimerTask), State, dueTime, period);
    }

    /// <summary>
    /// 按上次启动继续
    /// </summary>
    /// <exception cref="Exception">TimerTrigger not stopped</exception>
    /// <exception cref="Exception">TimerTrigger never started</exception>
    public void Continue()
    {
        if (LastDueTime is null || LastPeriod is null)
            throw new Exception("TimerTrigger never started");
        if (IsRunning)
            throw new Exception("TimerTrigger not stopped");
        IsRunning = true;
        r_stopWatch.Start();
        _timer = new Timer(
            new TimerCallback(TimerTask),
            State,
            (TimeSpan)LastDueTime,
            (TimeSpan)LastPeriod
        );
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        r_stopWatch.Stop();
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        State.Reset();
        r_stopWatch.Reset();
    }

    private void TimerTask(object? timerState)
    {
        if (IsRunning is false)
            _timer?.Dispose();
        if (timerState is not TimerTriggerState state)
        {
            _timer?.Dispose();
            throw new ArgumentException($"Not {nameof(TimerTriggerState)}", nameof(timerState));
        }
        Interlocked.Increment(ref state._counter);
        TimedTrigger?.Invoke(this);
        if (IsRunning is false)
            _timer?.Dispose();
    }

    /// <summary>
    /// 定时触发事件
    /// </summary>
    public event XEventHandler<TimerTrigger>? TimedTrigger;
}
