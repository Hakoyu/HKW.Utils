using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读可观察字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface IReadOnlyObservableDictionary<TKey, TValue>
    : IReadOnlyDictionary<TKey, TValue>,
        INotifyDictionaryChanged<TKey, TValue>,
        INotifyDictionaryChanging<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    where TKey : notnull
{
    /// <summary>
    /// 可观察的键的集合
    /// </summary>
    public IObservableList<TKey> ObservableKeys { get; }

    /// <summary>
    /// 可观察的值的集合
    /// </summary>
    public IObservableList<TValue> ObservableValues { get; }
}
