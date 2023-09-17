using System.Collections;
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
        IObservableCollection<KeyValuePair<TKey, TValue>>,
        INotifyDictionaryChanged<TKey, TValue>,
        INotifyDictionaryChanging<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    where TKey : notnull
{
    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    /// <summary>
    /// 启用可观察的键集合与值集合
    /// </summary>
    public bool ObservableKeysAndValues { get; set; }

    /// <summary>
    /// 可观察的键的集合
    /// <para>使用此值需启用 <see cref="ObservableKeysAndValues"/></para>
    /// </summary>
    public IObservableList<TKey> ObservableKeys { get; }

    /// <summary>
    /// 可观察的值的集合
    /// <para>使用此值需启用 <see cref="ObservableKeysAndValues"/></para>
    /// </summary>
    public IObservableList<TValue> ObservableValues { get; }

    /// <summary>
    /// 尝试添加多个键值对
    /// </summary>
    /// <param name="collection">键值对</param>
    /// <returns>成功添加的键值对列表</returns>
    public IList<KeyValuePair<TKey, TValue>> TryAddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> collection
    );
}
