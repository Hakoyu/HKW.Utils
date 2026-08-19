using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSelectableSetWrapper<TItem, TSet> : ObservableSetWrapper<TItem, TSet>
    where TItem : notnull
    where TSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="comparer">比较器, 必须与 <paramref name="set"/> 的比较器相同</param>
    public ObservableSelectableSetWrapper(TSet set, IEqualityComparer<TItem>? comparer = null)
        : base(set, comparer) { }

    private bool _hasSelection;

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection => _hasSelection;

    private TItem _selectedItem = default!;

    /// <summary>
    /// 选中的项目
    /// </summary>
    public TItem SelectedItem
    {
        get => _selectedItem;
        set => SetSelectedItem(value);
    }

    /// <inheritdoc/>
    protected override void OnSetRemoved(
        NotifySetChangeEventArgs<TItem>? args,
        IList<TItem> items,
        int removeIndex
    )
    {
        if (_hasSelection is true && Comparer.Equals(_selectedItem, items[0]))
            ClearSelection();

        base.OnSetRemoved(args, items, removeIndex);
    }

    /// <inheritdoc/>
    protected override void OnSetCleared()
    {
        ClearSelection();
        base.OnSetCleared();
    }

    /// <inheritdoc/>
    protected override void OnSetOperated(
        NotifySetChangeEventArgs<TItem>? args,
        SetChangeAction action,
        IList<TItem> otherItems,
        IList<TItem>? newItems,
        IList<TItem>? oldItems,
        IList<int> removeIndexs
    )
    {
        if (_hasSelection is true && oldItems?.Contains(_selectedItem, Comparer) is true)
        {
            ClearSelection();
        }

        base.OnSetOperated(args, action, otherItems, newItems, oldItems, removeIndexs);
    }

    private void SetSelectedItem(TItem item)
    {
        if (Comparer.Equals(_selectedItem, item))
            return;

        _selectedItem = item;
        _hasSelection = Contains(item);
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(HasSelection));
        OnSelectionChanged();
    }

    /// <summary>
    /// 清除选择
    /// </summary>
    protected void ClearSelection()
    {
        if (
            _hasSelection is false
            && EqualityComparer<TItem>.Default.Equals(_selectedItem, default!)
        )
            return;

        _selectedItem = default!;
        _hasSelection = false;
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(HasSelection));
        OnSelectionChanged();
    }

    /// <summary>
    /// 选择状态改变后
    /// </summary>
    protected virtual void OnSelectionChanged() { }
}
