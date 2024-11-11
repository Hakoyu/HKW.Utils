using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中集合
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableSet<TItem, TSet>
    : ReactiveObjectX,
        ISet<TItem>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    public ObservableSelectableSet(TSet set)
    {
        BaseSet = set;
    }

    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableSet(TSet set, TItem seletedItem)
        : this(set)
    {
        SelectedItem = seletedItem;
    }

    /// <inheritdoc/>
    public TSet BaseSet { get; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public TItem? SelectedItem { get; set; }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ISet<TItem>)BaseSet).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ISet<TItem>)BaseSet).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        return ((ISet<TItem>)BaseSet).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ISet<TItem>)BaseSet).Clear();
        SelectedItem = default;
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ISet<TItem>)BaseSet).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ISet<TItem>)BaseSet).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)BaseSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = BaseSet.Remove(item);
        if (result && item?.Equals(SelectedItem) is true)
            SelectedItem = default;
        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        BaseSet.ExceptWith(other);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        BaseSet.IntersectWith(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return BaseSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return BaseSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return BaseSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return BaseSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return BaseSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return BaseSet.SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        BaseSet.SymmetricExceptWith(other);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        BaseSet.UnionWith(other);
    }

    void ICollection<TItem>.Add(TItem item)
    {
        BaseSet.Add(item);
    }

    #endregion
}
