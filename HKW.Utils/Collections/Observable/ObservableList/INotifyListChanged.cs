using HKW.HKWUtils.Events;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用通知列表改变后接口
/// </summary>
public interface INotifyListChanged
{
    /// <summary>
    /// 列表改变后事件
    /// </summary>
    public event XEventHandler<NotifyListChangedEventArgs<object>>? ListChanged;
}
