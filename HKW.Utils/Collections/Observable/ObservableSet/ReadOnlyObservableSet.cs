using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Natives;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读可观察合集
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableSet<T> : IObservableSet<T>, IReadOnlyObservableSet<T>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IObservableSet<T> _set;

    /// <inheritdoc/>
    /// <param name="set"></param>
    public ReadOnlyObservableSet(IObservableSet<T> set)
    {
        _set = set;
    }

    #region IReadOnlyObservableSet
    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<T>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public IEqualityComparer<T> Comparer => _set.Comparer;

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((IReadOnlySet<T>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion

    #region IObservableSet

    bool ISet<T>.Add(T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Event
    event XCancelEventHandler<NotifySetChangingEventArgs<T>>? INotifySetChanging<T>.SetChanging
    {
        add => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
        remove => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifySetChangedEventArgs<T>>? SetChanged
    {
        add => _set.SetChanged += value;
        remove => _set.SetChanged -= value;
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _set.CollectionChanged += value;
        remove => _set.CollectionChanged -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _set.PropertyChanged += value;
        remove => _set.PropertyChanged -= value;
    }
    #endregion
}
