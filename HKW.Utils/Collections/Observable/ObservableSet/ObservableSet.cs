using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly HashSet<T> _set;

    /// <inheritdoc/>
    public IEqualityComparer<T>? Comparer => _set.Comparer;

    /// <inheritdoc/>
    public bool NotifySetModifies { get; set; }

    #region Ctor

    /// <inheritdoc/>
    public ObservableSet()
    {
        _set = new();
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public ObservableSet(int capacity)
    {
        _set = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEqualityComparer<T> comparer)
    {
        _set = new(comparer);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSet(IEnumerable<T> collection)
    {
        _set = new(collection);
    }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
    {
        _set = new(collection, comparer);
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T>? comparer)
    {
        _set = new(capacity, comparer);
    }

    #endregion Ctor

    #region ISet

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_set).Count;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public bool Add(T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnSetAdding(list))
            return false;
        var result = ((ISet<T>)_set).Add(item);
        if (result)
            OnSetAdded(list);
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnSetRemoving(list))
            return false;
        var result = ((ICollection<T>)_set).Remove(item);
        if (result)
            OnSetRemoved(list);
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        if (OnSetClearing())
            return;
        ((ICollection<T>)_set).Clear();
        OnSetCleared();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        var oldItems = new SimpleReadOnlyList<T>(_set.Except(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        if (OnSetOperating(SetChangeAction.Intersect, otherItems, null, oldItems))
            return;
        ((ISet<T>)_set).IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Intersect, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        var oldItems = new SimpleReadOnlyList<T>(_set.Intersect(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        if (OnSetOperating(SetChangeAction.Except, otherItems, null, oldItems))
            return;
        ((ISet<T>)_set).IntersectWith(otherItems);
        OnSetOperated(SetChangeAction.Except, otherItems, null, oldItems);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherItems = new SimpleReadOnlyList<T>(other);
        var oldItems = new SimpleReadOnlyList<T>(other.Intersect(_set));
        var newItems = new SimpleReadOnlyList<T>(_set.Union(other).Except(oldItems).Except(_set));
        if (OnSetOperating(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems))
            return;
        ((ISet<T>)_set).SymmetricExceptWith(otherItems);
        OnSetOperated(SetChangeAction.SymmetricExcept, otherItems, newItems, oldItems);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        var newItems = new SimpleReadOnlyList<T>(_set.Except(other));
        var otherItems = new SimpleReadOnlyList<T>(other);
        if (OnSetOperating(SetChangeAction.Union, otherItems, newItems, null))
            return;
        ((ISet<T>)_set).UnionWith(otherItems);
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

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetAdding(IList<T> items)
    {
        if (SetChanging is null)
            return false;
        return OnSetChanging(new(SetChangeAction.Add, items));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="items">键值对</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetRemoving(IList<T> items)
    {
        if (SetChanging is null)
            return false;
        return OnSetChanging(new(SetChangeAction.Remove, items));
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetClearing()
    {
        if (SetChanging is null)
            return false;
        return OnSetChanging(new(SetChangeAction.Clear));
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="action">行动</param>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetOperating(
        SetChangeAction action,
        IList<T> other,
        IList<T>? newItems,
        IList<T>? oldTiems
    )
    {
        if (SetChanging is null)
            return false;
        return OnSetChanging(new(action, other, newItems, oldTiems));
    }

    /// <summary>
    /// 集合改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual bool OnSetChanging(NotifySetChangingEventArgs<T> args)
    {
        SetChanging?.Invoke(args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifySetChangingEventArgs<T>>? SetChanging;

    #endregion SetChanging

    #region SetChanged

    /// <summary>
    /// 集合添加键值对后
    /// </summary>
    /// <param name="items">键值对</param>
    private void OnSetAdded(IList<T> items)
    {
        if (SetChanged is not null)
            OnSetChanged(new(SetChangeAction.Add, items));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)items));
    }

    /// <summary>
    /// 集合删除项目后
    /// </summary>
    /// <param name="items">键值对</param>
    private void OnSetRemoved(IList<T> items)
    {
        if (SetChanged is not null)
            OnSetChanged(new(SetChangeAction.Remove, items));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (IList)items));
    }

    /// <summary>
    /// 集合清理后
    /// </summary>
    private void OnSetCleared()
    {
        if (SetChanged is not null)
            OnSetChanged(new(SetChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 集合运算前
    /// </summary>
    /// <param name="action">行动</param>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetOperated(
        SetChangeAction action,
        IList<T> other,
        IList<T>? newItems,
        IList<T>? oldTiems
    )
    {
        if (SetChanged is not null)
            OnSetChanged(new(action, other, newItems, oldTiems));
        if (CollectionChanged is not null)
        {
            if (newItems is not null)
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)newItems));
            if (oldTiems is not null)
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (IList)oldTiems));
        }
    }

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanged(NotifySetChangedEventArgs<T> args)
    {
        SetChanged?.Invoke(args);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifySetChangedEventArgs<T>>? SetChanged;

    #endregion SetChanged

    #region CollectionChanged

    /// <summary>
    /// 集合已改变前
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
    /// 属性已改变前
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
