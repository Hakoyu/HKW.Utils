﻿using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观察字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface IReadOnlyObservableDictionary<TKey, TValue>
    : IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyObservableCollection<KeyValuePair<TKey, TValue>>,
        INotifyDictionaryChanged<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    where TKey : notnull
{
    /// <summary>
    /// 可观察的键集合
    /// </summary>
    public IReadOnlyObservableCollection<TKey> ObservableKeys { get; }

    /// <summary>
    /// 可观察的值集合
    /// </summary>
    public IReadOnlyObservableCollection<TValue> ObservableValues { get; }
}
