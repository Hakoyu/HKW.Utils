namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知字典改变前接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface INotifyDictionaryChanging<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 字典改变前事件
    /// </summary>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;
}
