using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可选择组队长
/// </summary>
public partial class SelectionGroupLeader : ReactiveObjectX
{
    /// <inheritdoc/>
    /// <param name="isSelected">已选中</param>
    public SelectionGroupLeader(bool isSelected = false)
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
    /// 转换器
    /// </summary>
    public ObservablePropertyWrapper<SelectionGroupLeader, bool?> Wrapper { get; }
}
