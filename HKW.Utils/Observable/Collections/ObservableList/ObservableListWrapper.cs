using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ObservableListWrapper<TItem, TList>
    : IObservableList<TItem>,
        IReadOnlyObservableList<TItem>,
        IList,
        IListWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    public ObservableListWrapper(TList list)
    {
        BaseList = list;
    }

    /// <inheritdoc/>
    public TList BaseList { get; }

    #region IListT

    /// <inheritdoc/>
    public int Count => BaseList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)BaseList).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => BaseList[index];
        set
        {
            var oldValue = BaseList[index];
            if (oldValue?.Equals(value) is true)
                return;
            OnListReplacing(value, oldValue, index);
            BaseList[index] = value;
            OnListReplaced(value, oldValue, index);
        }
    }

    /// <inheritdoc/>
    public TItem this[int index, bool skipCheck]
    {
        get => BaseList[index];
        set
        {
            var oldValue = BaseList[index];
            if (skipCheck is false && oldValue?.Equals(value) is true)
                return;
            OnListReplacing(value, oldValue, index);
            BaseList[index] = value;
            OnListReplaced(value, oldValue, index);
        }
    }

    /// <summary>
    /// 列表改变事件参数
    /// </summary>
    protected NotifyListChangeEventArgs<TItem>? ListChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        var index = BaseList.Count;
        OnListAdding(item, index);
        BaseList.Add(item);
        OnListAdded(item, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        if (index < 0 || index > BaseList.Count)
            BaseList.Insert(index, item);
        OnListAdding(item, index);
        BaseList.Insert(index, item);
        OnListAdded(item, index);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var index = BaseList.IndexOf(item);
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
        var item = BaseList[index];
        OnListRemoving(item, index);
        BaseList.RemoveAt(index);
        OnListRemoved(item, index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnListClearing();
        BaseList.Clear();
        OnListCleared();
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return BaseList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        BaseList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return BaseList.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return BaseList.IndexOf(item);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseList).GetEnumerator();
    }

    #endregion IListT

    #region IList
    bool IList.IsFixedSize => ((IList)BaseList).IsFixedSize;
    bool ICollection.IsSynchronized => ((ICollection)BaseList).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)BaseList).SyncRoot;
    object? IList.this[int index]
    {
        get => BaseList[index];
        set
        {
            var oldValue = BaseList[index];
            if (oldValue?.Equals(value) is true)
                return;
            OnListReplacing((TItem)value!, oldValue, index);
            BaseList[index] = (TItem)value!;
            OnListReplaced((TItem)value!, oldValue, index);
        }
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
        return BaseList.Contains(item);
    }

    int IList.IndexOf(object? value)
    {
        var item = (TItem)value!;
        return BaseList.IndexOf(item);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)BaseList).CopyTo(array, index);
    }

    #endregion


    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdding(TItem item, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Add, item, index));
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoving(TItem item, int index)
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
    /// <param name="index"></param>
    protected virtual void OnListReplacing(TItem newItem, TItem oldItem, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Replace, newItem, oldItem, index));
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnListChanging(NotifyListChangeEventArgs<TItem> args)
    {
        ListChanging?.Invoke(this, ListChangeEventArgs = args);
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<TItem>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdded(TItem item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Add, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Add,
                    new SimpleSingleItemReadOnlyList<TItem>(item),
                    index
                )
            );
        OnCountChanged();
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoved(TItem item, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Remove, item, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Remove,
                    new SimpleSingleItemReadOnlyList<TItem>(item),
                    index
                )
            );
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
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        OnCountChanged();
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListReplaced(TItem newItem, TItem oldItem, int index)
    {
        if (ListChanged is not null)
            OnListChanged(
                ListChangeEventArgs ?? new(ListChangeAction.Replace, newItem, oldItem, index)
            );
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Replace,
                    new SimpleSingleItemReadOnlyList<TItem>(newItem),
                    new SimpleSingleItemReadOnlyList<TItem>(oldItem),
                    index
                )
            );
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
        OnPropertyChanged(nameof(Count));
        ListChangeEventArgs = null;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion PropertyChanged
}
