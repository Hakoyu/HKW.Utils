using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读的可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ReadOnlyObservableList<T>
    : IObservableList<T>,
        IReadOnlyObservableList<T>,
        IList,
        IDisposable
{
    /// <summary>
    /// 原始可观测列表
    /// </summary>
    protected IObservableList<T> _list;

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableList<T> list)
    {
        _list = list;

        _list.ListChanging -= List_ListChanging;
        _list.ListChanged -= List_ListChanged;
        _list.CollectionChanged -= List_CollectionChanged;
        _list.PropertyChanged -= List_PropertyChanged;

        _list.ListChanging += List_ListChanging;
        _list.ListChanged += List_ListChanged;
        _list.CollectionChanged += List_CollectionChanged;
        _list.PropertyChanged += List_PropertyChanged;
    }

    private void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
    {
        ListChanging?.Invoke(this, e);
    }

    private void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
    {
        ListChanged?.Invoke(this, e);
    }

    private void List_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void List_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
    #endregion

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ReadOnlyObservableList() => Dispose(false);

    /// <summary>
    /// 解除对引用列表注册的所有观测事件
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _list.ListChanging -= List_ListChanging;
            _list.ListChanged -= List_ListChanged;
            _list.CollectionChanged -= List_CollectionChanged;
            _list.PropertyChanged -= List_PropertyChanged;
        }
        _disposed = true;
    }
    #endregion

    #region IReadOnlyObservableList

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<T>)_list).Count;

    /// <inheritdoc/>
    public T this[int index] => ((IReadOnlyList<T>)_list)[index];

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

    /// <inheritdoc/>
    bool ICollection.IsSynchronized => ((IList)_list).IsSynchronized;

    /// <inheritdoc/>
    object ICollection.SyncRoot => ((IList)_list).SyncRoot;

    object? IList.this[int index]
    {
        get => ((IList)_list)[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

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
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }
    T IObservableList<T>.this[int index, bool skipCheck]
    {
        get => ((IReadOnlyList<T>)_list)[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
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

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IList.Contains(object? value)
    {
        return _list.Contains((T)value!);
    }

    int IList.IndexOf(object? value)
    {
        return _list.IndexOf((T)value!);
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        _list.CopyTo((T[])array, index);
    }

    #endregion IObservableListT

    #region Event
    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<T>? ListChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion Event
}
