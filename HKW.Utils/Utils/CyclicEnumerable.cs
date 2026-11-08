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
/// <remarks>
/// 结构体复制共享底层游标, 但各副本的位置及释放标记独立, 不能交替使用或分别释放副本;
/// 需要独立枚举时应重新构造. 默认值未初始化, 读取当前位置, 移动及重置会抛出异常, 但可安全释放.
/// 重置仅在底层抛出 <see cref="NotSupportedException"/> 时释放并重新枚举;
/// 重建失败后可再次调用 Reset 重试, 不能继续使用已释放的底层迭代器.
/// </remarks>
[DebuggerDisplay("Index = {CurrentIndex}, Current = {Current}")]
public struct CyclicEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerable<T>? _enumerable;

    private IEnumerator<T>? _enumerator;
    private int _nextIndex;
    private bool _hasCurrent;
    private bool _disposed;

    /// <inheritdoc/>
    /// <remarks>
    /// 仅成功移动后有效; 重置, 耗尽或移动异常后抛出 InvalidOperationException, 释放后抛出 ObjectDisposedException.
    /// </remarks>
    public T Current
    {
        get
        {
            ThrowIfUnavailable();
            if (_hasCurrent is false)
                throw new InvalidOperationException("Enumeration has not started or has finished.");
            return _enumerator!.Current;
        }
    }

    /// <inheritdoc/>
    object IEnumerator.Current => Current!;

    /// <summary>
    /// 当前项目索引
    /// </summary>
    /// <remarks>
    /// 记录本轮最后一次成功移动的索引; 初始及重置后为 -1. 耗尽后保留最后索引, 不代表 Current 仍有效.
    /// </remarks>
    public int CurrentIndex => _nextIndex - 1;

    /// <summary>
    /// 自动重置, 在迭代结束后自动重置迭代器进行下一个循环
    /// </summary>
    public bool AutoReset { get; set; }

    /// <inheritdoc/>
    /// <param name="enumerable">枚举器</param>
    /// <param name="autoReset">自动重置</param>
    public CyclicEnumerator(IEnumerable<T> enumerable, bool autoReset = false)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        _enumerable = enumerable;
        _enumerator = enumerable.GetEnumerator();
        AutoReset = autoReset;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        ThrowIfUnavailable();
        _hasCurrent = false;
        var result = _enumerator!.MoveNext();
        if (AutoReset && result is false)
        {
            Reset();
            result = _enumerator!.MoveNext();
        }

        if (result)
        {
            _nextIndex++;
            _hasCurrent = true;
        }

        return result;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        ThrowIfUninitializedOrDisposed();
        _hasCurrent = false;
        _nextIndex = 0;
        if (_enumerator is null)
        {
            _enumerator = _enumerable!.GetEnumerator();
            return;
        }

        try
        {
            _enumerator.Reset();
        }
        catch (NotSupportedException)
        {
            var previous = _enumerator;
            _enumerator = null;
            previous.Dispose();
            _enumerator = _enumerable!.GetEnumerator();
        }
    }

    /// <summary>
    /// 解除对引用列表注册的所有观测事件
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _hasCurrent = false;
        var previous = _enumerator;
        _enumerator = null;
        previous?.Dispose();
    }

    private readonly void ThrowIfUninitializedOrDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CyclicEnumerator<T>));
        if (_enumerable is null)
            throw new InvalidOperationException("The cyclic enumerator has not been initialized.");
    }

    private readonly void ThrowIfUnavailable()
    {
        ThrowIfUninitializedOrDisposed();
        if (_enumerator is null)
            throw new InvalidOperationException(
                "The underlying enumerator is unavailable. Call Reset to recreate it."
            );
    }
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
