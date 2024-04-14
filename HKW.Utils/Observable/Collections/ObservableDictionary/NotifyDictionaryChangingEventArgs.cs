using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知字典已改变事件参数
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("DictionaryChanging, Action = {Action}, NewPair = {NewPair}, OldPair = {NewPair}")]
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
    public KeyValuePair<TKey, TValue>? NewPair { get; }

    /// <summary>
    /// 旧键值对
    /// </summary>
    public KeyValuePair<TKey, TValue>? OldPair { get; }

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
    /// <param name="pair">键值对</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        KeyValuePair<TKey, TValue> pair
    )
    {
        if (action != DictionaryChangeAction.Add && action != DictionaryChangeAction.Remove)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(DictionaryChangeAction.Add)} or {nameof(DictionaryChangeAction.Remove)}",
                nameof(action)
            );
        Action = action;
        if (Action is DictionaryChangeAction.Add)
            NewPair = pair;
        else
            OldPair = pair;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeAction.Replace"/></summary>
    /// <param name="action">改变行动</param>
    /// <param name="newPair">新键值对</param>
    /// <param name="oldPair">旧键值对</param>
    public NotifyDictionaryChangingEventArgs(
        DictionaryChangeAction action,
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (action != DictionaryChangeAction.Replace)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(DictionaryChangeAction.Replace)}",
                nameof(action)
            );
        Action = action;
        NewPair = newPair;
        OldPair = oldPair;
    }

    #endregion

    /// <summary>
    /// 尝试获取新键值对
    /// </summary>
    /// <param name="newPair"></param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetNewPair([MaybeNullWhen(false)] out KeyValuePair<TKey, TValue> newPair)
    {
        if (NewPair.HasValue)
        {
            newPair = NewPair!.Value;
            return true;
        }
        newPair = default;
        return false;
    }

    /// <summary>
    /// 尝试获取旧键值对
    /// </summary>
    /// <param name="oldPair"></param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetOldPair([MaybeNullWhen(false)] out KeyValuePair<TKey, TValue> oldPair)
    {
        if (OldPair.HasValue)
        {
            oldPair = OldPair!.Value;
            return true;
        }
        oldPair = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"DictionaryChanging, Action = {Action}, NewPair = {NewPair}, OldPair = {NewPair}";
    }
}
