using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤集合
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
/// <typeparam name="TFilteredSet">已过滤集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
#pragma warning disable S2436
public class FilterSetWrapper<TItem, TSet, TFilteredSet>
#pragma warning restore S2436
    : ISet<TItem>,
        IReadOnlySet<TItem>,
        IFilterCollection<TItem, TSet, TFilteredSet>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
    where TFilteredSet : ISet<TItem>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    public FilterSetWrapper(TSet set, TFilteredSet filteredSet, Predicate<TItem> filter)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(filteredSet);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(set);
        ArgumentException.ThrowIfReadOnlyCollection(filteredSet);

        SourceSet = set;
        FilteredSet = filteredSet;
        Filter = filter;
    }
    #endregion

    /// <inheritdoc/>
    public bool AutoFilter { get; set; }

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => field;
        set
        {
            field = value;
            Refresh();
        }
    }

    /// <inheritdoc/>
    public TSet SourceSet { get; }

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TSet IFilterCollection<TItem, TSet, TFilteredSet>.SourceCollection => SourceSet;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilterCollection<TItem, TSet, TFilteredSet>.FilteredCollection => FilteredSet;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (Filter is null)
            FilteredSet.AddRange(SourceSet);
        else if (SourceSet.HasValue)
            FilteredSet.AddRange(SourceSet.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)SourceSet).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)SourceSet).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        var result = ((ISet<TItem>)SourceSet).Add(item);
        if (AutoFilter && result && Filter(item))
            FilteredSet.Add(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)SourceSet).Clear();
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)SourceSet).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)SourceSet).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)SourceSet).ExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.ExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)SourceSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)SourceSet).IntersectWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.IntersectWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = ((ICollection<TItem>)SourceSet).Remove(item);
        FilteredSet.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)SourceSet).SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)SourceSet).SymmetricExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.SymmetricExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)SourceSet).UnionWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.UnionWith(other.Where(i => Filter(i)));
    }

    void ICollection<TItem>.Add(TItem item)
    {
        ((ICollection<TItem>)SourceSet).Add(item);
        if (AutoFilter is false)
            return;
        if (Filter(item))
            FilteredSet.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }
    #endregion
}
