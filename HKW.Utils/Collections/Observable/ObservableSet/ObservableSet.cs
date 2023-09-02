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
    private readonly HashSet<T> r_set;

    /// <inheritdoc/>
    public IEqualityComparer<T>? Comparer => r_set.Comparer;

    /// <inheritdoc/>
    public bool NotifySetModifies { get; set; }

    #region Ctor

    /// <inheritdoc/>
    public ObservableSet()
    {
        r_set = new();
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public ObservableSet(int capacity)
    {
        r_set = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEqualityComparer<T> comparer)
    {
        r_set = new(comparer);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSet(IEnumerable<T> collection)
    {
        r_set = new(collection);
    }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
    {
        r_set = new(collection, comparer);
    }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T>? comparer)
    {
        r_set = new(capacity, comparer);
    }

    #endregion Ctor

    #region ISet

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)r_set).Count;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<T>)r_set).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public bool Add(T item)
    {
        if (OnSetAdding(item))
            return false;
        var result = ((ISet<T>)r_set).Add(item);
        if (result)
        {
            OnSetAdded(item);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (OnSetRemoving(item))
            return false;
        var result = ((ICollection<T>)r_set).Remove(item);
        if (result)
        {
            OnSetRemoved(item);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var oldCount = Count;
        if (OnSetClearing())
            return;
        ((ICollection<T>)r_set).Clear();
        OnSetCleared();
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法必须启用 <see cref="NotifySetModifies"/> 才能触发 <see cref="CollectionChanged"/> </summary>
    public void IntersectWith(IEnumerable<T> other)
    {
        List<T>? oldItems = null;
        if (NotifySetModifies)
        {
            oldItems = r_set.Except(other).ToList();
        }
        var oldCount = Count;
        if (OnSetIntersecting(other, null, oldItems))
            return;
        ((ISet<T>)r_set).IntersectWith(other);
        OnSetIntersected(other, null, oldItems);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法必须启用 <see cref="NotifySetModifies"/> 才能触发 <see cref="CollectionChanged"/> </summary>
    public void ExceptWith(IEnumerable<T> other)
    {
        List<T>? oldItems = null;
        if (NotifySetModifies)
        {
            oldItems = r_set.Intersect(other).ToList();
        }
        var oldCount = Count;
        if (OnSetExcepting(other, null, oldItems))
            return;
        ((ISet<T>)r_set).ExceptWith(other);
        OnSetExcepted(other, null, oldItems);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法必须启用 <see cref="NotifySetModifies"/> 才能触发 <see cref="CollectionChanged"/> </summary>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        List<T>? newItems = null;
        List<T>? oldItems = null;
        if (NotifySetModifies)
        {
            oldItems = other.Intersect(r_set).ToList();
            var result = r_set.Union(other).Except(oldItems);
            newItems = result.Except(r_set).ToList();
        }
        var oldCount = Count;
        if (OnSetSymmetricExcepting(other, newItems, oldItems))
            return;
        ((ISet<T>)r_set).SymmetricExceptWith(other);
        OnSetSymmetricExcepted(other, newItems, oldItems);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法必须启用 <see cref="NotifySetModifies"/> 才能触发 <see cref="CollectionChanged"/> </summary>
    public void UnionWith(IEnumerable<T> other)
    {
        List<T>? newItems = null;
        if (NotifySetModifies)
        {
            newItems = other.Except(r_set).ToList();
        }
        var oldCount = Count;
        if (OnSetUnioning(other, newItems, null))
            return;
        ((ISet<T>)r_set).UnionWith(other);
        OnSetUnioned(other, newItems, null);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        if (OnSetAdding(item))
            return;
        ((ICollection<T>)r_set).Add(item);
        OnSetAdded(item);
        OnCountPropertyChanged();
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)r_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)r_set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)r_set).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)r_set).SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_set).GetEnumerator();
    }

    #endregion ISet

    #region SetChanging

    /// <summary>
    /// 集合添加项目前
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetAdding(T item)
    {
        return OnSetChanging(new(SetChangeAction.Add, item));
    }

    /// <summary>
    /// 集合删除项目前
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetRemoving(T item)
    {
        return OnSetChanging(new(SetChangeAction.Remove, item));
    }

    /// <summary>
    /// 集合清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetClearing()
    {
        return OnSetChanging(new(SetChangeAction.Clear));
    }

    /// <summary>
    /// 集合相交前
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetIntersecting(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        return OnSetChanging(new(SetChangeAction.Intersect, other, newItems, oldTiems));
    }

    /// <summary>
    /// 集合排除前
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetExcepting(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        return OnSetChanging(new(SetChangeAction.Except, other, newItems, oldTiems));
    }

    /// <summary>
    /// 集合相同排除前
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetSymmetricExcepting(
        IEnumerable<T> other,
        IList<T>? newItems,
        IList<T>? oldTiems
    )
    {
        return OnSetChanging(new(SetChangeAction.SymmetricExcept, other, newItems, oldTiems));
    }

    /// <summary>
    /// 集合合并前
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetUnioning(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        return OnSetChanging(new(SetChangeAction.Union, other, newItems, oldTiems));
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
    /// 集合添加条目后
    /// </summary>
    /// <param name="item">条目</param>
    private void OnSetAdded(T item)
    {
        OnSetChanged(new(SetChangeAction.Add, item));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
    }

    /// <summary>
    /// 集合删除项目后
    /// </summary>
    /// <param name="item">条目</param>
    private void OnSetRemoved(T item)
    {
        OnSetChanged(new(SetChangeAction.Remove, item));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item));
    }

    /// <summary>
    /// 集合清理后
    /// </summary>
    private void OnSetCleared()
    {
        OnSetChanged(new(SetChangeAction.Clear));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 集合相交后
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetIntersected(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        OnSetChanged(new(SetChangeAction.Intersect, other, newItems, oldTiems));
        OnSetModifies(newItems, oldTiems);
    }

    /// <summary>
    /// 集合排除后
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetExcepted(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        OnSetChanged(new(SetChangeAction.Except, other, newItems, oldTiems));
        OnSetModifies(newItems, oldTiems);
    }

    /// <summary>
    /// 集合相同排除后
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetSymmetricExcepted(
        IEnumerable<T> other,
        IList<T>? newItems,
        IList<T>? oldTiems
    )
    {
        OnSetChanged(new(SetChangeAction.SymmetricExcept, other, newItems, oldTiems));
        OnSetModifies(newItems, oldTiems);
    }

    /// <summary>
    /// 集合合并后
    /// </summary>
    /// <param name="other">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldTiems">旧项目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetUnioned(IEnumerable<T> other, IList<T>? newItems, IList<T>? oldTiems)
    {
        OnSetChanged(new(SetChangeAction.Union, other, newItems, oldTiems));
        OnSetModifies(newItems, oldTiems);
    }

    private void OnSetModifies(IList<T>? newItems, IList<T>? oldTiems)
    {
        if (newItems is not null)
        {
            foreach (var item in newItems)
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
        }
        if (oldTiems is not null)
        {
            foreach (var item in oldTiems)
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item));
        }
    }

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanged(NotifySetChangedEventArgs<T> args)
    {
        SetChanged?.Invoke(args);
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifySetChangedEventArgs<T>>? SetChanged;

    #endregion SetChanged

    #region PropertyChanged

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountPropertyChanged()
    {
        OnPropertyChanged(nameof(Count));
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

    #region CollectionChanged

    /// <summary>
    /// 集合已改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(null, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged
}
