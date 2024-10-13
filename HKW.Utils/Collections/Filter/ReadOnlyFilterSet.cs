using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读过滤集合
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyFilterSet<T, TFilteredSet>
    : ISet<T>,
        IReadOnlySet<T>,
        IFilterCollection<T, IObservableSet<T>, TFilteredSet>
    where TFilteredSet : ISet<T>
{
    private readonly IObservableSet<T> _set;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    public ReadOnlyFilterSet(IObservableSet<T> set, TFilteredSet filteredSet, Predicate<T> filter)
    {
        if (filteredSet.IsReadOnly)
            throw new ReadOnlyException("FilteredSet is read only");
        _set = set;
        FilteredSet = filteredSet;
        Filter = filter;
    }
    #endregion
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<T> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<T> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IObservableSet<T> IFilterCollection<T, IObservableSet<T>, TFilteredSet>.BaseCollection =>
        throw new ReadOnlyException();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilterCollection<T, IObservableSet<T>, TFilteredSet>.FilteredCollection =>
        FilteredSet;

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh()
    {
        if (Filter is null)
            FilteredSet.AddRange(_set);
        else if (_set.HasValue())
            FilteredSet.AddRange(_set.Where(i => Filter(i)));
    }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

    /// <inheritdoc/>
    bool ISet<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_set).GetEnumerator();
    }

    /// <inheritdoc/>
    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
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
    bool ICollection<T>.Remove(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion
}
