using HKW.HKWUtils.Events;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用通知列表改变前接口
/// </summary>
public interface INotifyListChanging
{
    /// <summary>
    /// 列表改变前事件
    /// </summary>
    public event XCancelEventHandler<NotifyListChangingEventArgs<object>>? ListChanging;
}
