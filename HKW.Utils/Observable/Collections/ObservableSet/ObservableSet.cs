﻿using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察集合
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableSet<T> : IObservableSet<T>, IReadOnlyObservableSet<T>
{
    /// <summary>
    /// 源集合
    /// </summary>
    private readonly HashSet<T> _set;

    #region Ctor

    /// <inheritdoc/>
    public ObservableSet()
        : this(null, null, null) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public ObservableSet(int capacity)
        : this(capacity, null, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEqualityComparer<T> comparer)
        : this(null, null, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSet(IEnumerable<T> collection)
        : this(null, collection, null) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
        : this(null, collection, comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T>? comparer)
        : this(capacity, null, comparer) { }

    private ObservableSet(int? capacity, IEnumerable<T>? collection, IEqualityComparer<T>? comparer)
    {
        if (capacity is not null)
            _set = new(capacity.Value, comparer);
        if (collection is not null)
            _set = new(collection, comparer);
        else
            _set = new(comparer);
    }
    #endregion

    #region ISet

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

    #region Change

    /// <summary>
    /// 集合改变参数
    /// </summary>
    protected NotifySetChangeEventArgs<T>? SetChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public bool Add(T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        OnSetAdding(list);
        var result = _set.Add(item);
        if (result)
            OnSetAdded(list);
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        OnSetRemoving(list);
        var result = _set.Remove(item);
        if (result)
            OnSetRemoved(list);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnSetClearing();
        _set.Clear();
        OnSetCleared();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        var oldItems = new SimpleReadOnlyList<T>(_set.Except(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        OnSetOperating(SetChangeAction.Intersect, otherItems, null, oldItems);
        _set.IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Intersect, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        var oldItems = new SimpleReadOnlyList<T>(_set.Intersect(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        OnSetOperating(SetChangeAction.Except, otherItems, null, oldItems);
        _set.ExceptWith(otherItems);
        OnSetOperated(SetChangeAction.Except, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherItems = new SimpleReadOnlyList<T>(other);
        var oldItems = new SimpleReadOnlyList<T>(otherItems.Intersect(_set));
        var newItems = new SimpleReadOnlyList<T>(otherItems.Except(oldItems));
        OnSetOperating(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems);
        if (other is HashSet<T> otherSet)
            _set.SymmetricExceptWith(otherSet);
        else
            _set.SymmetricExceptWith(otherItems);
        OnSetOperated(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        var newItems = new SimpleReadOnlyList<T>(_set.Except(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        OnSetOperating(SetChangeAction.Union, otherItems, newItems, null);
        _set.UnionWith(otherItems);
        OnSetOperated(SetChangeAction.Union, otherItems, newItems, null);
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }

    #endregion ISet
    private readonly List<int> _addIndexs = new();
    private readonly List<int> _removeIndexs = new();

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetAdding(IList<T> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Add, items));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetRemoving(IList<T> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
        {
            _removeIndexs.Clear();
            var removeItems = items.ToHashSet();
            foreach ((var index, var item) in _set.ReverseEnumerateIndex())
            {
                if (removeItems.Contains(item))
                {
                    _removeIndexs.Add(index);
                    removeItems.Remove(item);
                }
            }
        }
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetClearing()
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Clear));
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="action">行动</param>
    /// <param name="otherItems">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetOperating(
        SetChangeAction action,
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems
    )
    {
        if (SetChanging is not null)
            OnSetChanging(new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null && oldItems is not null)
        {
            _removeIndexs.Clear();
            var removeItems = oldItems.ToHashSet();
            foreach ((var index, var item) in _set.ReverseEnumerateIndex())
            {
                if (removeItems.Contains(item))
                {
                    _removeIndexs.Add(index);
                    removeItems.Remove(item);
                    if (removeItems.HasValue() is false)
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 集合改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetChanging(NotifySetChangeEventArgs<T> args)
    {
        SetChanging?.Invoke(this, SetChangeEventArgs = args);
    }

    /// <inheritdoc/>
    public event ObservableSetChangingEventHandler<T>? SetChanging;

    #endregion SetChanging

    #region SetChanged

    /// <summary>
    /// 集合添加键值对后
    /// </summary>
    /// <param name="items">键值对</param>
    protected virtual void OnSetAdded(IList<T> items)
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(SetChangeAction.Add, items));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)items, Count - 1));
        OnCountChanged();
    }

    /// <summary>
    /// 集合删除项目后
    /// </summary>
    /// <param name="items">键值对</param>
    protected virtual void OnSetRemoved(IList<T> items)
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
        {
            foreach ((var item, var index) in items.Zip(_removeIndexs))
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
        }
        OnCountChanged();
    }

    /// <summary>
    /// 集合清理后
    /// </summary>
    protected virtual void OnSetCleared()
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(SetChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        OnCountChanged();
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="action">行动</param>
    /// <param name="otherItems">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    protected virtual void OnSetOperated(
        SetChangeAction action,
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems
    )
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null)
        {
            if (oldItems is not null)
            {
                foreach ((var item, var index) in oldItems.Zip(_removeIndexs))
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
            }
            if (newItems is not null)
            {
                var addItems = newItems.ToHashSet();
                foreach ((var index, var item) in _set.ReverseEnumerateIndex())
                {
                    if (addItems.Contains(item))
                    {
                        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
                        addItems.Remove(item);
                        if (addItems.HasValue() is false)
                            break;
                    }
                }
            }
        }
        OnCountChanged();
    }

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanged(NotifySetChangeEventArgs<T> args)
    {
        SetChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableSetChangedEventHandler<T>? SetChanged;

    #endregion SetChanged

    #region CollectionChanged

    /// <summary>
    /// 集合已改变前
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
        SetChangeEventArgs = null;
    }

    /// <summary>
    /// 属性已改变前
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
