using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读过滤集合
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
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
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(filteredSet);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(set);
        ArgumentException.ThrowIfReadOnlyCollection(filteredSet);

        _set = set;
        FilteredSet = filteredSet;
        Filter = filter;
    }
    #endregion

    /// <inheritdoc/>
    bool IFilterCollection<T, IObservableSet<T>, TFilteredSet>.AutoFilter { get; set; } = true;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<T> Filter
    {
        get => field;
        set
        {
            field = value;
            Refresh();
        }
    }

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet { get; }

    IObservableSet<T> IFilterCollection<T, IObservableSet<T>, TFilteredSet>.SourceCollection =>
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);

    TFilteredSet IFilterCollection<T, IObservableSet<T>, TFilteredSet>.FilteredCollection =>
        FilteredSet;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (Filter is null)
            FilteredSet.AddRange(_set);
        else if (_set.HasValue)
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
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
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
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_set).GetEnumerator();
    }

    /// <inheritdoc/>
    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
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
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion
}
