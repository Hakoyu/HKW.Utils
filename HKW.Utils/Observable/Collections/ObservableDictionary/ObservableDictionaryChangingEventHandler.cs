﻿namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察字典改变前事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void ObservableDictionaryChangingEventHandler<TKey, TValue>(
    IObservableDictionary<TKey, TValue> sender,
    NotifyDictionaryChangeEventArgs<TKey, TValue> e
)
    where TKey : notnull;
