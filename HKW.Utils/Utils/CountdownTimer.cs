using HKW.HKWUtils.Events;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 倒计时器
/// </summary>
public class CountdownTimer
{
    /// <summary>
    /// 倒计时持续时间
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// 正在运行
    /// </summary>
    [DefaultValue(false)]
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 已完成
    /// </summary>
    [DefaultValue(false)]
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// 运行时间
    /// </summary>
    public TimeSpan Elapsed => r_timer.Elapsed;

    /// <summary>
    /// 完成后自动重置
    /// </summary>
    [DefaultValue(false)]
    public bool AutoReset { get; set; }

    /// <summary>
    /// 上一次运行的倒计时持续时间
    /// </summary>
    public TimeSpan LastDuration { get; private set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TimerTrigger r_timer = new(0, 10);

    #region Ctor
    /// <inheritdoc/>
    public CountdownTimer()
        : this(TimeSpan.Zero) { }

    /// <inheritdoc/>
    /// <param name="duration">持续时间 (单位: ms)</param>
    public CountdownTimer(int duration)
        : this(TimeSpan.FromMilliseconds(duration)) { }

    /// <inheritdoc/>
    /// <param name="duration">持续时间</param>
    public CountdownTimer(TimeSpan duration)
    {
        Duration = duration;
        r_timer.TimedTrigger += TimedTrigger;
    }
    #endregion
    private void TimedTrigger(TimerTrigger args)
    {
        if (LastDuration <= Elapsed)
        {
            r_timer.Stop();
            Completed?.Invoke();
            if (AutoReset)
            {
                IsRunning = false;
                Reset();
            }
        }
    }

    #region Timer
    /// <summary>
    /// 启动定时器
    /// </summary>
    /// <exception cref="Exception">CountdownTimer is running</exception>
    public void Start()
    {
        Start(Duration);
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    /// <param name="duration">持续时间 (单位: ms)</param>
    /// <exception cref="Exception">CountdownTimer is running</exception>
    public void Start(int duration)
    {
        Start(TimeSpan.FromMilliseconds(duration));
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <exception cref="Exception">CountdownTimer is running</exception>
    public void Start(TimeSpan duration)
    {
        if (IsRunning)
            throw new Exception("CountdownTimer is running");
        LastDuration = duration;
        IsRunning = true;
        r_timer.Start();
    }

    /// <summary>
    /// 继续倒计时
    /// </summary>
    /// <exception cref="Exception">CountdownTimer never started</exception>
    /// <exception cref="Exception">Countdown completed</exception>
    /// <exception cref="Exception">CountdownTimer is running</exception>
    public void Continue()
    {
        if (LastDuration == TimeSpan.Zero)
            throw new Exception("CountdownTimer never started");
        if (IsCompleted)
            throw new Exception("Countdown completed");
        if (IsRunning)
            throw new Exception("CountdownTimer is running");
        IsRunning = true;
        r_timer.Continue();
    }

    /// <summary>
    /// 停止倒计时
    /// </summary>
    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            r_timer.Stop();
            Stopped?.Invoke();
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <exception cref="Exception">CountdownTimer is running</exception>
    public void Reset()
    {
        if (IsRunning)
            throw new Exception("CountdownTimer is running");
        IsCompleted = false;
        IsRunning = false;
    }
    #endregion

    /// <summary>
    /// 倒计时完成事件
    /// </summary>
    public event XEventHandler? Completed;

    /// <summary>
    /// 倒计时停止事件
    /// </summary>
    public event XEventHandler? Stopped;
}
