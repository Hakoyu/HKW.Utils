using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合包装器
/// <para>!!!注意!!! 基础集合必须是顺序集合, <see cref="HashSet{T}"/>无法有效使用此包装器, 请使用 <see cref="OrderedSet{T}"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
public class ObservableSetWrapper<TItem, TSet>
    : IObservableSet<TItem>,
        IReadOnlyObservableSet<TItem>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="comparer">比较器, 必须与 <see langword="set"/> 的比较器相同</param>
    public ObservableSetWrapper(TSet set, IEqualityComparer<TItem>? comparer = null)
    {
        SourceSet = set;
        Comparer = comparer ?? EqualityComparer<TItem>.Default;
    }

    /// <inheritdoc/>
    public TSet SourceSet { get; }
    TSet ICollectionWrapper<TItem, TSet>.SourceCollection => SourceSet;

    #region ISet

    /// <inheritdoc/>
    public int Count => SourceSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc cref="HashSet{T}.Comparer"/>
    public IEqualityComparer<TItem> Comparer { get; }

    #region Change

    /// <summary>
    /// 集合改变参数
    /// </summary>
    protected NotifySetChangeEventArgs<TItem>? SetChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        if (SourceSet.Contains(item))
            return false;
        var list = new SingleItemReadOnlyList<TItem>(item);
        OnSetAdding(list);
        SourceSet.Add(item);
        OnSetAdded(list);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        if (SourceSet.Contains(item) is false)
            return false;
        var list = new SingleItemReadOnlyList<TItem>(item);
        OnSetRemoving(list, out var removeIndex);
        SourceSet.Remove(item);
        OnSetRemoved(list, removeIndex);
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnSetClearing();
        SourceSet.Clear();
        OnSetCleared();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        var oldItems = new ReadOnlyList<TItem>(SourceSet.Except(other, Comparer));
        var otherItems = new ReadOnlyList<TItem>(other);
        OnSetOperating(SetChangeAction.Intersect, otherItems, null, oldItems, out var removeIndexs);
        SourceSet.IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Intersect, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        var oldItems = new ReadOnlyList<TItem>(SourceSet.Intersect(other));
        var otherItems = new ReadOnlyList<TItem>(other);
        OnSetOperating(SetChangeAction.Except, otherItems, null, oldItems, out var removeIndexs);
        SourceSet.ExceptWith(otherItems);
        OnSetOperated(SetChangeAction.Except, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        var otherItems = new ReadOnlyList<TItem>(other);
        var oldItems = new ReadOnlyList<TItem>(otherItems.Intersect(SourceSet, Comparer));
        var newItems = new ReadOnlyList<TItem>(otherItems.Except(oldItems, Comparer));
        OnSetOperating(
            SetChangeAction.SymmetricExcept,
            otherItems,
            newItems,
            oldItems,
            out var removeIndexs
        );
        if (other is HashSet<TItem> otherSet)
            SourceSet.SymmetricExceptWith(otherSet);
        else
            SourceSet.SymmetricExceptWith(otherItems);
        OnSetOperated(
            SetChangeAction.SymmetricExcept,
            otherItems,
            newItems,
            oldItems,
            removeIndexs
        );
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        var otherItems = new ReadOnlyList<TItem>(other);
        var newItems = new ReadOnlyList<TItem>(other.Except(SourceSet, Comparer));
        OnSetOperating(SetChangeAction.Union, otherItems, newItems, null, out var removeIndexs);
        SourceSet.UnionWith(otherItems);
        OnSetOperated(SetChangeAction.Union, otherItems, newItems, null, removeIndexs);
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
        return SourceSet.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceSet.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return SourceSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return SourceSet.SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }

    /// <inheritdoc cref="HashSet{T}.TrimExcess()"/>
    public void TrimExcess()
    {
        if (SourceSet is HashSet<TItem> set)
            set.TrimExcess();
    }

    /// <inheritdoc cref="HashSet{T}.TrimExcess(int)"/>
    public void TrimExcess(int capacity)
    {
        if (SourceSet is HashSet<TItem> set)
            set.TrimExcess(capacity);
    }

    #endregion ISet

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="items">键值对</param>
    protected virtual void OnSetAdding(IList<TItem> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Add, items));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <param name="removeIndex">项目索引</param>
    protected virtual void OnSetRemoving(IList<TItem> items, out int removeIndex)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
            removeIndex = SourceSet.IndexOf(items[0]);
        else
            removeIndex = -1;
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
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
    /// <param name="removeIndexs">删除项目索引集合</param>
    protected virtual void OnSetOperating(
        SetChangeAction action,
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems,
        out IList<int> removeIndexs
    )
    {
        if (SetChanging is not null)
            OnSetChanging(new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null && oldItems is not null)
        {
            removeIndexs = new List<int>();
            var removeItems = oldItems.ToHashSet();
            foreach (var (e, i) in SourceSet.ReverseWithIndex())
            {
                if (removeItems.Contains(e))
                {
                    removeIndexs.Add(i);
                    removeItems.Remove(e);
                    if (removeItems.HasValue is false)
                        break;
                }
            }
        }
        else
            removeIndexs = null!;
    }

    /// <summary>
    /// 集合改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanging(NotifySetChangeEventArgs<TItem> args)
    {
        SetChangeEventArgs = args;
        SetChanging?.Invoke(this, args);
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
    /// <param name="removeIndex">删除项目的索引</param>
    protected virtual void OnSetRemoved(IList<TItem> items, int removeIndex)
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, items[0], removeIndex));
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
    /// <param name="removeIndexs">删除项目集合</param>
    protected virtual void OnSetOperated(
        SetChangeAction action,
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems,
        IList<int> removeIndexs
    )
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null)
        {
            if (oldItems is not null)
            {
                foreach (var (e, i) in ((IEnumerable<TItem>)oldItems).Reverse().Zip(removeIndexs))
                {
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, e, index: i));
                }
            }
            if (newItems is not null)
            {
                var index = SourceSet.Count - newItems.Count;
                foreach (var item in newItems)
                {
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index++));
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
