using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典改变时事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Action = {ChangeAction}")]
public class NotifyDictionaryChangingEventArgs<TKey, TValue> : CancelEventArgs
{
    /// <summary>
    /// 改变方案
    /// </summary>
    public NotifyDictionaryChangeAction Action { get; }

    /// <summary>
    /// 改变的条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? Entry { get; }

    /// <summary>
    /// 新条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? NewEntry { get; }

    /// <summary>
    /// 取消
    /// </summary>
    public new bool Cancel { get; set; } = false;

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="ObservableDictionary{TKey, TValue}.Clear"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyDictionaryChangeAction.Clear"/></exception>
    public NotifyDictionaryChangingEventArgs(NotifyDictionaryChangeAction changeAction)
    {
        if (changeAction is not NotifyDictionaryChangeAction.Clear)
            throw new ArgumentException(
                string.Format(
                    ExceptionsMessage.Format_CanOnlyBeUsedFor,
                    NotifyDictionaryChangeAction.Clear
                ),
                nameof(changeAction)
            );
        Action = changeAction;
    }

    /// <inheritdoc/>
    /// <param name="changeAction">改变方案</param>
    /// <param name="keyValuePair">改变的条目</param>
    public NotifyDictionaryChangingEventArgs(
        NotifyDictionaryChangeAction changeAction,
        KeyValuePair<TKey, TValue> keyValuePair
    )
    {
        Action = changeAction;
        Entry = keyValuePair;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="ObservableDictionary{TKey, TValue}.this[TKey]"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <param name="entry">旧条目</param>
    /// <param name="newEntry">新条目</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyDictionaryChangeAction.ValueChange"/></exception>
    public NotifyDictionaryChangingEventArgs(
        NotifyDictionaryChangeAction changeAction,
        KeyValuePair<TKey, TValue> entry,
        KeyValuePair<TKey, TValue> newEntry
    )
    {
        if (changeAction is not NotifyDictionaryChangeAction.ValueChange)
            throw new ArgumentException(
                string.Format(
                    ExceptionsMessage.Format_CanOnlyBeUsedFor,
                    NotifyDictionaryChangeAction.ValueChange
                ),
                nameof(changeAction)
            );
        Action = changeAction;
        Entry = entry;
        NewEntry = newEntry;
    }
}
