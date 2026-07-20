using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 并发双向字典
/// </summary>
/// <typeparam name="T1">项目类型1</typeparam>
/// <typeparam name="T2">项目类型2</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ConcurrentBidirectionalDictionary<T1, T2>
    : IDictionary<T1, T2>,
        IReadOnlyDictionary<T1, T2>,
        IDisposable
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> _dictionary1;
    private readonly Dictionary<T2, T1> _dictionary2;
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    /// <inheritdoc/>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public ConcurrentBidirectionalDictionary(
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
        : this(16, comparer1, comparer2) { }

    /// <inheritdoc/>
    /// <param name="capacity">初始容量</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public ConcurrentBidirectionalDictionary(
        int capacity,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public ConcurrentBidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        var capacity = keyValuePairs is ICollection<KeyValuePair<T1, T2>> collection
            ? collection.Count
            : 16;
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);
        foreach (var kv in keyValuePairs)
        {
            _dictionary1.Add(kv.Key, kv.Value);
            _dictionary2.Add(kv.Value, kv.Key);
        }
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public ConcurrentBidirectionalDictionary(
        IEnumerable<(T1, T2)> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        var capacity = keyValuePairs is ICollection<(T1, T2)> collection ? collection.Count : 16;
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);

        foreach (var kv in keyValuePairs)
        {
            _dictionary1.Add(kv.Item1, kv.Item2);
            _dictionary2.Add(kv.Item2, kv.Item1);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public ICollection<T1> Keys => GetKeysSnapshot();

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public ICollection<T2> Values => GetValuesSnapshot();

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public int Count
    {
        get
        {
            ThrowIfDisposed();
            _lock.EnterReadLock();
            try
            {
                return _dictionary1.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// 字典1只读快照
    /// </summary>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public ReadOnlyDictionary<T1, T2> Dictionary1
    {
        get
        {
            ThrowIfDisposed();
            _lock.EnterReadLock();
            try
            {
                return new ReadOnlyDictionary<T1, T2>(
                    new Dictionary<T1, T2>(_dictionary1, _dictionary1.Comparer)
                );
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// 字典2只读快照
    /// </summary>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public ReadOnlyDictionary<T2, T1> Dictionary2
    {
        get
        {
            ThrowIfDisposed();
            _lock.EnterReadLock();
            try
            {
                return new ReadOnlyDictionary<T2, T1>(
                    new Dictionary<T2, T1>(_dictionary2, _dictionary2.Comparer)
                );
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => GetKeysSnapshot();

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => GetValuesSnapshot();

    #region Dictionary1

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public T2 this[T1 key]
    {
        get
        {
            ThrowIfDisposed();
            _lock.EnterReadLock();
            try
            {
                return _dictionary1[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <inheritdoc/>
    /// <exception cref="UseAlternativeMethodException">使用代替方法 <see cref="TryAdd(T1, T2)"/></exception>
    void IDictionary<T1, T2>.Add(T1 key, T2 value)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <inheritdoc/>
    /// <exception cref="UseAlternativeMethodException">使用代替方法 <see cref="TryAdd(KeyValuePair{T1, T2})"/></exception>
    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TrySetValue(T1 key, T2 value)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            ref var d1ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary1, key);
            if (Unsafe.IsNullRef(ref d1ValueRef))
                return false;
            var d1Value = d1ValueRef;
            if (_dictionary2.ContainsKey(value))
                return false;
            if (
                _dictionary2.TryGetValue(d1Value, out var d2Value) is false
                || _dictionary1.Comparer.Equals(d2Value, key) is false
            )
                return false;

            d1ValueRef = value;
            _dictionary2.Remove(d1Value);
            _dictionary2.Add(value, key);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TryAdd(T1 key, T2 value)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            var result = _dictionary1.TryAdd(key, value);
            if (result is false)
                return false;

            if (_dictionary2.TryAdd(value, key) is false)
            {
                _dictionary1.Remove(key);
                return false;
            }

            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TryAdd(KeyValuePair<T1, T2> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Remove(T1 key)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            if (_dictionary1.Remove(key, out var value) is false)
                return false;

            _dictionary2.Remove(value);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public void Clear()
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            _dictionary1.Clear();
            _dictionary2.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Contains(KeyValuePair<T1, T2> item)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool ContainsKey(T1 key)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.ContainsKey(key);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).CopyTo(array, arrayIndex);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Remove(KeyValuePair<T1, T2> item)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            if (
                _dictionary1.TryGetValue(item.Key, out var value) is false
                || _dictionary2.Comparer.Equals(item.Value, value) is false
            )
                return false;

            _dictionary1.Remove(item.Key);
            _dictionary2.Remove(value);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.TryGetValue(key, out value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T1, T2>>)GetSnapshot()).GetEnumerator();
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Dictionary2

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public T1 this[T2 key]
    {
        get
        {
            ThrowIfDisposed();
            _lock.EnterReadLock();
            try
            {
                return _dictionary2[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TrySetValue(T2 key, T1 value)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            ref var d2ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary2, key);
            if (Unsafe.IsNullRef(ref d2ValueRef))
                return false;
            var d2Value = d2ValueRef;
            if (_dictionary1.ContainsKey(value))
                return false;
            if (
                _dictionary1.TryGetValue(d2Value, out var d1Value) is false
                || _dictionary2.Comparer.Equals(d1Value, key) is false
            )
                return false;

            d2ValueRef = value;
            _dictionary1.Remove(d2Value);
            _dictionary1.Add(value, key);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TryAdd(T2 key, T1 value)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            var result = _dictionary2.TryAdd(key, value);
            if (result is false)
                return false;

            if (_dictionary1.TryAdd(value, key) is false)
            {
                _dictionary2.Remove(key);
                return false;
            }

            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool TryAdd(KeyValuePair<T2, T1> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Remove(T2 key)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            if (_dictionary2.Remove(key, out var value) is false)
                return false;

            _dictionary1.Remove(value);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Remove(KeyValuePair<T2, T1> item)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            if (
                _dictionary2.TryGetValue(item.Key, out var value) is false
                || _dictionary1.Comparer.Equals(item.Value, value) is false
            )
                return false;

            _dictionary1.Remove(value);
            _dictionary2.Remove(item.Key);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool Contains(KeyValuePair<T2, T1> item)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public bool ContainsKey(T2 key)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.ContainsKey(key);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).CopyTo(array, arrayIndex);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value)
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.TryGetValue(key, out value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    #endregion

    #region Snapshot API

    /// <summary>
    /// 获取键值对快照
    /// </summary>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public KeyValuePair<T1, T2>[] GetSnapshot()
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.ToArray();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// 获取键快照
    /// </summary>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public T1[] GetKeysSnapshot()
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            var result = new T1[_dictionary1.Count];
            _dictionary1.Keys.CopyTo(result, 0);
            return result;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// 获取值快照
    /// </summary>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public T2[] GetValuesSnapshot()
    {
        ThrowIfDisposed();
        _lock.EnterReadLock();
        try
        {
            var result = new T2[_dictionary1.Count];
            _dictionary1.Values.CopyTo(result, 0);
            return result;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// 确保容量
    /// </summary>
    /// <param name="capacity">目标容量</param>
    /// <returns>最终容量</returns>
    /// <exception cref="ObjectDisposedException">对象已释放</exception>
    public int EnsureCapacity(int capacity)
    {
        ThrowIfDisposed();
        _lock.EnterWriteLock();
        try
        {
            var c1 = _dictionary1.EnsureCapacity(capacity);
            var c2 = _dictionary2.EnsureCapacity(capacity);
            return c1 >= c2 ? c1 : c2;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    #endregion

    #region Dispose

    private bool _disposed;

    /// <inheritdoc/>
    ~ConcurrentBidirectionalDictionary()
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
        if (Interlocked.Exchange(ref _disposed, true))
            return;

        if (disposing)
        {
            _lock.EnterWriteLock();
            try
            {
                _dictionary1.Clear();
                _dictionary2.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed), this);
    }

    #endregion
}
