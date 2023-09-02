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
    private readonly IObservableSet<T> r_set;

    /// <inheritdoc/>
    /// <param name="set"></param>
    public ReadOnlyObservableSet(IObservableSet<T> set)
    {
        r_set = set;
    }

    #region IReadOnlyObservableSet
    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<T>)r_set).Count;

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((IReadOnlySet<T>)r_set).Contains(item);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)r_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((IReadOnlySet<T>)r_set).SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifySetChangingEventArgs<T>>? SetChanging
    {
        add { ((INotifySetChanging<T>)r_set).SetChanging += value; }
        remove { ((INotifySetChanging<T>)r_set).SetChanging -= value; }
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifySetChangedEventArgs<T>>? SetChanged
    {
        add { ((INotifySetChanged<T>)r_set).SetChanged += value; }
        remove { ((INotifySetChanged<T>)r_set).SetChanged -= value; }
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add { ((INotifyCollectionChanged)r_set).CollectionChanged += value; }
        remove { ((INotifyCollectionChanged)r_set).CollectionChanged -= value; }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add { ((INotifyPropertyChanged)r_set).PropertyChanged += value; }
        remove { ((INotifyPropertyChanged)r_set).PropertyChanged -= value; }
    }
    #endregion

    #region IObservableSet
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<T>? IObservableSet<T>.Comparer => r_set.Comparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableSet<T>.NotifySetModifies
    {
        get => r_set.NotifySetModifies;
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
        r_set.CopyTo(array, arrayIndex);
    }
    #endregion
}
