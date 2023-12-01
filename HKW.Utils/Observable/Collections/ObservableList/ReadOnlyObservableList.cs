using HKW.HKWUtils.DebugViews;

using HKW.HKWUtils.Natives;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读的可观察列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableList<T> : IObservableList<T>, IReadOnlyObservableList<T>, IDisposable
{
    /// <summary>
    /// 原始可观察列表
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IObservableListX<T> _list;

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableListX<T> list)
    {
        _list = list;
        _list.ListChanging += List_ListChanging;
        _list.ListChanged += List_ListChanged;
        _list.CollectionChanged += List_CollectionChanged;
        _list.PropertyChanged += List_PropertyChanged;
    }

    private void List_ListChanging(IObservableList<T> sender, NotifyListChangingEventArgs<T> e)
    {
        ListChanging?.Invoke(this, e);
    }

    private void List_ListChanged(IObservableList<T> sender, NotifyListChangedEventArgs<T> e)
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
    private bool _disposedValue;

    /// <inheritdoc/>
    ~ReadOnlyObservableList() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (disposing)
            Close();
        _disposedValue = true;
    }

    /// <inheritdoc/>
    public void Close()
    {
        _list.ListChanging -= List_ListChanging;
        _list.ListChanged -= List_ListChanged;
        _list.CollectionChanged -= List_CollectionChanged;
        _list.PropertyChanged -= List_PropertyChanged;
    }
    #endregion

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
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
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
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    #endregion IObservableList

    #region Event

    /// <inheritdoc/>
    /// <remarks>!!!注意!!! 使用 <see cref="CancelEventArgs.Cancel"/> 不会产生效果</remarks>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<T>? ListChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion Event
}
