using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典已改变事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Action = {ChangeAction}")]
public class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
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
    /// 旧条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? OldEntry { get; }

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="NotifyDictionaryChangeAction.Clear"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyDictionaryChangeAction.Clear"/></exception>
    public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangeAction changeAction)
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
    /// <param name="entry">改变的条目</param>
    public NotifyDictionaryChangedEventArgs(
        NotifyDictionaryChangeAction changeAction,
        KeyValuePair<TKey, TValue> entry
    )
    {
        Action = changeAction;
        Entry = entry;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="ObservableDictionary{TKey, TValue}.this[TKey]"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <param name="entry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyDictionaryChangeAction.ValueChange"/></exception>
    public NotifyDictionaryChangedEventArgs(
        NotifyDictionaryChangeAction changeAction,
        KeyValuePair<TKey, TValue> entry,
        KeyValuePair<TKey, TValue> oldEntry
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
        OldEntry = oldEntry;
    }
    #endregion
}
