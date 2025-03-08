using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中列表
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableList<T> : ObservableSelectableListWrapper<T, List<T>>
{
    /// <inheritdoc/>
    public ObservableSelectableList()
        : base([]) { }

    /// <inheritdoc/>
    /// <param name="seletedIndex">选中项索引</param>
    public ObservableSelectableList(int seletedIndex)
        : base([])
    {
        SelectedIndex = seletedIndex;
    }

    /// <inheritdoc/>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableList(T seletedItem)
        : base([])
    {
        SelectedItem = seletedItem;
    }

    /// <inheritdoc/>
    /// <param name="items">项目</param>
    /// <param name="seletedIndex">选中项索引</param>
    public ObservableSelectableList(IEnumerable<T> items, int seletedIndex)
        : base(items.ToList())
    {
        SelectedIndex = seletedIndex;
    }

    /// <inheritdoc/>
    /// <param name="items">项目</param>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableList(IEnumerable<T> items, T seletedItem)
        : base(items.ToList())
    {
        SelectedItem = seletedItem;
    }
}
