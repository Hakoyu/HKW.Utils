using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察集合
/// </summary>
[DebuggerDisplay("Count = {Count}")]
public class ObservableSet<T> : IObservableSet<T>
    where T : notnull
{
    /// <summary>
    /// 源集合
    /// </summary>
    private readonly HashSet<T> r_set;

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
    #endregion

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<T>)r_set).Count;

    /// <inheritdoc/>
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
    /// <summary>注意: 此方法不触发 <see cref="INotifyCollectionChanged"/></summary>
    public void IntersectWith(IEnumerable<T> other)
    {
        var oldCount = Count;
        if (OnSetIntersecting(other))
            return;
        ((ISet<T>)r_set).IntersectWith(other);
        OnSetIntersected(other);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法不触发 <see cref="INotifyCollectionChanged"/></summary>
    public void ExceptWith(IEnumerable<T> other)
    {
        var oldCount = Count;
        if (OnSetExcepting(other))
            return;
        ((ISet<T>)r_set).ExceptWith(other);
        OnSetExcepted(other);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法不触发 <see cref="INotifyCollectionChanged"/></summary>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var oldCount = Count;
        if (OnSetSymmetricExcepting(other))
            return;
        ((ISet<T>)r_set).SymmetricExceptWith(other);
        OnSetSymmetricExcepted(other);
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    /// <summary>注意: 此方法不触发 <see cref="INotifyCollectionChanged"/></summary>
    public void UnionWith(IEnumerable<T> other)
    {
        var oldCount = Count;
        if (OnSetUnioning(other))
            return;
        ((ISet<T>)r_set).UnionWith(other);
        OnSetUnioned(other);
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
    #endregion
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
    #endregion
    #region SetChanging
    /// <summary>
    /// 集合添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetAdding(T item)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Add, item));
        return false;
    }

    /// <summary>
    /// 集合删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetRemoving(T item)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Remove, item));
        return false;
    }

    /// <summary>
    /// 集合清理时
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetClearing()
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Clear));
        return false;
    }

    /// <summary>
    /// 集合相交
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetIntersecting(IEnumerable<T> collection)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Intersect, collection));
        return false;
    }

    /// <summary>
    /// 集合除外
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetExcepting(IEnumerable<T> collection)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Except, collection));
        return false;
    }

    /// <summary>
    /// 集合对称除外
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetSymmetricExcepting(IEnumerable<T> collection)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.SymmetricExcept, collection));
        return false;
    }

    /// <summary>
    /// 集合合并
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnSetUnioning(IEnumerable<T> collection)
    {
        if (SetChanging is not null)
            return OnSetChanging(new(NotifySetChangeAction.Union, collection));
        return false;
    }

    /// <summary>
    /// 集合改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual bool OnSetChanging(NotifySetChangingEventArgs<T> args)
    {
        SetChanging?.Invoke(this, args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event NotifySetChangingEventHandler<T>? SetChanging;
    #endregion
    #region SetChanged
    /// <summary>
    /// 集合已添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    private void OnSetAdded(T item)
    {
        if (SetChanged is not null)
            OnSetChanged(new(NotifySetChangeAction.Add, item));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
    }

    /// <summary>
    /// 集合已删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    private void OnSetRemoved(T item)
    {
        if (SetChanged is not null)
            OnSetChanged(new(NotifySetChangeAction.Remove, item));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item));
    }

    /// <summary>
    /// 集合已清理时
    /// </summary>
    private void OnSetCleared()
    {
        if (SetChanged is not null)
            OnSetChanged(new(NotifySetChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 集合相交
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetIntersected(IEnumerable<T> collection)
    {
        OnSetChanged(new(NotifySetChangeAction.Intersect, collection));
    }

    /// <summary>
    /// 集合除外
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetExcepted(IEnumerable<T> collection)
    {
        OnSetChanged(new(NotifySetChangeAction.Except, collection));
    }

    /// <summary>
    /// 集合对称除外
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetSymmetricExcepted(IEnumerable<T> collection)
    {
        OnSetChanged(new(NotifySetChangeAction.SymmetricExcept, collection));
    }

    /// <summary>
    /// 集合合并
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private void OnSetUnioned(IEnumerable<T> collection)
    {
        OnSetChanged(new(NotifySetChangeAction.Union, collection));
    }

    /// <summary>
    /// 集合已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnSetChanged(NotifySetChangedEventArgs<T> args)
    {
        SetChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifySetChangedEventHandler<T>? SetChanged;
    #endregion
    #region PropertyChanged
    /// <summary>
    /// 数量已改变时
    /// </summary>
    private void OnCountPropertyChanged()
    {
        if (PropertyChanged is not null)
            OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// 属性已改变时
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
    #region CollectionChanged

    /// <summary>
    /// 集合已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    #endregion
}
