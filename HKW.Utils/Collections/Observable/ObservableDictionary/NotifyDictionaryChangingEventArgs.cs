using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典改变前事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("DictionaryChanging, Action = {Action}")]
public class NotifyDictionaryChangingEventArgs<TKey, TValue> : CancelEventArgs
    where TKey : notnull
{
    /// <summary>
    /// 改变行动
    /// </summary>
    public DictionaryChangeAction Action { get; }

    /// <summary>
    /// 新条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? NewEntry { get; }

    /// <summary>
    /// 旧条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? OldEntry { get; }

    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="DictionaryChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    /// <exception cref="ArgumentException"><paramref name="action"/> 不是 <see cref="DictionaryChangeAction.Clear"/></exception>
    public NotifyDictionaryChangingEventArgs(DictionaryChangeAction action)
    {
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="DictionaryChangeAction.Add"/>
    /// <see cref="DictionaryChangeAction.Remove"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="entry">改变的条目</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        KeyValuePair<TKey, TValue> entry
    )
    {
        Action = action;
        if (Action is DictionaryChangeAction.Add)
            NewEntry = entry;
        else
            OldEntry = entry;
    }

    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="DictionaryChangeAction.ValueChange"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newEntry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    /// <exception cref="ArgumentException"><paramref name="action"/> 不是 <see cref="DictionaryChangeAction.ValueChange"/></exception>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        KeyValuePair<TKey, TValue> newEntry,
        KeyValuePair<TKey, TValue> oldEntry
    )
    {
        Action = action;
        NewEntry = newEntry;
        OldEntry = oldEntry;
    }
}
