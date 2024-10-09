using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读的可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableList<T> : IObservableList<T>, IReadOnlyObservableList<T>, IList
{
    /// <summary>
    /// 原始可观测列表
    /// </summary>
    public IObservableList<T> _list;

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableList<T> list)
    {
        _list = list;
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
        set => throw new ReadOnlyException();
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
        set => throw new ReadOnlyException();
    }
    T IObservableList<T>.this[int index, bool skipCheck]
    {
        get => ((IReadOnlyList<T>)_list)[index];
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
        throw new ReadOnlyException();
    }

    void IList.Clear()
    {
        throw new ReadOnlyException();
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
