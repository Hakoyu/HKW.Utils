using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TList">集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSelectableListWrapper<TItem, TList> : ObservableListWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    public ObservableSelectableListWrapper(TList list)
        : base(list) { }

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection => _selectedIndex >= 0 && _selectedIndex < Count;

    private int _selectedIndex = -1;

    /// <summary>
    /// 选中的索引
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            using var operation = BeginOperation();
            SetSelectedIndex(value);
        }
    }

    private TItem _selectedItem = default!;

    /// <summary>
    /// 选中的项目
    /// </summary>
#pragma warning disable S4275
    public TItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            using var operation = BeginOperation();
            SetSelectedIndex(SourceList.IndexOf(value));
        }
    }
#pragma warning restore S4275

    /// <inheritdoc/>
    protected override void OnListAdded(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (_selectedIndex >= index)
        {
            _selectedIndex++;
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        }

        base.OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    protected override void OnListRemoved(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (_selectedIndex > index)
        {
            _selectedIndex--;
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        }
        else if (_selectedIndex == index)
            ClearSelectionCore();

        base.OnListRemoved(args, item, index);
    }

    /// <inheritdoc/>
    protected override void OnListReplaced(
        NotifyListChangeEventArgs<TItem>? args,
        TItem newItem,
        TItem oldItem,
        int index
    )
    {
        if (_selectedIndex == index)
            UpdateSelectedItem();

        base.OnListReplaced(args, newItem, oldItem, index);
    }

    /// <inheritdoc/>
    protected override void OnListCleared()
    {
        ClearSelectionCore();

        base.OnListCleared();
    }

    private void SetSelectedIndex(int index)
    {
        if (index == -1)
        {
            ClearSelectionCore();
            return;
        }

        ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(this, index);

        if (_selectedIndex == index)
            return;

        var item = SourceList[index];
        var hadSelection = HasSelection;
        _selectedIndex = index;
        _selectedItem = item;
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
        OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        if (hadSelection is false)
            OnPropertyChanged(PropertyChangedArgsCache.HasSelection);
    }

    private void UpdateSelectedItem()
    {
        _selectedItem = SourceList[_selectedIndex];
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
    }

    /// <summary>
    /// 清除选择, 不删除集合中的项目. 默认值仍可通过选择属性选中.
    /// </summary>
    public void ClearSelection()
    {
        using var operation = BeginOperation();
        ClearSelectionCore();
    }

    private void ClearSelectionCore()
    {
        if (_selectedIndex == -1)
            return;

        _selectedIndex = -1;
        _selectedItem = default!;
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
        OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        OnPropertyChanged(PropertyChangedArgsCache.HasSelection);
    }
}
