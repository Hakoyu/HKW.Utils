namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface IObservableDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>,
        IObservableCollection<KeyValuePair<TKey, TValue>>,
        INotifyDictionaryChanged<TKey, TValue>,
        INotifyDictionaryChanging<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 可观测的键集合
    /// </summary>
    public IObservableCollection<TKey> ObservableKeys { get; }

    /// <summary>
    /// 可观测的值集合
    /// </summary>
    public IObservableCollection<TValue> ObservableValues { get; }
}
