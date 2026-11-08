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
/// <remarks>
/// 成功移动时先通知 Current, 再按实际旧新索引决定是否通知 CurrentIndex.
/// 重置仅通知实际变化的 CurrentIndex, 此时 Current 无效, 回调不能把它当作有效项目读取.
/// 自动循环仅通知完成后的最终状态, 不通知重置过程中的中间位置.
/// 重置仅对 NotSupportedException 回退重新枚举; 重建失败后可再次调用 Reset 重试.
/// </remarks>
[DebuggerDisplay("Index = {CurrentIndex}, Current = {Current}")]
public class ObservableCyclicEnumerator<T> : IEnumerator<T>, INotifyPropertyChanged
{
    private CyclicEnumerator<T> _enumerator;

    /// <inheritdoc/>
    /// <remarks>
    /// 仅成功移动后有效; 重置, 耗尽或移动异常后抛出 InvalidOperationException, 释放后抛出 ObjectDisposedException.
    /// </remarks>
    public T Current => _enumerator.Current;

    /// <inheritdoc/>
    object IEnumerator.Current => Current!;

    /// <summary>
    /// 当前项目索引
    /// </summary>
    /// <remarks>
    /// 记录本轮最后一次成功移动的索引; 初始及重置后为 -1. 耗尽后保留最后索引, 不代表 Current 仍有效.
    /// </remarks>
    public int CurrentIndex => _enumerator.CurrentIndex;

    /// <summary>
    /// 自动重置, 在迭代结束后自动重置迭代器进行下一个循环
    /// </summary>
    public bool AutoReset
    {
        get => _enumerator.AutoReset;
        set => _enumerator.AutoReset = value;
    }

    /// <inheritdoc/>
    /// <param name="enumerable">枚举器</param>
    /// <param name="autoReset">自动重置</param>
    public ObservableCyclicEnumerator(IEnumerable<T> enumerable, bool autoReset)
    {
        _enumerator = new CyclicEnumerator<T>(enumerable, autoReset);
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        var previousIndex = CurrentIndex;
        try
        {
            var result = _enumerator.MoveNext();
            if (result)
                PropertyChanged?.Invoke(this, new(nameof(Current)));
            return result;
        }
        finally
        {
            if (previousIndex != CurrentIndex)
                PropertyChanged?.Invoke(this, new(nameof(CurrentIndex)));
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        var previousIndex = CurrentIndex;
        try
        {
            _enumerator.Reset();
        }
        finally
        {
            if (previousIndex != CurrentIndex)
                PropertyChanged?.Invoke(this, new(nameof(CurrentIndex)));
        }
    }

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableCyclicEnumerator()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
        if (disposing)
        {
            _enumerator.Dispose();
        }
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
