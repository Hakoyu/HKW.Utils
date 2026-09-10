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
        set => SetSelectedIndex(value);
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
            var index = SourceList.IndexOf(value);
            if (index < 0)
            {
                ClearSelection();
                return;
            }

            SetSelectedIndex(index);
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
            SetSelectedIndex(_selectedIndex + 1);

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
            SetSelectedIndex(_selectedIndex - 1);
        else if (_selectedIndex == index)
            ClearSelection();

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
        ClearSelection();

        base.OnListCleared();
    }

    private void SetSelectedIndex(int index)
    {
        if (index == -1)
        {
            ClearSelection();
            return;
        }

        ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(this, index);

        if (_selectedIndex == index)
            return;

        _selectedIndex = index;
        _selectedItem = SourceList[index];
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(SelectedIndex));
        OnPropertyChanged(nameof(HasSelection));
    }

    private void UpdateSelectedItem()
    {
        _selectedItem = SourceList[_selectedIndex];
        OnPropertyChanged(nameof(SelectedItem));
    }

    private void ClearSelection()
    {
        if (_selectedIndex == -1 && EqualityComparer<TItem>.Default.Equals(_selectedItem, default!))
            return;

        _selectedIndex = -1;
        _selectedItem = default!;
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(SelectedIndex));
        OnPropertyChanged(nameof(HasSelection));
    }
}
