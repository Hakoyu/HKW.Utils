using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合包装器
/// <para>注意: 若要求稳定的逐项批量索引, 基础集合必须是顺序集合, 请使用 <see cref="OrderedHashSet{T}"/>.
/// <see cref="HashSet{T}"/> 无法保证此顺序, 其批量通知使用 Reset, 而非禁止包装该后端.</para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSetWrapper<TItem, TSet>
    : IObservableSet<TItem>,
        IReadOnlyObservableSet<TItem>,
        ICollectionNotificationState
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="comparer">比较器; 已知后端采用源比较器, 自定义后端须传入与源语义相同的比较器</param>
    public ObservableSetWrapper(TSet set, IEqualityComparer<TItem>? comparer)
    {
        ArgumentNullException.ThrowIfNull(set);
        SourceSet = set;
        Comparer = CollectionSourceComparer.Set(set, comparer);
        _canSkipHooks =
            (
                GetType().IsGenericType
                && GetType().GetGenericTypeDefinition() == typeof(ObservableSet<>)
            )
            || GetType() == typeof(ObservableSetWrapper<TItem, TSet>);
    }

    /// <inheritdoc/>
    protected TSet SourceSet { get; }

    private readonly CollectionNotificationScope<
        ObservableSetChangingEventHandler<TItem>,
        ObservableSetChangedEventHandler<TItem>
    > _notifications = new();

    bool ICollectionNotificationState.IsNotifying => _notifications.IsActive;

    private protected CollectionNotificationScope<
        ObservableSetChangingEventHandler<TItem>,
        ObservableSetChangedEventHandler<TItem>
    >.Lease BeginOperation()
    {
        return _notifications.Enter(SetChanging, SetChanged, CollectionChanged, PropertyChanged);
    }

    private readonly bool _canSkipHooks;
    private bool CanSkipNotifications =>
        _canSkipHooks
        && _notifications.Changing is null
        && _notifications.Changed is null
        && _notifications.Collection is null
        && _notifications.Property is null;

    // 只检查已存在的有序集实例, 不创建允许空项的有序集.
#pragma warning disable CS8714
    private int GetItemIndex(TItem item)
    {
        if (SourceSet is OrderedHashSet<TItem> ordered)
            return ordered.IndexOf(item);
        var index = 0;
        foreach (var storedItem in SourceSet)
        {
            if (Comparer.Equals(storedItem, item))
                return index;
            index++;
        }
        return -1;
    }

    private TItem GetStoredItem(TItem item)
    {
        if (SourceSet is OrderedHashSet<TItem> ordered)
            return ordered.GetAt(ordered.IndexOf(item));
        if (SourceSet is HashSet<TItem> hashSet && hashSet.TryGetValue(item, out var stored))
            return stored;
        foreach (var storedItem in SourceSet)
            if (Comparer.Equals(storedItem, item))
                return storedItem;
        throw new InvalidOperationException(
            "The source set comparer must match the supplied comparer."
        );
    }

#pragma warning restore CS8714

    #region ISet

    /// <inheritdoc/>
    public int Count => SourceSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceSet.IsReadOnly;

    /// <inheritdoc cref="HashSet{T}.Comparer"/>
    public IEqualityComparer<TItem> Comparer { get; }

    #region Change

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        using var operation = BeginOperation();
        if (CanSkipNotifications)
            return SourceSet.Add(item);
        if (SourceSet.Contains(item))
            return false;
        var list = new SingleItemReadOnlyList<TItem>(item);
        var args = OnSetAdding(list);
        SourceSet.Add(item);
        OnSetAdded(args, list);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        using var operation = BeginOperation();
        if (CanSkipNotifications)
            return SourceSet.Remove(item);
        if (SourceSet.Count == 0)
            return false;
        if (SourceSet.Contains(item) is false)
            return false;
        item = GetStoredItem(item);
        var list = new SingleItemReadOnlyList<TItem>(item);
        var args = OnSetRemoving(list, out var removeIndex);
        SourceSet.Remove(item);
        OnSetRemoved(args, list, removeIndex);
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var operation = BeginOperation();
        if (SourceSet.Count == 0)
            return;
        OnSetClearing();
        SourceSet.Clear();
        OnSetCleared();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(other);

        var snapshot = other.ToArray();
        if (CanSkipNotifications)
        {
            SourceSet.IntersectWith(snapshot);
            return;
        }
        var otherItems = Array.AsReadOnly(snapshot);
        var oldItems = new ReadOnlyList<TItem>(SourceSet.Except(otherItems, Comparer));
        if (oldItems.Count == 0)
            return;

        var args = OnSetOperating(
            SetChangeAction.Intersect,
            otherItems,
            null,
            oldItems,
            out var removeIndexs
        );
        SourceSet.IntersectWith(otherItems);
        OnSetOperated(args, SetChangeAction.Intersect, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(other);

        var snapshot = other.ToArray();
        if (CanSkipNotifications)
        {
            SourceSet.ExceptWith(snapshot);
            return;
        }
        var otherItems = Array.AsReadOnly(snapshot);
        var oldItems = new ReadOnlyList<TItem>(SourceSet.Intersect(otherItems, Comparer));
        if (oldItems.Count == 0)
            return;

        var args = OnSetOperating(
            SetChangeAction.Except,
            otherItems,
            null,
            oldItems,
            out var removeIndexs
        );
        SourceSet.ExceptWith(otherItems);
        OnSetOperated(args, SetChangeAction.Except, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(other);

        var snapshot = other.ToArray();
        if (CanSkipNotifications)
        {
            SourceSet.SymmetricExceptWith(snapshot);
            return;
        }
        var otherItems = Array.AsReadOnly(snapshot);
        var oldItems = new ReadOnlyList<TItem>(SourceSet.Intersect(otherItems, Comparer));
        var newItems = new ReadOnlyList<TItem>(otherItems.Except(oldItems, Comparer));
        if (oldItems.Count == 0 && newItems.Count == 0)
            return;

        var args = OnSetOperating(
            SetChangeAction.SymmetricExcept,
            otherItems,
            newItems,
            oldItems,
            out var removeIndexs
        );
        SourceSet.SymmetricExceptWith(otherItems);
        OnSetOperated(
            args,
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
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(other);
        var snapshot = other.ToArray();
        if (CanSkipNotifications)
        {
            SourceSet.UnionWith(snapshot);
            return;
        }
        var otherItems = Array.AsReadOnly(snapshot);
        var newItems = new ReadOnlyList<TItem>(otherItems.Except(SourceSet, Comparer));
        if (newItems.Count == 0)
            return;
        var args = OnSetOperating(
            SetChangeAction.Union,
            otherItems,
            newItems,
            null,
            out var removeIndexs
        );
        SourceSet.UnionWith(otherItems);
        OnSetOperated(args, SetChangeAction.Union, otherItems, newItems, null, removeIndexs);
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

    #endregion ISet

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>事件参数</returns>
    protected virtual NotifySetChangeEventArgs<TItem>? OnSetAdding(IList<TItem> items)
    {
        if (_notifications.Changing is not null)
            return OnSetChanging(new(SetChangeAction.Add, items));
        return null;
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <param name="removeIndex">项目索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifySetChangeEventArgs<TItem>? OnSetRemoving(
        IList<TItem> items,
        out int removeIndex
    )
    {
        removeIndex = -1;
        NotifySetChangeEventArgs<TItem>? args = null;
        if (_notifications.Changing is not null)
            args = OnSetChanging(new(SetChangeAction.Remove, items));
        if (_notifications.Collection is not null)
            removeIndex = GetItemIndex(items[0]);
        return args;
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
    protected virtual void OnSetClearing()
    {
        if (_notifications.Changing is not null)
            OnSetChanging(NotifySetChangeEventArgs<TItem>.Cache_Clear);
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="action">行动</param>
    /// <param name="otherItems">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="removeIndexs">删除项目索引集合</param>
    /// <returns>事件参数</returns>
    protected virtual NotifySetChangeEventArgs<TItem>? OnSetOperating(
        SetChangeAction action,
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems,
        out IList<int> removeIndexs
    )
    {
        removeIndexs = null!;
        NotifySetChangeEventArgs<TItem>? args = null;
        if (_notifications.Changing is not null)
            args = OnSetChanging(new(action, otherItems, newItems, oldItems));
        if (_notifications.Collection is not null && oldItems is not null)
        {
            removeIndexs = new List<int>();
            var removeItems = oldItems.ToHashSet(Comparer);
            foreach (var (e, i) in SourceSet.ReverseWithIndex())
            {
                if (removeItems.Remove(e))
                {
                    removeIndexs.Add(i);
                    if (removeItems.Count == 0)
                        break;
                }
            }
        }
        return args;
    }

    /// <summary>
    /// 集合改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual NotifySetChangeEventArgs<TItem>? OnSetChanging(
        NotifySetChangeEventArgs<TItem> args
    )
    {
        _notifications.Changing?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableSetChangingEventHandler<TItem>? SetChanging;

    #endregion SetChanging

    #region SetChanged

    /// <summary>
    /// 集合添加键值对后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="items">键值对</param>
    protected virtual void OnSetAdded(NotifySetChangeEventArgs<TItem>? args, IList<TItem> items)
    {
        if (_notifications.Changed is not null)
            OnSetChanged(args ?? new(SetChangeAction.Add, items));
        if (_notifications.Collection is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Add, (IList)items, GetItemIndex(items[0]))
            );
        OnPropertyChanged(PropertyChangedArgsCache.Count);
    }

    /// <summary>
    /// 集合删除项目后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="items">键值对</param>
    /// <param name="removeIndex">删除项目的索引</param>
    protected virtual void OnSetRemoved(
        NotifySetChangeEventArgs<TItem>? args,
        IList<TItem> items,
        int removeIndex
    )
    {
        if (_notifications.Changed is not null)
            OnSetChanged(args ?? new(SetChangeAction.Remove, items));
        if (_notifications.Collection is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, items[0], removeIndex));
        OnPropertyChanged(PropertyChangedArgsCache.Count);
    }

    /// <summary>
    /// 集合清理后
    /// </summary>
    protected virtual void OnSetCleared()
    {
        if (_notifications.Changed is not null)
            OnSetChanged(NotifySetChangeEventArgs<TItem>.Cache_Clear);
        if (_notifications.Collection is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnPropertyChanged(PropertyChangedArgsCache.Count);
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="action">行动</param>
    /// <param name="otherItems">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="removeIndexs">删除项目集合</param>
    protected virtual void OnSetOperated(
        NotifySetChangeEventArgs<TItem>? args,
        SetChangeAction action,
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems,
        IList<int> removeIndexs
    )
    {
        if (_notifications.Changed is not null)
            OnSetChanged(args ?? new(action, otherItems, newItems, oldItems));
        if (_notifications.Collection is not null)
        {
            if (SourceSet is HashSet<TItem> or SortedSet<TItem>)
            {
                // 非追加有序后端的批量运算使用重置, 避免伪造逐项插入索引.
                OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
                OnPropertyChanged(PropertyChangedArgsCache.Count);
                return;
            }
            if (oldItems is not null)
            {
                foreach (var (e, i) in oldItems.Reverse().Zip(removeIndexs))
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, e, index: i));
            }
            if (newItems is not null)
            {
                var index = SourceSet.Count - newItems.Count;
                foreach (var item in newItems)
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index++));
            }
        }
        OnPropertyChanged(PropertyChangedArgsCache.Count);
    }

    /// <summary>
    /// 当集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanged(NotifySetChangeEventArgs<TItem> args)
    {
        _notifications.Changed?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableSetChangedEventHandler<TItem>? SetChanged;
    #endregion SetChanged

    /// <summary>
    /// 当集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        _notifications.Collection?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// 属性已改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        _notifications.Property?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
