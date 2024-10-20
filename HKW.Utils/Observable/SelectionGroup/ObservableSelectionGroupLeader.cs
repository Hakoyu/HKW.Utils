using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测的选择组组长
/// </summary>
public partial class ObservableSelectionGroupLeader : ReactiveObjectX
{
    /// <inheritdoc/>
    /// <param name="isSelected">已选中</param>
    public ObservableSelectionGroupLeader(bool isSelected = false)
    {
        IsSelected = isSelected;
        Wrapper = new(this, nameof(IsSelected), x => x.IsSelected, (x, v) => x.IsSelected = v);
    }

    /// <summary>
    /// 已选中
    /// </summary>
    [ReactiveProperty]
    public bool? IsSelected { get; set; }

    /// <summary>
    /// 包装器
    /// </summary>
    public ObservablePropertyWrapper<ObservableSelectionGroupLeader, bool?> Wrapper { get; }
}
