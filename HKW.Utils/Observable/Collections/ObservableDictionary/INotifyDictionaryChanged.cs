﻿namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知字典改变后接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface INotifyDictionaryChanged<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 字典改变后事件
    /// </summary>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;
}
