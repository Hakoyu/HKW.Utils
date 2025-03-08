using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 顺序集合
/// <para>按删除插入顺序排列的集合</para>
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class OrderedSet<T> : ISet<T>
{
    /// <inheritdoc/>
    public OrderedSet()
    {
        _set = new();
        _linkedList = new();
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public OrderedSet(int capacity)
    {
        _set = new(capacity);
        _linkedList = new();
    }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public OrderedSet(IEqualityComparer<T> comparer)
    {
        _set = new(comparer);
        _linkedList = new();
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public OrderedSet(IEnumerable<T> collection)
    {
        _set = new(collection);
        _linkedList = new(collection);
    }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public OrderedSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
    {
        _set = new(collection, comparer);
        _linkedList = new(collection);
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public OrderedSet(int capacity, IEqualityComparer<T>? comparer)
    {
        _set = new(capacity, comparer);
        _linkedList = new();
    }

    /// <summary>
    /// 基础集合
    /// </summary>
    private HashSet<T> _set;

    /// <summary>
    /// 链表
    /// </summary>
    private LinkedList<T> _linkedList;

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(T item)
    {
        var result = ((ISet<T>)_set).Add(item);
        if (result)
            _linkedList.AddLast(item);
        return result;
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        if (_set.Add(item))
            _linkedList.AddLast(item);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var result = ((ICollection<T>)_set).Remove(item);
        if (result)
            _linkedList.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)_set).Clear();
        _linkedList.Clear();
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        foreach (var item in _set.Intersect(other, _set.Comparer))
            _linkedList.Remove(item);
        _set.ExceptWith(other);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        foreach (var item in _set.Except(other, _set.Comparer))
            _linkedList.Remove(item);
        _set.IntersectWith(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var oldItems = new SimpleReadOnlyList<T>(other.Intersect(_set, _set.Comparer));
        foreach (var item in oldItems)
            _linkedList.Remove(item);
        foreach (var item in other.Except(oldItems, _set.Comparer))
            _linkedList.AddLast(item);
        _set.SymmetricExceptWith(other);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var item in other.Except(_set, _set.Comparer))
            _linkedList.AddLast(item);
        _set.UnionWith(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_linkedList).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_linkedList).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_linkedList).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).SetEquals(other);
    }
    #endregion
}
