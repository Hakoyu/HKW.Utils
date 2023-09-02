using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读的可观察列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableList<T>
    : IObservableList<T>,
        IObservableList,
        IReadOnlyObservableList<T>
{
    /// <summary>
    /// 原始可观察列表
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IObservableList<T> r_list;

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableList<T> list)
    {
        r_list = list;
    }

    #endregion Ctor

    #region IReadOnlyObservableList

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<T>)r_list).Count;

    /// <inheritdoc/>
    public T this[int index] => ((IReadOnlyList<T>)r_list)[index];

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return r_list.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_list).GetEnumerator();
    }

    #endregion IReadOnlyObservableList

    #region IObservableListT

    T IList<T>.this[int index]
    {
        get => ((IReadOnlyList<T>)r_list)[index];
        set => throw new ReadOnlyException();
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new ReadOnlyException();
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new ReadOnlyException();
    }

    void ICollection<T>.Clear()
    {
        throw new ReadOnlyException();
    }

    int IList<T>.IndexOf(T item)
    {
        return r_list.IndexOf(item);
    }

    bool ICollection<T>.Contains(T item)
    {
        return r_list.Contains(item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        r_list.CopyTo(array, arrayIndex);
    }

    #endregion IObservableListT

    #region IObservableList
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsFixedSize => ((IList)r_list).IsFixedSize;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsReadOnly => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => ((IList)r_list).IsSynchronized;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => ((IList)r_list).SyncRoot;

    object? IList.this[int index]
    {
        get => ((IReadOnlyList<T>)r_list)[index];
        set => throw new ReadOnlyException();
    }

    int IList.Add(object? value)
    {
        throw new ReadOnlyException();
    }

    void IList.Clear()
    {
        throw new ReadOnlyException();
    }

    void IList.Insert(int index, object? value)
    {
        throw new ReadOnlyException();
    }

    void IList.Remove(object? value)
    {
        throw new ReadOnlyException();
    }

    void IList.RemoveAt(int index)
    {
        throw new ReadOnlyException();
    }

    int IList.IndexOf(object? value)
    {
        return ((IList)r_list).IndexOf(value);
    }

    bool IList.Contains(object? value)
    {
        return ((IList)r_list).Contains(value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((IList)r_list).CopyTo(array, index);
    }

    #endregion IObservableList

    #region Event

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifyListChangingEventArgs<T>>? ListChanging
    {
        add => r_list.ListChanging += value;
        remove => r_list.ListChanging -= value;
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyListChangedEventArgs<T>>? ListChanged
    {
        add => r_list.ListChanged += value;
        remove => r_list.ListChanged -= value;
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => r_list.CollectionChanged += value;
        remove => r_list.CollectionChanged -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => r_list.PropertyChanged += value;
        remove => r_list.PropertyChanged -= value;
    }

    event XCancelEventHandler<NotifyListChangingEventArgs<object>>? INotifyListChanging.ListChanging
    {
        add => ((IObservableList)r_list).ListChanging += value;
        remove => ((IObservableList)r_list).ListChanging -= value;
    }

    event XEventHandler<NotifyListChangedEventArgs<object>>? INotifyListChanged.ListChanged
    {
        add => ((IObservableList)r_list).ListChanged += value;
        remove => ((IObservableList)r_list).ListChanged -= value;
    }

    #endregion Event
}
