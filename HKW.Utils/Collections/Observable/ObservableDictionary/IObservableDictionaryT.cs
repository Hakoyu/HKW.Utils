using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface IObservableDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>,
        INotifyDictionaryChanged<TKey, TValue>,
        INotifyDictionaryChanging<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    where TKey : notnull
{
    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<TKey>? Comparer { get; }

    /// <summary>
    /// 可观察的键集合与值集合
    /// </summary>
    public bool ObservableKeysAndValues { get; }

    /// <summary>
    /// 可观察的键的集合
    /// </summary>
    public IReadOnlyObservableList<TKey> ObservableKeys { get; }

    /// <summary>
    /// 可观察的值的集合
    /// </summary>
    public IReadOnlyObservableList<TValue> ObservableValues { get; }
}
