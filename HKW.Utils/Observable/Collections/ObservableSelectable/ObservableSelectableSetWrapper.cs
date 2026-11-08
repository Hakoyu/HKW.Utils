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
        set
        {
            using var operation = BeginOperation();
            SetSelectedItemCore(value);
        }
    }

    /// <inheritdoc/>
    protected override void OnSetRemoved(
        NotifySetChangeEventArgs<TItem>? args,
        IList<TItem> items,
        int removeIndex
    )
    {
        if (_hasSelection && Comparer.Equals(_selectedItem, items[0]))
            ClearSelectionCore();
        else
            RefreshSelectionIndex();

        base.OnSetRemoved(args, items, removeIndex);
    }

    /// <inheritdoc/>
    protected override void OnSetCleared()
    {
        ClearSelectionCore();
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
        if (_hasSelection && SourceSet.Contains(_selectedItem) is false)
            ClearSelectionCore();
        else
            RefreshSelectionIndex();

        base.OnSetOperated(args, action, otherItems, newItems, oldItems, removeIndexs);
    }

    /// <summary>
    /// 在当前操作作用域内选择项目, 缺失项目清除选择.
    /// </summary>
    private protected void SetSelectedItemCore(TItem item)
    {
        if (SourceSet.Contains(item) is false)
        {
            ClearSelectionCore();
            return;
        }

        if (SourceSet is OrderedHashSet<TItem> ordered)
            item = ordered.GetAt(ordered.IndexOf(item));
        else if (SourceSet is HashSet<TItem> hashSet)
            hashSet.TryGetValue(item, out item!);
        else if (SourceSet is SortedSet<TItem> sortedSet)
            sortedSet.TryGetValue(item, out item!);
        else
            item = GetStoredSelection(item);

        if (_hasSelection && Comparer.Equals(_selectedItem, item))
            return;

        var hadSelection = _hasSelection;
        _selectedItem = item;
        _hasSelection = true;
        var indexChanged = CommitSelectionIndex();
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
        if (hadSelection is false)
            OnPropertyChanged(PropertyChangedArgsCache.HasSelection);
        if (indexChanged)
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        OnSelectionChanged();
    }

    /// <summary>
    /// 清除选择
    /// </summary>
    /// <remarks>
    /// 不删除项目. 默认值仍可通过选择属性选中.
    /// </remarks>
    public void ClearSelection()
    {
        using var operation = BeginOperation();
        ClearSelectionCore();
    }

    /// <summary>
    /// 在当前操作作用域内清除选择.
    /// </summary>
    private protected void ClearSelectionCore()
    {
        if (_hasSelection is false)
            return;

        _selectedItem = default!;
        _hasSelection = false;
        var indexChanged = CommitSelectionIndex();
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
        OnPropertyChanged(PropertyChangedArgsCache.HasSelection);
        if (indexChanged)
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        OnSelectionChanged();
    }

    private TItem GetStoredSelection(TItem item)
    {
        foreach (var stored in SourceSet)
            if (Comparer.Equals(stored, item))
                return stored;
        throw new InvalidOperationException(
            "The source set comparer must match the supplied comparer."
        );
    }

    private int _selectionIndex = -1;

    /// <summary>
    /// 获取有序后端的选择索引, 无索引后端返回 -1.
    /// </summary>
    protected virtual int GetSelectionIndex() => -1;

    private bool CommitSelectionIndex()
    {
        var index = GetSelectionIndex();
        var changed = index != _selectionIndex;
        _selectionIndex = index;
        return changed;
    }

    private void RefreshSelectionIndex()
    {
        if (CommitSelectionIndex())
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
    }

    /// <summary>
    /// 选择状态改变后
    /// </summary>
    protected virtual void OnSelectionChanged() { }
}
