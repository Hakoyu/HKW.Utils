using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典已改变事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay(
    "DictionaryChanging, Action = {Action}, NewPairsCount = {NewItems.Count}, OldPairsCount = {OldItems.Count}"
)]
public class NotifyDictionaryChangingEventArgs<TKey, TValue> : CancelEventArgs
    where TKey : notnull
{
    /// <summary>
    /// 改变行动
    /// </summary>
    public DictionaryChangeAction Action { get; }

    /// <summary>
    /// 新键值对
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>>? NewPairs { get; }

    /// <summary>
    /// 旧键值对
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>>? OldPairs { get; }

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
    /// <summary>仅用于
    /// <see cref="DictionaryChangeAction.Add"/>
    /// <see cref="DictionaryChangeAction.Remove"/>
    /// <see cref="DictionaryChangeAction.Clear"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="pairs">键值对</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> pairs
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
        IList<KeyValuePair<TKey, TValue>> list;
        if (pairs.IsReadOnly)
            list = pairs;
        else
            list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(pairs);
        if (Action is DictionaryChangeAction.Add)
            NewPairs = list;
        else
            OldPairs = list;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeAction.ValueChange"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (action != DictionaryChangeAction.ValueChange)
            throw new ArgumentException(
                string.Format(MessageFormat.MustBe, nameof(DictionaryChangeAction.ValueChange)),
                nameof(action)
            );
        Action = action;
        if (newPairs.IsReadOnly)
            NewPairs = newPairs;
        else
            NewPairs = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(newPairs);
        if (oldPairs.IsReadOnly)
            OldPairs = oldPairs;
        else
            OldPairs = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(oldPairs);
    }

    #endregion
}
