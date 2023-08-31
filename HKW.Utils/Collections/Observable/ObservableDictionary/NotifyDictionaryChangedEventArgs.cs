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
[DebuggerDisplay("DictionaryChanged, ChangeMode = {ChangeMode}")]
public class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
    where TKey : notnull
{
    /// <summary>
    /// 改变模式
    /// </summary>
    public DictionaryChangeMode ChangeMode { get; }

    /// <summary>
    /// 新条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? NewEntry { get; }

    /// <summary>
    /// 旧条目
    /// </summary>
    public KeyValuePair<TKey, TValue>? OldEntry { get; }

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="DictionaryChangeMode.Clear"/></summary>
    /// <param name="changeMode">改变方案</param>
    public NotifyDictionaryChangedEventArgs(DictionaryChangeMode changeMode)
    {
        ChangeMode = changeMode;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="DictionaryChangeMode.Add"/>
    /// <see cref="DictionaryChangeMode.Remove"/>
    /// </summary>
    /// <param name="changeMode">改变方案</param>
    /// <param name="entry">改变的条目</param>
    public NotifyDictionaryChangedEventArgs(
        DictionaryChangeMode changeMode,
        KeyValuePair<TKey, TValue> entry
    )
    {
        ChangeMode = changeMode;
        if (ChangeMode is DictionaryChangeMode.Add)
            NewEntry = entry;
        else
            OldEntry = entry;
    }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="DictionaryChangeMode.ValueChange"/></summary>
    /// <param name="changeMode">改变方案</param>
    /// <param name="newEntry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    public NotifyDictionaryChangedEventArgs(
        DictionaryChangeMode changeMode,
        KeyValuePair<TKey, TValue> newEntry,
        KeyValuePair<TKey, TValue> oldEntry
    )
    {
        ChangeMode = changeMode;
        NewEntry = newEntry;
        OldEntry = oldEntry;
    }
    #endregion
}
