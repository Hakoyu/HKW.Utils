using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Natives;
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
public class ReadOnlyObservableList<T> : IObservableList<T>, IReadOnlyObservableList<T>
{
    /// <summary>
    /// 原始可观察列表
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IObservableList<T> _list;

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableList<T> list)
    {
        _list = list;
    }

    #endregion Ctor

    #region IReadOnlyObservableList

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<T>)_list).Count;

    /// <inheritdoc/>
    public T this[int index] => ((IReadOnlyList<T>)_list)[index];

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    #endregion IReadOnlyObservableList

    #region IObservableListT

    T IList<T>.this[int index]
    {
        get => ((IReadOnlyList<T>)_list)[index];
        set => throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    int IList<T>.IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    bool ICollection<T>.Contains(T item)
    {
        return _list.Contains(item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    #endregion IObservableListT

    #region IObservableList
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableCollection<T>.TriggerRemoveActionOnClear
    {
        get => _list.TriggerRemoveActionOnClear;
        set => throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IObservableList<T>.InsertRange(int index, IEnumerable<T> collection)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IObservableList<T>.RemoveRange(int index, int count)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IObservableList<T>.ChangeRange(IEnumerable<T> collection, int index)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IObservableList<T>.ChangeRange(IEnumerable<T> collection, int index, int count)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IObservableCollection<T>.AddRange(IEnumerable<T> items)
    {
        throw new NotImplementedException(ExceptionMessage.ReadOnlyCollection);
    }

    #endregion IObservableList

    #region Event

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifyListChangingEventArgs<T>>? ListChanging
    {
        add => _list.ListChanging += value;
        remove => _list.ListChanging -= value;
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyListChangedEventArgs<T>>? ListChanged
    {
        add => _list.ListChanged += value;
        remove => _list.ListChanged -= value;
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _list.CollectionChanged += value;
        remove => _list.CollectionChanged -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _list.PropertyChanged += value;
        remove => _list.PropertyChanged -= value;
    }
    #endregion Event
}
