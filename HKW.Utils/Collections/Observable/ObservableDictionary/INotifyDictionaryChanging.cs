using HKW.HKWUtils.Events;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用通知字典改变前的接口
/// </summary>
public interface INotifyDictionaryChanging
{
    /// <summary>
    /// 字典改变前事件
    /// </summary>
    public event XCancelEventHandler<
        NotifyDictionaryChangingEventArgs<object, object>
    >? DictionaryChanging;
}
