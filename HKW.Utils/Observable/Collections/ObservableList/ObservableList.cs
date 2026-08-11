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
            OnListReplacing(value, oldValue, index);
            _list[index] = value;
            OnListReplaced(value, oldValue, index);
        }
    }

    /// <summary>
    /// 列表改变事件参数
    /// </summary>
    protected NotifyListChangeEventArgs<T>? ListChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var index = _list.Count;
        OnListAdding(item, index);
        _list.Add(item);
        OnListAdded(item, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        ArgumentOutOfRangeException.ThrowIfOutOfRangeMaxExclusive(index, 0, Count);
        OnListAdding(item, index);
        _list.Insert(index, item);
        OnListAdded(item, index);
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
        OnListRemoving(item, index);
        _list.RemoveAt(index);
        OnListRemoved(item, index);
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
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
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
        set
        {
            var oldValue = _list[index];
            if (EqualityComparer<T>.Default.Equals((T)oldValue, (T)value!))
                return;
            OnListReplacing((T)value!, oldValue, index);
            _list[index] = (T)value!;
            OnListReplaced((T)value!, oldValue, index);
        }
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

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdding(T item, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Add, item, index));
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoving(T item, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Remove, item, index));
    }

    /// <summary>
    /// 列表清理前
    /// </summary>
    protected virtual void OnListClearing()
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Clear));
    }

    /// <summary>
    /// 列表改变项目前
    /// </summary>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListReplacing(T newItem, T oldItem, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Replace, newItem, oldItem, index));
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanging(NotifyListChangeEventArgs<T> args)
    {
        ListChangeEventArgs = args;
        ListChanging?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdded(T item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Add, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
        OnCountChanged();
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoved(T item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Remove, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
        OnCountChanged();
    }

    /// <summary>
    /// 列表清理后
    /// </summary>
    protected virtual void OnListCleared()
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnCountChanged();
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListReplaced(T newItem, T oldItem, int index)
    {
        if (ListChanged is not null)
            OnListChanged(
                ListChangeEventArgs ?? new(ListChangeAction.Replace, newItem, oldItem, index)
            );
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newItem, oldItem, index)
            );
        PropertyChanged?.InvokeIndexer(this);
    }

    /// <summary>
    /// 列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangeEventArgs<T> args)
    {
        ListChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<T>? ListChanged;

    #endregion ListChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged

    #region PropertyChanged

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountChanged()
    {
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
        ListChangeEventArgs = null;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="propertyName">属性名</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion PropertyChanged
}
