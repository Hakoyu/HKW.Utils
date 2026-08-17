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
public sealed class ConcurrentBidirectionalDictionary<T1, T2>
    : IBidirectionalDictionary<T1, T2>,
        IReadOnlyBidirectionalDictionary<T1, T2>,
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
    {
        _dictionary1 = new(comparer1);
        _dictionary2 = new(comparer2);
    }

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
    /// <param name="pairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public ConcurrentBidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> pairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(pairs);

        if (pairs is ICollection<KeyValuePair<T1, T2>> c)
        {
            _dictionary1 = new(c.Count, comparer1);
            _dictionary2 = new(c.Count, comparer2);
        }
        else
        {
            _dictionary1 = new(comparer1);
            _dictionary2 = new(comparer2);
        }
        foreach (var pair in pairs)
        {
            _dictionary1.Add(pair.Key, pair.Value);
            _dictionary2.Add(pair.Value, pair.Key);
        }
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => GetKeysSnapshot();

    /// <inheritdoc/>
    public ICollection<T2> Values => GetValuesSnapshot();

    /// <inheritdoc/>
    public int Count
    {
        get
        {
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

    /// <inheritdoc/>
    public IDictionary<T1, T2> Dictionary1
    {
        get
        {
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

    /// <inheritdoc/>
    public IDictionary<T2, T1> Dictionary2
    {
        get
        {
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
    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => GetKeysSnapshot();

    /// <inheritdoc/>
    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => GetValuesSnapshot();

    #region Dictionary1

    /// <inheritdoc/>
    public T2 this[T1 key1]
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _dictionary1[key1];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <inheritdoc/>
    void IDictionary<T1, T2>.Add(T1 key1, T2 value2)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item1)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <inheritdoc/>
    public bool TryAdd(T1 key1, T2 value2)
    {
        _lock.EnterWriteLock();
        try
        {
            var result = _dictionary1.TryAdd(key1, value2);
            if (result is false)
                return false;

            if (_dictionary2.TryAdd(value2, key1) is false)
            {
                _dictionary1.Remove(key1);
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
    public bool TrySetValue(T1 key1, T2 value2)
    {
        _lock.EnterWriteLock();
        try
        {
            ref var d1ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary1, key1);
            if (Unsafe.IsNullRef(ref d1ValueRef))
            {
                // 3
                if (_dictionary2.ContainsKey(value2))
                    return false;
                // 1
                _dictionary1.Add(key1, value2);
                _dictionary2.Add(value2, key1);
                return true;
            }
            else
            {
                var d1Value = d1ValueRef;
                // 4, 如果 d1Value 和 value 相等, 证明 dic2 存在 (value, key)
                if (_dictionary2.Comparer.Equals(d1Value, value2))
                    return true;
                // 3
                if (_dictionary2.ContainsKey(value2))
                    return false;
                // 2
                d1ValueRef = value2;
                _dictionary2.Remove(d1Value);
                _dictionary2.Add(value2, key1);
                return true;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public bool Remove(T1 key1)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_dictionary1.Remove(key1, out var value) is false)
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
    public bool Remove(KeyValuePair<T1, T2> item1)
    {
        _lock.EnterWriteLock();
        try
        {
            if (
                _dictionary1.TryGetValue(item1.Key, out var value) is false
                || _dictionary2.Comparer.Equals(item1.Value, value) is false
            )
                return false;

            _dictionary1.Remove(item1.Key);
            _dictionary2.Remove(value);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public void Clear()
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

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item1)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.TryGetValue(item1.Key, out var value2)
                && _dictionary2.Comparer.Equals(item1.Value, value2);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public bool ContainsKey(T1 key1)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.ContainsKey(key1);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T1, T2>[] array1, int arrayIndex)
    {
        _lock.EnterReadLock();
        try
        {
            ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).CopyTo(array1, arrayIndex);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public bool TryGetValue(T1 key1, [MaybeNullWhen(false)] out T2 value)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary1.TryGetValue(key1, out value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T1, T2>>)GetSnapshot()).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Dictionary2

    /// <inheritdoc/>
    public T1 this[T2 key2]
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _dictionary2[key2];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <inheritdoc/>
    public bool TrySetValue(T2 key2, T1 value1)
    {
        _lock.EnterWriteLock();
        try
        {
            ref var d2ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary2, key2);
            if (Unsafe.IsNullRef(ref d2ValueRef))
            {
                // 3
                if (_dictionary1.ContainsKey(value1))
                    return false;
                // 1
                _dictionary2.Add(key2, value1);
                _dictionary1.Add(value1, key2);
                return true;
            }
            else
            {
                var d2Value = d2ValueRef;
                // 4, 如果 d2Value 和 value 相等, 证明 dic1 存在 (value, key)
                if (_dictionary1.Comparer.Equals(d2Value, value1))
                    return true;
                // 3
                if (_dictionary1.ContainsKey(value1))
                    return false;
                // 2
                d2ValueRef = value1;
                _dictionary1.Remove(d2Value);
                _dictionary1.Add(value1, key2);
                return true;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public bool TryAdd(T2 key2, T1 value1)
    {
        _lock.EnterWriteLock();
        try
        {
            var result = _dictionary2.TryAdd(key2, value1);
            if (result is false)
                return false;

            if (_dictionary1.TryAdd(value1, key2) is false)
            {
                _dictionary2.Remove(key2);
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
    public bool Remove(T2 key2)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_dictionary2.Remove(key2, out var value) is false)
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
    public bool Remove(KeyValuePair<T2, T1> item2)
    {
        _lock.EnterWriteLock();
        try
        {
            if (
                _dictionary2.TryGetValue(item2.Key, out var value) is false
                || _dictionary1.Comparer.Equals(item2.Value, value) is false
            )
                return false;

            _dictionary1.Remove(value);
            _dictionary2.Remove(item2.Key);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T2, T1> item2)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.TryGetValue(item2.Key, out var value1)
                && _dictionary1.Comparer.Equals(item2.Value, value1);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public bool ContainsKey(T2 key2)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.ContainsKey(key2);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T2, T1>[] array2, int arrayIndex)
    {
        _lock.EnterReadLock();
        try
        {
            ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).CopyTo(array2, arrayIndex);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key2, [MaybeNullWhen(false)] out T1 value)
    {
        _lock.EnterReadLock();
        try
        {
            return _dictionary2.TryGetValue(key2, out value);
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
    public KeyValuePair<T1, T2>[] GetSnapshot()
    {
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
    public T1[] GetKeysSnapshot()
    {
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
    public T2[] GetValuesSnapshot()
    {
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
    public int EnsureCapacity(int capacity)
    {
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

    /// <inheritdoc/>
    public void Dispose()
    {
        _lock.Dispose();
    }
}
