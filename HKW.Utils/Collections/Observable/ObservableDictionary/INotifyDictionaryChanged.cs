using HKW.HKWUtils.Events;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用通知字典改变后接口
/// </summary>
public interface INotifyDictionaryChanged
{
    /// <summary>
    /// 字典改变后事件
    /// </summary>
    public event XEventHandler<NotifyDictionaryChangedEventArgs<object, object>>? DictionaryChanged;
}
