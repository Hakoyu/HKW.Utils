using HKW.HKWUtils.Events;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表改变后接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifyListChanged<T>
{
    /// <summary>
    /// 列表改变后事件
    /// </summary>
    public event XEventHandler<NotifyListChangedEventArgs<T>>? ListChanged;
}
