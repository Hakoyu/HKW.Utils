using System.Collections;
using System.Diagnostics;
using System.Linq;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 顺序集合
/// <para>按删除插入顺序排列的集合</para>
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class OrderedSet<T> : ISet<T>, IReadOnlySet<T>
    where T : notnull
{
    /// <summary>
    /// 顺序字典
    /// </summary>
    private readonly OrderedDictionary<T, byte> _dictionary;

    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <inheritdoc/>
    public OrderedSet()
    {
        _dictionary = new();
        Comparer = EqualityComparer<T>.Default;
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public OrderedSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(collection);

        _dictionary = new(comparer);
        Comparer = comparer ?? EqualityComparer<T>.Default;
        UnionWith(collection);
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public OrderedSet(int capacity, IEqualityComparer<T>? comparer = null)
    {
        _dictionary = new(capacity, comparer);
        Comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>获取指定索引处的值。</summary>
    /// <param name="index">索引</param>
    /// <returns>指定索引处的值</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0 或大于或等于 <see cref="Count"/>。</exception>
    public T GetAt(int index)
    {
        return _dictionary.GetAt(index).Key;
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public bool Add(T item) => _dictionary.TryAdd(item, 0);

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => _dictionary.TryAdd(item, 0);

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return _dictionary.Remove(item, out _);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dictionary.Clear();
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Count == 0)
            return;
        if (other == this)
        {
            Clear();
            return;
        }
        if (other is ICollection<T> { Count: 0 })
            return;

        foreach (var item in other)
            _dictionary.Remove(item);
    }

    /// <inheritdoc/>
    /// <summary><paramref name="other"/> 为 <see cref="HashSet{T}"/> 时性能最佳</summary>
    public void IntersectWith(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Count == 0 || other == this)
            return;
        if (other is ICollection<T> { Count: 0 })
        {
            Clear();
            return;
        }

        var lookup = CreateLookupSet(other);
        lookup.SymmetricExceptWith(_dictionary.Keys);
        foreach (var item in lookup)
            _dictionary.Remove(item);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other == this)
        {
            Clear();
            return;
        }
        if (other is ICollection<T> { Count: 0 })
            return;
        if (Count == 0)
        {
            UnionWith(other);
            return;
        }
        var lookup = CreateLookupSet(other);
        foreach (var item in lookup.Where(item => Remove(item) is false))
        {
            _dictionary.TryAdd(item, 0);
        }
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other is ICollection<T> { Count: 0 })
            return;

        foreach (var item in other)
            _dictionary.TryAdd(item, 0);
    }

    /// <inheritdoc/>
    /// <summary><paramref name="other"/> 为 <see cref="HashSet{T}"/> 时性能最佳</summary>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other == this)
            return false;

        var lookup = CreateLookupSet(other);
        if (lookup.Count <= Count)
            return false;

        if (_dictionary.Keys.Any(x => lookup.Contains(x) is false))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Count == 0 || other == this)
            return false;

        var lookup = CreateLookupSet(other);
        if (lookup.Count == 0)
            return true;
        if (lookup.Count >= Count)
            return false;

        if (lookup.Any(x => _dictionary.ContainsKey(x) is false))
            return false;

        return true;
    }

    /// <inheritdoc/>
    /// <summary><paramref name="other"/> 为 <see cref="HashSet{T}"/> 时性能最佳</summary>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Count == 0 || other == this)
            return true;

        var lookup = CreateLookupSet(other);
        if (lookup.Count < Count)
            return false;

        if (_dictionary.Keys.Any(x => lookup.Contains(x) is false))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other == this)
            return true;

        var lookup = CreateLookupSet(other);
        if (lookup.Count > Count)
            return false;

        if (lookup.Any(x => _dictionary.ContainsKey(x) is false))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Count == 0)
            return false;

        if (other.Any(x => _dictionary.ContainsKey(x)))
            return true;

        return false;
    }

    /// <inheritdoc/>
    public bool Contains(T item) => _dictionary.ContainsKey(item);

    /// <inheritdoc/>
    /// <summary><paramref name="other"/> 为 <see cref="HashSet{T}"/> 时性能最佳</summary>
    public bool SetEquals(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other == this)
            return true;

        var lookup = CreateLookupSet(other);
        if (lookup.Count != Count)
            return false;

        if (_dictionary.Keys.Any(x => lookup.Contains(x) is false))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _dictionary.Keys.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _dictionary.Keys.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
    private HashSet<T> CreateLookupSet(IEnumerable<T> other)
    {
        if (
            other is HashSet<T> hashSet
            && (ReferenceEquals(Comparer, hashSet.Comparer) || Comparer.Equals(hashSet.Comparer))
        )
            return hashSet;

        return new HashSet<T>(other, Comparer);
    }
}
