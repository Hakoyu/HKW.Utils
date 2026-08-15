using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableList<T> : IObservableList<T>, IReadOnlyObservableList<T>, IList
{
    /// <inheritdoc/>
    public ObservableList()
    {
        _list = new();
    }

    /// <inheritdoc/>
    public ObservableList(int capacity)
    {
        _list = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableList(IEnumerable<T> collection)
    {
        _list = new(collection);
    }

    /// <inheritdoc/>
    private List<T> _list { get; }

    #region IListT

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    #region Change

    /// <inheritdoc/>
    public T this[int index]
    {
        get => _list[index];
        set
        {
            var oldValue = _list[index];
            if (EqualityComparer<T>.Default.Equals(oldValue, value) is true)
                return;
            var args = OnListReplacing(value, oldValue, index);
            _list[index] = value;
            OnListReplaced(args, value, oldValue, index);
        }
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var index = _list.Count;
        var args = OnListAdding(item, index);
        _list.Add(item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        ArgumentOutOfRangeException.ThrowIfOutOfRangeMaxExclusive(index, 0, Count);
        var args = OnListAdding(item, index);
        _list.Insert(index, item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var index = _list.IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        var item = _list[index];
        var args = OnListRemoving(item, index);
        _list.RemoveAt(index);
        OnListRemoved(args, item, index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnListClearing();
        _list.Clear();
        OnListCleared();
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
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

    #endregion IListT

    #region IList
    bool IList.IsFixedSize => ((IList)_list).IsFixedSize;
    bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;
    object? IList.this[int index]
    {
        get => _list[index];
        set => this[index] = (T)value!;
    }

    int IList.Add(object? value)
    {
        var item = (T)value!;
        Add(item);
        return Count - 1;
    }

    void IList.Insert(int index, object? value)
    {
        var item = (T)value!;
        Insert(index, item);
    }

    void IList.Remove(object? value)
    {
        var item = (T)value!;
        Remove(item);
    }

    bool IList.Contains(object? value)
    {
        var item = (T)value!;
        return _list.Contains(item);
    }

    int IList.IndexOf(object? value)
    {
        var item = (T)value!;
        return _list.IndexOf(item);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }

    #endregion

    #region ListChanging

    private NotifyListChangeEventArgs<T>? OnListAdding(T item, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Add, item, index));
        return null;
    }

    private NotifyListChangeEventArgs<T>? OnListRemoving(T item, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Remove, item, index));
        return null;
    }

    private NotifyListChangeEventArgs<T>? OnListReplacing(T newItem, T oldItem, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Replace, newItem, oldItem, index));
        return null;
    }

    private void OnListClearing()
    {
        if (ListChanging is not null)
            OnListChanging(NotifyListChangeEventArgs<T>.Cache_Clear);
    }

    private NotifyListChangeEventArgs<T> OnListChanging(NotifyListChangeEventArgs<T> args)
    {
        ListChanging?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    private void OnListAdded(NotifyListChangeEventArgs<T>? args, T item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Add, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
        OnCountChanged();
    }

    private void OnListRemoved(NotifyListChangeEventArgs<T>? args, T item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Remove, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
        OnCountChanged();
    }

    private void OnListReplaced(NotifyListChangeEventArgs<T>? args, T newItem, T oldItem, int index)
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Replace, newItem, oldItem, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newItem, oldItem, index)
            );
        PropertyChanged?.InvokeIndexer(this);
    }

    private void OnListCleared()
    {
        if (ListChanged is not null)
            OnListChanged(NotifyListChangeEventArgs<T>.Cache_Clear);
        if (CollectionChanged is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnCountChanged();
    }

    /// <summary>
    /// 列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    private void OnListChanged(NotifyListChangeEventArgs<T> args)
    {
        ListChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<T>? ListChanged;

    #endregion ListChanged

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountChanged()
    {
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
