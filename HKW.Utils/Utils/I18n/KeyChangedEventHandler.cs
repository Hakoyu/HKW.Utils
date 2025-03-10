﻿namespace HKW.HKWUtils;

/// <summary>
/// 键改变后事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void KeyChangedEventHandler<TKey, TValue>(
    I18nObject<TKey, TValue> sender,
    (TKey OldKey, TKey NewKey) e
)
    where TKey : notnull;
