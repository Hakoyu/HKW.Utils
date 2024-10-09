using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
///
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TSet"></typeparam>
public class ObservableSetWrapper<TItem, TSet>
    : IObservableSet<TItem>,
        IReadOnlyObservableSet<TItem>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    public ObservableSetWrapper(TSet set)
    {
        BaseSet = set;
    }

    /// <inheritdoc/>
    public TSet BaseSet { get; }

    #region ISet

    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)BaseSet).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)BaseSet).IsReadOnly;

    #region Change

    /// <summary>
    /// 集合改变参数
    /// </summary>
    protected NotifySetChangeEventArgs<TItem>? SetChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        var list = new SimpleSingleItemReadOnlyList<TItem>(item);
        OnSetAdding(list);
        var result = BaseSet.Add(item);
        if (result)
            OnSetAdded(list);
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var list = new SimpleSingleItemReadOnlyList<TItem>(item);
        OnSetRemoving(list);
        var result = BaseSet.Remove(item);
        if (result)
            OnSetRemoved(list);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnSetClearing();
        BaseSet.Clear();
        OnSetCleared();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        var oldItems = new SimpleReadOnlyList<TItem>(BaseSet.Except(other));
        var otherItems = new SimpleReadOnlyList<TItem>(other);
        OnSetOperating(SetChangeAction.Intersect, otherItems, null, oldItems);
        BaseSet.IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Intersect, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        var oldItems = new SimpleReadOnlyList<TItem>(BaseSet.Intersect(other));
        var otherItems = new SimpleReadOnlyList<TItem>(other);
        OnSetOperating(SetChangeAction.Except, otherItems, null, oldItems);
        BaseSet.ExceptWith(otherItems);
        OnSetOperated(SetChangeAction.Except, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        var otherItems = new SimpleReadOnlyList<TItem>(other);
        var oldItems = new SimpleReadOnlyList<TItem>(otherItems.Intersect(BaseSet));
        var newItems = new SimpleReadOnlyList<TItem>(otherItems.Except(oldItems));
        OnSetOperating(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems);
        if (other is HashSet<TItem> otherSet)
            BaseSet.SymmetricExceptWith(otherSet);
        else
            BaseSet.SymmetricExceptWith(otherItems);
        OnSetOperated(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        var newItems = new SimpleReadOnlyList<TItem>(BaseSet.Except(other));
        var otherItems = new SimpleReadOnlyList<TItem>(other);
        OnSetOperating(SetChangeAction.Union, otherItems, newItems, null);
        BaseSet.UnionWith(otherItems);
        OnSetOperated(SetChangeAction.Union, otherItems, newItems, null);
    }

    /// <inheritdoc/>
    void ICollection<TItem>.Add(TItem item)
    {
        Add(item);
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)BaseSet).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)BaseSet).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)BaseSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return ((ISet<TItem>)BaseSet).SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseSet).GetEnumerator();
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
    protected virtual void OnSetAdding(IList<TItem> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Add, items));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnSetRemoving(IList<TItem> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
        {
            _removeIndexs.Clear();
            var removeItems = items.ToHashSet();
            foreach ((var index, var item) in BaseSet.ReverseEnumerateIndex())
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
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems
    )
    {
        if (SetChanging is not null)
            OnSetChanging(new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null && oldItems is not null)
        {
            _removeIndexs.Clear();
            var removeItems = oldItems.ToHashSet();
            foreach ((var index, var item) in BaseSet.ReverseEnumerateIndex())
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
    protected virtual void OnSetChanging(NotifySetChangeEventArgs<TItem> args)
    {
        SetChanging?.Invoke(this, SetChangeEventArgs = args);
    }

    /// <inheritdoc/>
    public event ObservableSetChangingEventHandler<TItem>? SetChanging;

    #endregion SetChanging

    #region SetChanged

    /// <summary>
    /// 集合添加键值对后
    /// </summary>
    /// <param name="items">键值对</param>
    protected virtual void OnSetAdded(IList<TItem> items)
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
    protected virtual void OnSetRemoved(IList<TItem> items)
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
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems
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
                foreach ((var index, var item) in BaseSet.ReverseEnumerateIndex())
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
    protected virtual void OnSetChanged(NotifySetChangeEventArgs<TItem> args)
    {
        SetChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableSetChangedEventHandler<TItem>? SetChanged;

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
