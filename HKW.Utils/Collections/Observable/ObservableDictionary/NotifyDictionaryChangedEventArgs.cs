using HKW.HKWUtils.Natives;
using System.Collections;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典已改变事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay(
    "DictionaryChanged, Action = {Action}, NewItemsCount = {NewItems.Count}, OldItemsCount = {OldItems.Count}"
)]
public class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
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
    public NotifyDictionaryChangedEventArgs(DictionaryChangeAction action)
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
    /// <param name="items">键值对</param>
    public NotifyDictionaryChangedEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> items
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
        if (items.IsReadOnly)
            list = items;
        else
            list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(items);
        if (Action is DictionaryChangeAction.Add)
            NewItems = list;
        else
            OldItems = list;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeAction.ValueChange"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newItems">新键值对</param>
    /// <param name="oldItems">旧键值对</param>
    public NotifyDictionaryChangedEventArgs(
        DictionaryChangeAction action,
        IList<KeyValuePair<TKey, TValue>> newItems,
        IList<KeyValuePair<TKey, TValue>> oldItems
    )
    {
        if (action != DictionaryChangeAction.ValueChange)
            throw new ArgumentException(
                string.Format(MessageFormat.MustBe, nameof(DictionaryChangeAction.ValueChange)),
                nameof(action)
            );
        Action = action;
        if (newItems.IsReadOnly)
            NewItems = newItems;
        else
            NewItems = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(newItems);
        if (oldItems.IsReadOnly)
            OldItems = oldItems;
        else
            OldItems = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(oldItems);
    }

    #endregion
}
