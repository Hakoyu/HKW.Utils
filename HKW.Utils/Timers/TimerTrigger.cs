using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 定时触发器
/// </summary>
public class TimerTrigger : IDisposable, IAsyncDisposable
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
    public TimeSpan Elapsed => _stopWatch.Elapsed;

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
    private readonly Stopwatch _stopWatch = new();

    #region Ctor
    /// <inheritdoc/>
    public TimerTrigger() { }

    /// <inheritdoc/>
    /// <param name="dueTimeMillisecond">启动延迟 (单位: ms)</param>
    /// <param name="periodMillisecond">触发间隔 (单位: ms)</param>
    public TimerTrigger(int dueTimeMillisecond, int periodMillisecond)
        : this(
            TimeSpan.FromMilliseconds(dueTimeMillisecond),
            TimeSpan.FromMilliseconds(periodMillisecond)
        ) { }

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
    /// 按设定的 <see cref="DueTime"/> 和 <see cref="Period"/> 启动定时触发器
    /// </summary>
    /// <exception cref="Exception">正在运行</exception>
    public void Start()
    {
        Start(DueTime, Period);
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="dueTimeMillisecond">启动延迟 (单位: ms)</param>
    /// <param name="periodMillisecond">触发间隔 (单位: ms)</param>
    /// <exception cref="Exception">正在运行</exception>
    public void Start(int dueTimeMillisecond, int periodMillisecond)
    {
        Start(
            TimeSpan.FromMilliseconds(dueTimeMillisecond),
            TimeSpan.FromMilliseconds(periodMillisecond)
        );
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="dueTime">启动延迟</param>
    /// <param name="period">触发间隔</param>
    /// <exception cref="Exception">正在运行</exception>
    public void Start(TimeSpan dueTime, TimeSpan period)
    {
        if (period == TimeSpan.Zero)
            throw new ArgumentException(
                $"{ExceptionMessage.CannotBe} {TimeSpan.Zero}",
                nameof(period)
            );
        if (IsRunning)
            throw new Exception(ExceptionMessage.IsRunning);
        Reset();
        IsRunning = true;
        LastDueTime = dueTime;
        LastPeriod = period;
        _stopWatch.Start();
        _timer = new Timer(new TimerCallback(TimerTask), State, dueTime, period);
    }

    /// <summary>
    /// 按上次启动继续
    /// </summary>
    /// <exception cref="Exception">从未开始或正在运行</exception>
    public void Continue()
    {
        if (LastDueTime is null || LastPeriod is null)
            throw new Exception(ExceptionMessage.NeverStart);
        if (IsRunning)
            throw new Exception(ExceptionMessage.IsRunning);
        IsRunning = true;
        _stopWatch.Start();
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
        _timer?.Dispose();
        _stopWatch.Stop();
        IsRunning = false;
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        State.Reset();
        _stopWatch.Reset();
    }

    private void TimerTask(object? timerState)
    {
        if (timerState is not TimerTriggerState state)
        {
            _timer?.Dispose();
            throw new ArgumentException(
                $"{ExceptionMessage.NotBe} {nameof(TimerTriggerState)}",
                nameof(timerState)
            );
        }
        var count = state.Count;
        Interlocked.Increment(ref count);
        state.Count = count;
        TimedTrigger?.Invoke(this, state);
    }

    /// <summary>
    /// 定时触发事件
    /// </summary>
    public event TimeTriggerEventHandler? TimedTrigger;

    #region IDisposable
    private bool _disposedValue;

    /// <inheritdoc/>
    ~TimerTrigger() => Dispose(false);

    /// <inheritdoc/>

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (disposing)
        {
            _timer?.Dispose();
        }
        _disposedValue = true;
    }

    /// <inheritdoc/>

    public async ValueTask DisposeAsync()
    {
        if (_timer is not null)
            await _timer.DisposeAsync();
        Dispose(false);
        GC.SuppressFinalize(this);
    }
    #endregion
}
