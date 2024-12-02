using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
/// <typeparam name="TFilteredSet">已过滤集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class FilterSetWrapper<TItem, TSet, TFilteredSet>
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
        if (filteredSet.IsReadOnly)
            throw new ReadOnlyException("FilteredSet is read only");
        BaseSet = set;
        FilteredSet = filteredSet;
        Filter = filter;
    }
    #endregion

    /// <inheritdoc/>
    public bool AutoFilter { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<TItem> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <inheritdoc/>
    public TSet BaseSet { get; }

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TSet IFilterCollection<TItem, TSet, TFilteredSet>.BaseCollection => BaseSet;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilterCollection<TItem, TSet, TFilteredSet>.FilteredCollection => FilteredSet;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (Filter is null)
            FilteredSet.AddRange(BaseSet);
        else if (BaseSet.HasValue())
            FilteredSet.AddRange(BaseSet.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)BaseSet).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)BaseSet).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        var result = ((ISet<TItem>)BaseSet).Add(item);
        if (AutoFilter && result && Filter(item))
            FilteredSet.Add(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)BaseSet).Clear();
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)BaseSet).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)BaseSet).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)BaseSet).ExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.ExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)BaseSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)BaseSet).IntersectWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.IntersectWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = ((ICollection<TItem>)BaseSet).Remove(item);
        FilteredSet.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)BaseSet).SymmetricExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.SymmetricExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        ((ISet<TItem>)BaseSet).UnionWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.UnionWith(other.Where(i => Filter(i)));
    }

    void ICollection<TItem>.Add(TItem item)
    {
        ((ICollection<TItem>)BaseSet).Add(item);
        if (AutoFilter is false)
            return;
        if (Filter(item))
            FilteredSet.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseSet).GetEnumerator();
    }
    #endregion
}
