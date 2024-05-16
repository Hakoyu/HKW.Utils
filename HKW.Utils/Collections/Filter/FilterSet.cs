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
public class FilterSet<T, TSet, TFilteredSet>
    : ISet<T>,
        IReadOnlySet<T>,
        IFilterCollection<T, TSet, TFilteredSet>
    where TSet : ISet<T>
    where TFilteredSet : ISet<T>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    public FilterSet(TSet set, TFilteredSet filteredSet, Predicate<T> filter)
    {
        Set = set;
        FilteredSet = filteredSet;
        Filter = filter;
    }

    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="getFilteredSet">获取过滤集合</param>
    /// <param name="filter">过滤器</param>
    public FilterSet(TSet set, Func<TSet, TFilteredSet> getFilteredSet, Predicate<T> filter)
        : this(set, getFilteredSet(set), filter) { }
    #endregion
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<T> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public required Predicate<T> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <summary>
    /// 集合
    /// <para>使用此属性修改集合时不会同步至 <see cref="FilteredSet"/></para>
    /// </summary>
    public TSet Set { get; }

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TSet IFilterCollection<T, TSet, TFilteredSet>.Collection => Set;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilterCollection<T, TSet, TFilteredSet>.FilteredCollection => FilteredSet;

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh()
    {
        if (FilteredSet.IsReadOnly)
            return;
        if (Filter is null)
            FilteredSet.AddRange(Set);
        else if (Set.HasValue())
            FilteredSet.AddRange(Set.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<T>)Set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)Set).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(T item)
    {
        var result = ((ISet<T>)Set).Add(item);
        if (result && Filter(item) && FilteredSet.IsReadOnly is false)
            FilteredSet.Add(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)Set).Clear();
        if (FilteredSet.IsReadOnly)
            return;
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)Set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)Set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)Set).ExceptWith(other);
        if (FilteredSet.IsReadOnly)
            return;
        FilteredSet.ExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Set).GetEnumerator();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        ((ISet<T>)Set).IntersectWith(other);
        if (FilteredSet.IsReadOnly)
            return;
        FilteredSet.IntersectWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var result = ((ICollection<T>)Set).Remove(item);
        FilteredSet.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)Set).SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)Set).SymmetricExceptWith(other);
        if (FilteredSet.IsReadOnly)
            return;
        FilteredSet.SymmetricExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        ((ISet<T>)Set).UnionWith(other);
        if (FilteredSet.IsReadOnly)
            return;
        FilteredSet.UnionWith(other.Where(i => Filter(i)));
    }

    void ICollection<T>.Add(T item)
    {
        ((ICollection<T>)Set).Add(item);
        if (FilteredSet.IsReadOnly)
            return;
        if (Filter(item))
            FilteredSet.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Set).GetEnumerator();
    }
    #endregion
}
