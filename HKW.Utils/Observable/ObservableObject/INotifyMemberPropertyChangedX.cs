namespace HKW.HKWUtils.Observable;

/// <summary>
/// 成员属性改变后事件接口
/// </summary>
public interface INotifyMemberPropertyChangedX
{
    /// <summary>
    /// 成员属性改变后事件
    /// </summary>
    public event MemberPropertyChangedXEventHandler? MemberPropertyChangedX;
}
