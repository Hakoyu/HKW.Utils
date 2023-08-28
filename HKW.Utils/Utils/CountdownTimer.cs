using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Utils;

/// <summary>
/// 倒计时器
/// </summary>
public class CountdownTimer
{
    /// <summary>
    /// 倒计时
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;

    private CancellationTokenSource _cts = new();

    /// <summary>
    /// 已停止
    /// </summary>
    public bool IsStop { get; private set; } = false;

    /// <summary>
    /// 正在运行
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    /// <inheritdoc/>
    public CountdownTimer() { }

    /// <inheritdoc/>
    /// <param name="milliseconds">毫秒时间</param>
    public CountdownTimer(int milliseconds)
    {
        Duration = TimeSpan.FromMilliseconds(milliseconds);
    }

    /// <inheritdoc/>
    /// <param name="duration">持续时间</param>
    public CountdownTimer(TimeSpan duration)
    {
        Duration = duration;
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    public async void Start()
    {
        await Countdown(Duration);
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    /// <param name="milliseconds">毫秒时间</param>
    public async void Start(int? milliseconds = null)
    {
        var currentDuration = Duration;
        if (milliseconds is int ms)
            currentDuration = TimeSpan.FromMilliseconds(ms);
        await Countdown(currentDuration);
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    /// <param name="duration">持续时间</param>
    public async void Start(TimeSpan? duration = null)
    {
        var currentDuration = Duration;
        if (duration is TimeSpan span)
            currentDuration = span;
        await Countdown(currentDuration);
    }

    /// <summary>
    /// 倒计时
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <returns></returns>
    private async Task Countdown(TimeSpan duration)
    {
        if (IsRunning)
            return;
        IsRunning = true;
        IsStop = false;
        try
        {
            await Task.Delay(duration, _cts.Token);
        }
        catch { }
        finally
        {
            if (IsStop is false)
                TimeUp?.Invoke();
            IsRunning = false;
            _cts = new();
        }
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void Stop()
    {
        if (IsRunning)
        {
            IsStop = true;
            _cts.Cancel();
            TimeStop?.Invoke();
        }
    }

    /// <summary>
    /// 时间结束事件
    /// </summary>
    public event TimeUpHandler? TimeUp;

    /// <summary>
    /// 时间停止事件
    /// </summary>
    public event TimeStopHandler? TimeStop;

    /// <summary>
    /// 时间结束
    /// </summary>
    public delegate void TimeUpHandler();

    /// <summary>
    /// 时间停止
    /// </summary>
    public delegate void TimeStopHandler();
}
