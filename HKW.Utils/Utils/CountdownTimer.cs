using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [DefaultValue(typeof(TimeSpan), nameof(TimeSpan.Zero))]
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// 已停止
    /// </summary>
    [DefaultValue(false)]
    public bool IsCancel { get; private set; }

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
    ///自动重置
    /// </summary>
    [DefaultValue(false)]
    public bool AutoReset { get; set; }

    private CancellationTokenSource _cts = new();

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
        if (IsRunning || IsCompleted)
            return;
        IsRunning = true;
        IsCancel = false;
        try
        {
            await Task.Delay(duration, _cts.Token);
        }
        catch { }
        finally
        {
            if (IsCancel is false)
            {
                TimeUp?.Invoke();
            }
            if (AutoReset)
            {
                Reset();
            }
            else
            {
                IsCompleted = true;
                IsRunning = false;
                _cts = new();
            }
        }
    }

    /// <summary>
    /// 取消倒计时
    /// </summary>
    public void Cancel()
    {
        if (IsRunning)
        {
            IsCancel = true;
            _cts.Cancel();
            TimeCancel?.Invoke();
        }
    }

    // TODO: 倒计时中止
    //public void Stop()
    //{
    //    return;
    //}

    // TODO: 倒计时继续
    //public void Continue()
    //{
    //    return;
    //}

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        IsCompleted = false;
        IsCancel = false;
        IsRunning = false;
        _cts = new();
    }

    /// <summary>
    /// 倒计时结束事件
    /// </summary>
    public event CountdownHandler? TimeUp;

    /// <summary>
    /// 倒计时取消事件
    /// </summary>
    public event CountdownHandler? TimeCancel;

    /// <summary>
    /// 倒计时委托
    /// </summary>
    public delegate void CountdownHandler();
}
