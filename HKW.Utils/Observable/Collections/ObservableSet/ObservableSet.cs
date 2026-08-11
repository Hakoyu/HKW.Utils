using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSet<T> : IObservableSet<T>, IReadOnlyObservableSet<T>
    where T : notnull
{
    /// <inheritdoc/>
    public ObservableSet()
    {
        Comparer = EqualityComparer<T>.Default;
        _set = new();
    }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
    {
        Comparer = comparer ?? EqualityComparer<T>.Default;
        _set = new(collection, Comparer);
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T>? comparer = null)
    {
        Comparer = comparer ?? EqualityComparer<T>.Default;
        _set = new(capacity, comparer);
    }

    /// <inheritdoc/>
    private readonly OrderedSet<T> _set;

    #region ISet

    /// <inheritdoc/>
    public int Count => _set.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc cref="HashSet{T}.Comparer"/>
    public IEqualityComparer<T> Comparer { get; }

    #region Change

    /// <summary>
    /// 集合改变参数
    /// </summary>
    protected NotifySetChangeEventArgs<T>? SetChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public bool Add(T item)
    {
        if (_set.Contains(item))
            return false;
        var list = new SingleItemReadOnlyList<T>(item);
        OnSetAdding(list);
        _set.Add(item);
        OnSetAdded(list);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (_set.Contains(item) is false)
            return false;
        var list = new SingleItemReadOnlyList<T>(item);
        OnSetRemoving(list, out var removeIndex);
        _set.Remove(item);
        OnSetRemoved(list, removeIndex);
        return true;
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
        var oldItems = new ReadOnlyList<T>(_set.Except(other, Comparer));
        var otherItems = new ReadOnlyList<T>(other);
        OnSetOperating(SetChangeAction.Intersect, otherItems, null, oldItems, out var removeIndexs);
        _set.IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Intersect, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        var oldItems = new ReadOnlyList<T>(_set.Intersect(other, Comparer));
        var otherItems = new ReadOnlyList<T>(other);
        OnSetOperating(SetChangeAction.Except, otherItems, null, oldItems, out var removeIndexs);
        _set.ExceptWith(otherItems);
        OnSetOperated(SetChangeAction.Except, otherItems, null, oldItems, removeIndexs);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherItems = new ReadOnlyList<T>(other);
        var oldItems = new ReadOnlyList<T>(_set.Intersect(otherItems, Comparer));
        var newItems = new ReadOnlyList<T>(otherItems.Except(oldItems, Comparer));
        OnSetOperating(
            SetChangeAction.SymmetricExcept,
            otherItems,
            newItems,
            oldItems,
            out var removeIndexs
        );
        if (other is HashSet<T> otherSet)
            _set.SymmetricExceptWith(otherSet);
        else
            _set.SymmetricExceptWith(otherItems);
        OnSetOperated(
            SetChangeAction.SymmetricExcept,
            otherItems,
            newItems,
            oldItems,
            removeIndexs
        );
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        var otherItems = new ReadOnlyList<T>(other);
        var newItems = new ReadOnlyList<T>(other.Except(_set, Comparer));
        OnSetOperating(SetChangeAction.Union, otherItems, newItems, null, out var removeIndexs);
        _set.UnionWith(otherItems);
        OnSetOperated(SetChangeAction.Union, otherItems, newItems, null, removeIndexs);
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
        return _set.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _set.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _set.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _set.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _set.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return _set.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return _set.SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }

    /// <inheritdoc cref="HashSet{T}.TrimExcess()"/>
    public void TrimExcess()
    {
        if (_set is HashSet<T> set)
            set.TrimExcess();
    }

    /// <inheritdoc cref="HashSet{T}.TrimExcess(int)"/>
    public void TrimExcess(int capacity)
    {
        if (_set is HashSet<T> set)
            set.TrimExcess(capacity);
    }

    #endregion ISet

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="items">键值对</param>
    protected virtual void OnSetAdding(IList<T> items)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Add, items));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <param name="removeIndex">项目索引</param>
    protected virtual void OnSetRemoving(IList<T> items, out int removeIndex)
    {
        if (SetChanging is not null)
            OnSetChanging(new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
            removeIndex = _set.IndexOf(items[0]);
        else
            removeIndex = -1;
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
    protected virtual void OnSetClearing()
    {
        if (SetChanging is not null)
            OnSetChanging(NotifySetChangeEventArgs<T>.Cache_Clear);
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
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems,
        out IList<int> removeIndexs
    )
    {
        if (SetChanging is not null)
            OnSetChanging(new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null && oldItems is not null)
        {
            removeIndexs = new List<int>();
            var removeItems = oldItems.ToHashSet(Comparer);
            foreach (var (e, i) in _set.ReverseWithIndex())
            {
                if (removeItems.Remove(e))
                {
                    removeIndexs.Add(i);
                    if (removeItems.Count == 0)
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
    protected virtual void OnSetChanging(NotifySetChangeEventArgs<T> args)
    {
        SetChangeEventArgs = args;
        SetChanging?.Invoke(this, args);
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
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)items));
        OnCountChanged();
    }

    /// <summary>
    /// 集合删除项目后
    /// </summary>
    /// <param name="items">键值对</param>
    /// <param name="removeIndex">删除项目的索引</param>
    protected virtual void OnSetRemoved(IList<T> items, int removeIndex)
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
            OnSetChanged(NotifySetChangeEventArgs<T>.Cache_Clear);
        if (CollectionChanged is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
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
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems,
        IList<int> removeIndexs
    )
    {
        if (SetChanged is not null)
            OnSetChanged(SetChangeEventArgs ?? new(action, otherItems, newItems, oldItems));
        if (CollectionChanged is not null)
        {
            if (oldItems is not null)
            {
                foreach (var (e, i) in ((IEnumerable<T>)oldItems).Reverse().Zip(removeIndexs))
                {
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, e, index: i));
                }
            }
            if (newItems is not null)
            {
                var index = _set.Count - newItems.Count;
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
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
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
