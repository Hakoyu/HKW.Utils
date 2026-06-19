using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HKW.HKWUtils;

/// <summary>
/// 循环迭代器
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Index = {CurrentIndex}, Current = {Current}")]
public class CyclicEnumerator<T> : IEnumerator<T>
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
    public CyclicEnumerator(IEnumerable<T> enumerable, bool autoReset = false)
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
        CurrentIndex++;

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

    /// <summary>
    /// 为了防止忘记显式的调用Dispose方法
    /// </summary>
    ~CyclicEnumerator()
    {
        //必须为false
        Dispose(false);
    }

    /// <summary>
    /// 解除对引用列表注册的所有观测事件
    /// </summary>
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
}

/// <summary>
/// 循环迭代器
/// </summary>
public static class CyclicEnumerator
{
    /// <summary>
    /// 创建循环迭代器
    /// </summary>
    /// <param name="enumerable">枚举</param>
    /// <param name="autoReset">自动重置</param>
    /// <typeparam name="T">项目类型</typeparam>
    /// <returns>循环迭代器</returns>
    public static CyclicEnumerator<T> Create<T>(IEnumerable<T> enumerable, bool autoReset = false)
    {
        return new CyclicEnumerator<T>(enumerable, autoReset);
    }
}
