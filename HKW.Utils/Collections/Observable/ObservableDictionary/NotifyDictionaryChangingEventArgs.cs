using HKW.HKWUtils.Natives;
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
    public IList<KeyValuePair<TKey, TValue>>? NewItems { get; }

    /// <summary>
    /// 旧键值对
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>>? OldItems { get; }

    #region Ctor

    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="DictionaryChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifyDictionaryChangingEventArgs(DictionaryChangeAction action)
    {
        if (action != DictionaryChangeAction.Clear)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(DictionaryChangeAction.Clear)}",
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
                $"{ExceptionMessage.MustBe} {nameof(DictionaryChangeAction.Add)} or {nameof(DictionaryChangeAction.Remove)} or {nameof(DictionaryChangeAction.Clear)}",
                nameof(action)
            );
        Action = action;
        IList<KeyValuePair<TKey, TValue>> list;
        if (pairs.IsReadOnly)
            list = pairs;
        else
            list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(pairs);
        if (Action is DictionaryChangeAction.Add)
            NewItems = list;
        else
            OldItems = list;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeAction.Replace"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (action != DictionaryChangeAction.Replace)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(DictionaryChangeAction.Replace)}",
                nameof(action)
            );
        Action = action;
        if (newPairs.IsReadOnly)
            NewItems = newPairs;
        else
            NewItems = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(newPairs);
        if (oldPairs.IsReadOnly)
            OldItems = oldPairs;
        else
            OldItems = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(oldPairs);
    }

    #endregion
}
