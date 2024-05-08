using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤集合
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class FilterSet<TItem, TFilteredSet>
    : ISet<TItem>,
        IReadOnlySet<TItem>,
        IFilterCollection<TItem, TFilteredSet>
    where TFilteredSet : ISet<TItem>
{
    private readonly HashSet<TItem> _set = new();

    private Predicate<TItem> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public required Predicate<TItem> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    private TFilteredSet _filteredSet = default!;

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public required TFilteredSet FilteredSet
    {
        get => _filteredSet;
        init
        {
            _filteredSet = value;
            Refresh();
        }
    }
    TFilteredSet IFilterCollection<TItem, TFilteredSet>.FilteredCollection => FilteredSet;

    #region Ctor

    /// <inheritdoc/>
    public FilterSet()
        : this(null, null, null) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public FilterSet(int capacity)
        : this(capacity, null, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public FilterSet(IEqualityComparer<TItem> comparer)
        : this(null, null, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public FilterSet(IEnumerable<TItem> collection)
        : this(null, collection, null) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public FilterSet(IEnumerable<TItem> collection, IEqualityComparer<TItem>? comparer)
        : this(null, collection, comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public FilterSet(int capacity, IEqualityComparer<TItem>? comparer)
        : this(capacity, null, comparer) { }

    private FilterSet(
        int? capacity,
        IEnumerable<TItem>? collection,
        IEqualityComparer<TItem>? comparer
    )
    {
        if (capacity is not null)
            _set = new(capacity.Value, comparer);
        if (collection is not null)
            _set = new(collection, comparer);
        else
            _set = new(comparer);
    }
    #endregion

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh()
    {
        if (FilteredSet is null || Filter is null)
            return;
        FilteredSet.Clear();
        if (_set.HasValue())
            FilteredSet.AddRange(_set.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)_set).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        var result = ((ISet<TItem>)_set).Add(item);
        if (result && Filter(item))
            FilteredSet.Add(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)_set).Clear();
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)_set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)_set).ExceptWith(other);
        FilteredSet.ExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)_set).IntersectWith(other);
        FilteredSet.IntersectWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = ((ICollection<TItem>)_set).Remove(item);
        FilteredSet.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)_set).SymmetricExceptWith(other);
        FilteredSet.SymmetricExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)_set).UnionWith(other);
        FilteredSet.UnionWith(other.Where(i => Filter(i)));
    }

    void ICollection<TItem>.Add(TItem item)
    {
        ((ICollection<TItem>)_set).Add(item);
        if (Filter(item))
            FilteredSet.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion
}
