using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace HKW.HKWUtils;

/// <summary>
/// 可观测循环迭代器
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Index = {CurrentIndex}, Current = {Current}")]
public class ObservableCyclicEnumerator<T> : IEnumerator<T>, INotifyPropertyChanged
{
    private readonly IEnumerable<T> _enumerable;

    private IEnumerator<T> _enumerator;

    /// <inheritdoc/>
    public T Current => _enumerator.Current;

    /// <inheritdoc/>
    object IEnumerator.Current => Current!;

    /// <summary>
    /// 当前项目索引
    /// </summary>
    public int CurrentIndex { get; private set; }

    /// <summary>
    /// 自动重置, 在迭代结束后自动重置迭代器进行下一个循环
    /// </summary>
    public bool AutoReset { get; set; }

    /// <inheritdoc/>
    /// <param name="enumerable">枚举器</param>
    /// <param name="autoReset">自动重置</param>
    public ObservableCyclicEnumerator(IEnumerable<T> enumerable, bool autoReset = false)
    {
        _enumerable = enumerable;
        _enumerator = enumerable.GetEnumerator();
        AutoReset = autoReset;
        CurrentIndex = -1;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        var result = _enumerator.MoveNext();
        if (AutoReset && result is false)
        {
            Reset();
            result = _enumerator.MoveNext();
        }

        if (result)
        {
            CurrentIndex++;
            PropertyChanged?.Invoke(this, new(nameof(Current)));
            PropertyChanged?.Invoke(this, new(nameof(CurrentIndex)));
        }

        return result;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        try
        {
            CurrentIndex = -1;
            _enumerator.Reset();
        }
        catch
        {
            _enumerator.Dispose();
            _enumerator = _enumerable.GetEnumerator();
        }
    }

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableCyclicEnumerator()
    {
        //必须为false
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        //必须为true
        Dispose(true);
        //通知垃圾回收器不再调用终结器
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _enumerator.Dispose();
        }
        _disposed = true;
    }
    #endregion


    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

/// <summary>
/// 可观测循环迭代器
/// </summary>
public static class ObservableCyclicEnumerator
{
    /// <summary>
    /// 创建可观测循环迭代器
    /// </summary>
    /// <param name="enumerable">枚举</param>
    /// <param name="autoReset">自动重置</param>
    /// <typeparam name="T">项目类型</typeparam>
    /// <returns>可观测循环迭代器</returns>
    public static ObservableCyclicEnumerator<T> Create<T>(
        IEnumerable<T> enumerable,
        bool autoReset = false
    )
    {
        return new ObservableCyclicEnumerator<T>(enumerable, autoReset);
    }
}
