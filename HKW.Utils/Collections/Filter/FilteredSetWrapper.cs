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
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredSetWrapper<TItem, TSet, TFilteredSet>
#pragma warning restore S2436
    : ISet<TItem>,
        IReadOnlySet<TItem>,
        IFilteredCollectionWrapper<TItem, TSet, TFilteredSet>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
    where TFilteredSet : ISet<TItem>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredSetWrapper(
        TSet set,
        TFilteredSet filteredSet,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(filteredSet);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(set);
        ArgumentException.ThrowIfReadOnlyCollection(filteredSet);

        SourceSet = set;
        FilteredSet = filteredSet;
        Filter = filter;
        AutoFilter = autoFilter;
        Refresh();
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TSet ICollectionWrapper<TItem, TSet>.SourceCollection => SourceSet;

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilteredCollectionWrapper<TItem, TSet, TFilteredSet>.FilteredCollection =>
        FilteredSet;

    /// <inheritdoc/>
    public void BatchUpdate(Action<TSet> updateAction)
    {
        ArgumentNullException.ThrowIfNull(updateAction);

        var autoFilter = AutoFilter;
        AutoFilter = false;
        try
        {
            updateAction(SourceSet);
        }
        finally
        {
            AutoFilter = autoFilter;
        }

        Refresh();
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        FilteredSet.Clear();
        if (SourceSet.Count > 0)
            FilteredSet.AddRange(SourceSet.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => SourceSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceSet.IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        var result = SourceSet.Add(item);
        if (result is false)
            return result;
        if (AutoFilter is false)
            return result;
        if (Filter(item))
            FilteredSet.Add(item);
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = SourceSet.Remove(item);
        if (AutoFilter is false)
            return result;
        FilteredSet.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceSet.Clear();
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        SourceSet.ExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.ExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        SourceSet.IntersectWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.IntersectWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        SourceSet.SymmetricExceptWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.SymmetricExceptWith(other.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        SourceSet.UnionWith(other);
        if (AutoFilter is false)
            return;
        FilteredSet.UnionWith(other.Where(i => Filter(i)));
    }

    void ICollection<TItem>.Add(TItem item)
    {
        Add(item);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return SourceSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceSet.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return SourceSet.SetEquals(other);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }
    #endregion
}
