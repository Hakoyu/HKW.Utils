using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

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
    bool ICollection<T>.IsReadOnly => true;

    #endregion

    #region IListFind
    /// <inheritdoc/>
    public T? Find(Predicate<T> match)
    {
        return _list.Find(match);
    }

    /// <inheritdoc/>
    public (int Index, T? Value) Find(int startIndex, Predicate<T> match)
    {
        var index = _list.FindIndex(startIndex, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public (int Index, T? Value) Find(int startIndex, int count, Predicate<T> match)
    {
        var index = _list.FindIndex(startIndex, count, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public bool TryFind(Predicate<T> match, [MaybeNullWhen(false)] out T item)
    {
        var index = _list.FindIndex(match);
        item = _list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFind(int startIndex, Predicate<T> match, out (int Index, T Value) item)
    {
        var index = _list.FindIndex(startIndex, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFind(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    )
    {
        var index = _list.FindIndex(startIndex, count, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public int FindIndex(Predicate<T> match)
    {
        return _list.FindIndex(match);
    }

    /// <inheritdoc/>
    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, match);
    }

    /// <inheritdoc/>
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, count, match);
    }

    /// <inheritdoc/>
    public T? FindLast(Predicate<T> match)
    {
        return _list.FindLast(match);
    }

    /// <inheritdoc/>
    public (int Index, T? Value) FindLast(int startIndex, Predicate<T> match)
    {
        var index = _list.FindLastIndex(startIndex, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public (int Index, T? Value) FindLast(int startIndex, int count, Predicate<T> match)
    {
        var index = _list.FindLastIndex(startIndex, count, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public bool TryFindLast(Predicate<T> match, [MaybeNullWhen(false)] out T item)
    {
        var index = _list.FindLastIndex(match);
        item = _list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFindLast(int startIndex, Predicate<T> match, out (int Index, T Value) item)
    {
        var index = _list.FindLastIndex(startIndex, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFindLast(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    )
    {
        var index = _list.FindLastIndex(startIndex, count, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public int FindLastIndex(Predicate<T> match)
    {
        return _list.FindLastIndex(match);
    }

    /// <inheritdoc/>
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, match);
    }

    /// <inheritdoc/>
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, count, match);
    }
    #endregion

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
