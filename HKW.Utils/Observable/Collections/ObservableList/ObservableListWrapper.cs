using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableListWrapper<TItem, TList>
    : IObservableList<TItem>,
        IReadOnlyObservableList<TItem>,
        IList
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    public ObservableListWrapper(TList list)
    {
        SourceList = list;
    }

    /// <inheritdoc/>
    protected TList SourceList { get; }

    #region IListT

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    #region Change

    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => SourceList[index];
        set
        {
            var oldValue = SourceList[index];
            if (EqualityComparer<TItem>.Default.Equals(oldValue, value) is true)
                return;
            var args = OnListReplacing(value, oldValue, index);
            SourceList[index] = value;
            OnListReplaced(args, value, oldValue, index);
        }
    }

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        var index = SourceList.Count;
        var args = OnListAdding(item, index);
        SourceList.Add(item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        ArgumentOutOfRangeException.ThrowIfOutOfRangeMaxExclusive(index, 0, Count);
        var args = OnListAdding(item, index);
        SourceList.Insert(index, item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var index = SourceList.IndexOf(item);
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
        var item = SourceList[index];
        var args = OnListRemoving(item, index);
        SourceList.RemoveAt(index);
        OnListRemoved(args, item, index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnListClearing();
        SourceList.Clear();
        OnListCleared();
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }

    #endregion IListT

    #region IList
    bool IList.IsFixedSize => ((IList)SourceList).IsFixedSize;
    bool ICollection.IsSynchronized => ((ICollection)SourceList).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)SourceList).SyncRoot;
    object? IList.this[int index]
    {
        get => SourceList[index];
        set => this[index] = (TItem)value!;
    }

    int IList.Add(object? value)
    {
        var item = (TItem)value!;
        Add(item);
        return Count - 1;
    }

    void IList.Insert(int index, object? value)
    {
        var item = (TItem)value!;
        Insert(index, item);
    }

    void IList.Remove(object? value)
    {
        var item = (TItem)value!;
        Remove(item);
    }

    bool IList.Contains(object? value)
    {
        var item = (TItem)value!;
        return SourceList.Contains(item);
    }

    int IList.IndexOf(object? value)
    {
        var item = (TItem)value!;
        return SourceList.IndexOf(item);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)SourceList).CopyTo(array, index);
    }

    #endregion

    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListAdding(TItem item, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Add, item, index));
        return null;
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListRemoving(TItem item, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Remove, item, index));
        return null;
    }

    /// <summary>
    /// 列表改变项目前
    /// </summary>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListReplacing(
        TItem newItem,
        TItem oldItem,
        int index
    )
    {
        if (ListChanging is not null)
            return OnListChanging(new(ListChangeAction.Replace, newItem, oldItem, index));
        return null;
    }

    /// <summary>
    /// 列表清理前
    /// </summary>
    protected virtual void OnListClearing()
    {
        if (ListChanging is not null)
            OnListChanging(NotifyListChangeEventArgs<TItem>.Cache_Clear);
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem> OnListChanging(
        NotifyListChangeEventArgs<TItem> args
    )
    {
        ListChanging?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<TItem>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdded(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Add, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
        OnCountChanged();
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoved(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Remove, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
        OnCountChanged();
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListReplaced(
        NotifyListChangeEventArgs<TItem>? args,
        TItem newItem,
        TItem oldItem,
        int index
    )
    {
        if (ListChanged is not null)
            OnListChanged(args ?? new(ListChangeAction.Replace, newItem, oldItem, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newItem, oldItem, index)
            );
        PropertyChanged?.InvokeIndexer(this);
    }

    /// <summary>
    /// 列表清理后
    /// </summary>
    protected virtual void OnListCleared()
    {
        if (ListChanged is not null)
            OnListChanged(NotifyListChangeEventArgs<TItem>.Cache_Clear);
        if (CollectionChanged is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnCountChanged();
    }

    /// <summary>
    /// 列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangeEventArgs<TItem> args)
    {
        ListChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<TItem>? ListChanged;

    #endregion ListChanged

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

    /// <summary>
    /// 数量改变后
    /// </summary>
    protected virtual void OnCountChanged()
    {
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
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
}
