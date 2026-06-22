using System.Collections;
using System.Diagnostics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中集合包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableSetWrapper<TItem, TSet>
    : ReactiveObjectX,
        ISet<TItem>,
        ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    public ObservableSelectableSetWrapper(TSet set)
    {
        SourceSet = set;
    }

    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableSetWrapper(TSet set, TItem seletedItem)
        : this(set)
    {
        SelectedItem = seletedItem;
    }

    /// <inheritdoc/>
    public TSet SourceSet { get; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public TItem? SelectedItem { get; set; }

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ISet<TItem>)SourceSet).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ISet<TItem>)SourceSet).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(TItem item)
    {
        return ((ISet<TItem>)SourceSet).Add(item);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = SourceSet.Remove(item);
        if (result && item?.Equals(SelectedItem) is true)
            SelectedItem = default;
        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TItem> other)
    {
        SourceSet.ExceptWith(other);
        if (SelectedItem is not null && SourceSet.Contains(SelectedItem) is false)
            SelectedItem = default;
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TItem> other)
    {
        SourceSet.IntersectWith(other);
        if (SelectedItem is not null && SourceSet.Contains(SelectedItem) is false)
            SelectedItem = default;
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        SourceSet.SymmetricExceptWith(other);
        if (SelectedItem is not null && SourceSet.Contains(SelectedItem) is false)
            SelectedItem = default;
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TItem> other)
    {
        SourceSet.UnionWith(other);
    }

    void ICollection<TItem>.Add(TItem item)
    {
        SourceSet.Add(item);
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
    public void Clear()
    {
        ((ISet<TItem>)SourceSet).Clear();
        SelectedItem = default;
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ISet<TItem>)SourceSet).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ISet<TItem>)SourceSet).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)SourceSet).GetEnumerator();
    }

    #endregion
}
