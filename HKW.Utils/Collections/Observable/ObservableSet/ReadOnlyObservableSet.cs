using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifySetChangingEventArgs<T>>? SetChanging
    {
        add { ((INotifySetChanging<T>)_set).SetChanging += value; }
        remove { ((INotifySetChanging<T>)_set).SetChanging -= value; }
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifySetChangedEventArgs<T>>? SetChanged
    {
        add { ((INotifySetChanged<T>)_set).SetChanged += value; }
        remove { ((INotifySetChanged<T>)_set).SetChanged -= value; }
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add { ((INotifyCollectionChanged)_set).CollectionChanged += value; }
        remove { ((INotifyCollectionChanged)_set).CollectionChanged -= value; }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add { ((INotifyPropertyChanged)_set).PropertyChanged += value; }
        remove { ((INotifyPropertyChanged)_set).PropertyChanged -= value; }
    }
    #endregion

    #region IObservableSet
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<T>? IObservableSet<T>.Comparer => _set.Comparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableSet<T>.NotifySetModifies
    {
        get => _set.NotifySetModifies;
        set => throw new ReadOnlyException();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => true;

    bool ISet<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.Clear()
    {
        throw new ReadOnlyException();
    }

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new ReadOnlyException();
    }

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }
    #endregion
}
