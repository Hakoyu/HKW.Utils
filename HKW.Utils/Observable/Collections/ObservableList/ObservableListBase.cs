﻿using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public abstract class ObservableListBase<T> : IObservableList<T>, IReadOnlyObservableList<T>
{
    /// <summary>
    /// 原始列表
    /// </summary>
    protected readonly List<T> _list;

    #region Ctor

    /// <inheritdoc/>
    public ObservableListBase()
        : this(null, null) { }

    /// <inheritdoc/>
    public ObservableListBase(int capacity)
        : this(capacity, null) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableListBase(IEnumerable<T> collection)
        : this(null, collection) { }

    private ObservableListBase(int? capacity, IEnumerable<T>? collection)
    {
        if (capacity is not null)
            _list = new(capacity.Value);
        else if (collection is not null)
            _list = new(collection);
        else
            _list = new();
    }

    #endregion Ctor

    #region IListT

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
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
            if (oldValue?.Equals(value) is true)
                return;
            OnListReplacing(newList, oldList, index);
            _list[index] = value;
            OnListReplaced(newList, oldList, index);
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
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        OnListAdding(list, index);
        _list.Add(item);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > _list.Count)
            _list.Insert(index, item);
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        OnListAdding(list, index);
        _list.Insert(index, item);
        OnListAdded(list, index);
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
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        OnListRemoving(list, index);
        _list.RemoveAt(index);
        OnListRemoved(list, index);
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

    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdding(IList<T> items, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Add, items, index));
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoving(IList<T> items, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Remove, items, index));
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
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="index"></param>
    protected virtual void OnListReplacing(IList<T> newItems, IList<T> oldItems, int index)
    {
        if (ListChanging is not null)
            OnListChanging(new(ListChangeAction.Replace, newItems, oldItems, index));
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnListChanging(NotifyListChangeEventArgs<T> args)
    {
        ListChanging?.Invoke(this, ListChangeEventArgs = args);
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdded(IList<T> items, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Add, items, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)items, index));
        OnCountChanged();
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoved(IList<T> items, int index)
    {
        if (ListChanged is not null)
            OnListChanged(ListChangeEventArgs ?? new(ListChangeAction.Remove, items, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (IList)items, index));
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
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="index"></param>
    protected virtual void OnListReplaced(IList<T> newItems, IList<T> oldItems, int index)
    {
        if (ListChanged is not null)
            OnListChanged(
                ListChangeEventArgs ?? new(ListChangeAction.Replace, newItems, oldItems, index)
            );
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, (IList)newItems, (IList)oldItems, index)
            );
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
