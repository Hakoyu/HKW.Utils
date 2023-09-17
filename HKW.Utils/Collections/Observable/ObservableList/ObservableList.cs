using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableList<T> : IObservableList<T>, IReadOnlyObservableList<T>
{
    /// <summary>
    /// 原始列表
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly List<T> _list;

    /// <inheritdoc/>
    public bool TriggerRemoveActionOnClear { get; set; }

    #region Ctor

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

    #endregion Ctor

    #region IListT

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public T this[int index]
    {
        get => _list[index];
        set
        {
            var oldValue = _list[index];
            var oldList = new SimpleSingleItemReadOnlyList<T>(oldValue);
            var newList = new SimpleSingleItemReadOnlyList<T>(value);
            if (oldValue?.Equals(value) is true || OnListValueChanging(newList, oldList, index))
                return;
            _list[index] = value;
            OnListValueChanged(newList, oldList, index);
        }
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListAdding(list))
            return;
        _list.Add(item);
        OnListAdded(list);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListAdding(list, index))
            return;
        _list.Insert(index, item);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var index = _list.IndexOf(item);
        if (index == -1)
            return false;
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListRemoving(list))
            return false;
        _list.RemoveAt(index);
        OnListRemoved(list, index);
        return true;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        var item = _list[index];
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListRemoving(list, index))
            return;
        _list.RemoveAt(index);
        OnListRemoved(list, index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        if (TriggerRemoveActionOnClear)
        {
            var list = new SimpleReadOnlyList<T>(_list);
            if (OnListRemoving(list))
                return;
            _list.Clear();
            OnListRemoved(list);
            return;
        }
        if (OnListClearing())
            return;
        _list.Clear();
        OnListCleared();
    }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<T> collection)
    {
        var list = new SimpleReadOnlyList<T>(collection);
        if (OnListAdding(list))
            return;
        _list.AddRange(list);
        OnListAdded(list);
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        var list = new SimpleReadOnlyList<T>(collection);
        if (OnListAdding(list, index))
            return;
        _list.InsertRange(index, list);
        OnListAdded(list);
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        var list = new SimpleReadOnlyList<T>(_list.Skip(index).Take(count));
        if (OnListRemoving(list))
            return;
        _list.RemoveRange(index, count);
        OnListRemoved(list);
    }

    /// <inheritdoc/>
    public void ChangeRange(IEnumerable<T> collection, int index = 0)
    {
        var oldList =
            index == 0
                ? new SimpleReadOnlyList<T>(_list)
                : new SimpleReadOnlyList<T>(_list.Skip(index));
        var newList = new SimpleReadOnlyList<T>(collection);
        if (OnListValueChanging(newList, oldList, index))
            return;
        for (var i = index; i < oldList.Count; i++)
            _list[i] = newList[i];
        OnListValueChanged(newList, oldList, index);
    }

    /// <inheritdoc/>
    public void ChangeRange(IEnumerable<T> collection, int index, int count)
    {
        var oldList = new SimpleReadOnlyList<T>(_list.Skip(index).Take(count));
        var newList = new SimpleReadOnlyList<T>(collection);
        if (OnListValueChanging(newList, oldList, index))
            return;
        for (var i = index; i < oldList.Count; i++)
            _list[i] = newList[i];
        OnListValueChanged(newList, oldList, index);
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

    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListAdding(IList<T> items, int? index = null)
    {
        if (ListChanging is null)
            return false;
        return OnListChanging(new(ListChangeAction.Add, items, index ?? Count));
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListRemoving(IList<T> items, int? index = null)
    {
        if (ListChanging is null)
            return false;
        return OnListChanging(new(ListChangeAction.Remove, items, index ?? Count - 1));
    }

    /// <summary>
    /// 列表清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListClearing()
    {
        if (ListChanging is null)
            return false;
        return OnListChanging(new(ListChangeAction.Clear));
    }

    /// <summary>
    /// 列表改变项目前
    /// </summary>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="index"></param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListValueChanging(IList<T> newItems, IList<T> oldItems, int index)
    {
        if (ListChanging is null)
            return false;
        return OnListChanging(new(ListChangeAction.ValueChange, newItems, oldItems, index));
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual bool OnListChanging(NotifyListChangingEventArgs<T> args)
    {
        ListChanging?.Invoke(args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifyListChangingEventArgs<T>>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    private void OnListAdded(IList<T> items, int? index = null)
    {
        if (ListChanged is not null)
            OnListChanged(new(ListChangeAction.Add, items, index ?? Count - 1));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)items, index ?? -1));
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    private void OnListRemoved(IList<T> items, int? index = null)
    {
        if (ListChanged is not null)
            OnListChanged(new(ListChangeAction.Remove, items, index ?? Count - 1));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Remove, (IList)items, index ?? -1)
            );
    }

    /// <summary>
    /// 列表清理后
    /// </summary>
    private void OnListCleared()
    {
        if (ListChanged is not null)
            OnListChanged(new(ListChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="index"></param>
    private void OnListValueChanged(IList<T> newItems, IList<T> oldItems, int index)
    {
        if (ListChanged is not null)
            OnListChanged(new(ListChangeAction.ValueChange, newItems, oldItems, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, (IList)newItems, (IList)oldItems, index)
            );
    }

    /// <summary>
    /// 列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangedEventArgs<T> args)
    {
        ListChanged?.Invoke(args);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyListChangedEventArgs<T>>? ListChanged;

    #endregion ListChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(null, args);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged

    #region PropertyChanged

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _lastCount = 0;

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountPropertyChanged()
    {
        if (_lastCount != Count)
            OnPropertyChanged(nameof(Count));
        _lastCount = Count;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(null, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged
}
