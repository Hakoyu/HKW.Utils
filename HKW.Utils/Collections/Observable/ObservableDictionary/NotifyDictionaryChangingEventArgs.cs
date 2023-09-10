using HKW.HKWUtils.Natives;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典改变前事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay(
    "DictionaryChanging, Action = {Action}, NewEntriesCount = {NewEntries.Count}, OldEntriesCount = {OldEntries.Count}"
)]
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
    public IList<KeyValuePair<TKey, TValue>>? NewEntries { get; }

    /// <summary>
    /// 旧条目
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>>? OldEntries { get; }

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="DictionaryChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifyDictionaryChangingEventArgs(DictionaryChangeAction action)
    {
        if (action != DictionaryChangeAction.Clear)
            throw new ArgumentException(
                string.Format(MessageFormat.MustBe, nameof(DictionaryChangeAction.Clear)),
                nameof(action)
            );
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
        if (action != DictionaryChangeAction.Add && action != DictionaryChangeAction.Remove)
            throw new ArgumentException(
                string.Format(
                    MessageFormat.MustBe,
                    $"{nameof(DictionaryChangeAction.Add)} or {nameof(DictionaryChangeAction.Remove)}"
                ),
                nameof(action)
            );
        Action = action;
        if (Action is DictionaryChangeAction.Add)
            NewEntries = new List<KeyValuePair<TKey, TValue>>() { entry };
        else
            OldEntries = new List<KeyValuePair<TKey, TValue>>() { entry };
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeAction.ValueChange"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newEntry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        KeyValuePair<TKey, TValue> newEntry,
        KeyValuePair<TKey, TValue> oldEntry
    )
    {
        Action = action;
        NewEntries = new List<KeyValuePair<TKey, TValue>>() { newEntry };
        OldEntries = new List<KeyValuePair<TKey, TValue>>() { oldEntry };
    }

    /// <inheritdoc/>
    /// <see cref="DictionaryChangeAction.Add"/>
    /// <see cref="DictionaryChangeAction.Remove"/>
    /// <see cref="DictionaryChangeAction.Clear"/>
    /// <param name="action">改变行动</param>
    /// <param name="entries">条目</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> entries
    )
    {
        if (
            action != DictionaryChangeAction.Add
            && action != DictionaryChangeAction.Remove
            && action != DictionaryChangeAction.Clear
        )
            throw new ArgumentException(
                string.Format(
                    MessageFormat.MustBe,
                    $"{nameof(DictionaryChangeAction.Add)} or {nameof(DictionaryChangeAction.Remove)} or {nameof(DictionaryChangeAction.Clear)}"
                ),
                nameof(action)
            );
        Action = action;
        if (Action is DictionaryChangeAction.Add)
            NewEntries = entries;
        else
            OldEntries = entries;
    }
    #endregion
}
