using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 字典改变时事件处理器
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="sender">源</param>
/// <param name="e">事件参数</param>
public delegate void NotifyDictionaryChangingEventHandler<TKey, TValue>(
    object? sender,
    NotifyDictionaryChangingEventArgs<TKey, TValue> e
);
