using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class CyclicDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    #region ctor
    /// <inheritdoc/>
    public CyclicDictionary()
        : this(null!, null) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public CyclicDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : this(collection, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public CyclicDictionary(IEqualityComparer<TKey> comparer)
        : this(null!, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public CyclicDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
    {
        if (collection is not null)
            _dictionary = new(collection, comparer);
        else
            _dictionary = new(comparer);
    }
    #endregion

    #region Cyclic

    private KeyValuePair<TKey, TValue> _current = default;

    /// <summary>
    /// 当前项目
    /// </summary>
    public KeyValuePair<TKey, TValue> Current
    {
        get => _current;
        private set
        {
            _current = value;
            CurrentKey = _current.Key;
            CurrentValue = _current.Value;
        }
    }

    /// <summary>
    /// 当前索引
    /// </summary>
    public TKey CurrentKey { get; private set; } = default!;

    /// <summary>
    /// 当前值
    /// </summary>
    public TValue CurrentValue { get; private set; } = default!;

    /// <summary>
    /// 自动重置
    /// </summary>
    [DefaultValue(false)]
    public bool AutoReset { get; set; } = false;

    private readonly List<TKey> _keys = new();
    private int _currentIndex = 0;

    /// <summary>
    /// 移动到下一个
    /// </summary>
    /// <returns>移动成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool MoveNext()
    {
        if (_currentIndex >= Count)
        {
            if (AutoReset)
            {
                _currentIndex = 0;
                Current = _dictionary.GetPair(_keys[_currentIndex]);
                return true;
            }
            return false;
        }
        _currentIndex++;
        Current = _dictionary.GetPair(_keys[_currentIndex]);
        return true;
    }

    /// <summary>
    /// 重置循环
    /// </summary>
    public void Reset()
    {
        if (Count == 0)
        {
            _currentIndex = -1;
            Current = default!;
        }
        else
        {
            _currentIndex = 0;
            Current = _dictionary.First();
        }
    }
    #endregion

    #region IDictionary
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)_dictionary)[key];
        set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value;
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
        _keys.Add(key);
        Reset();
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        _keys.Add(item.Key);
        Reset();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        _keys.Clear();
        Reset();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var result = ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
        _keys.Remove(key);
        Reset();
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var result = ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
        _keys.Remove(item.Key);
        Reset();
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }
    #endregion
}
