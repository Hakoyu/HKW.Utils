using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表改变时事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Action = {ChangeAction}, Index = {Index}")]
public class NotifyListChangingEventArgs<T> : CancelEventArgs
    where T : notnull
{
    /// <summary>
    /// 改变方案
    /// </summary>
    public NotifyListChangeAction Action { get; }

    /// <summary>
    /// 项
    /// </summary>
    public T? Item { get; } = default;

    /// <summary>
    /// 新项
    /// <para>仅使用于 <see cref="ObservableList{T}.this[int]"/></para>
    /// </summary>
    public T? NewItem { get; } = default;

    /// <summary>
    /// 索引
    /// </summary>
    public int Index { get; } = -1;

    /// <summary>
    /// 取消
    /// </summary>
    public new bool Cancel { get; set; } = false;

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="ObservableList{T}.Clear"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyListChangeAction.Clear"/></exception>
    public NotifyListChangingEventArgs(NotifyListChangeAction changeAction)
    {
        if (changeAction is not NotifyListChangeAction.Clear)
            throw new ArgumentException(
                string.Format(
                    ExceptionsMessage.Format_CanOnlyBeUsedFor,
                    NotifyListChangeAction.Clear
                ),
                nameof(changeAction)
            );
        Action = changeAction;
    }

    /// <inheritdoc/>
    /// <param name="changeAction">改变方案</param>
    /// <param name="item">改变的条目</param>
    /// <param name="index">索引</param>
    public NotifyListChangingEventArgs(NotifyListChangeAction changeAction, T item, int index)
    {
        Action = changeAction;
        Item = item;
        Index = index;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 仅使用于 <see cref="ObservableList{T}.this[int]"/>
    /// </summary>
    /// <param name="changeAction">改变方案</param>
    /// <param name="oldItem">改变的条目</param>
    /// <param name="newItem">新值</param>
    /// <param name="index">索引</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifyListChangeAction.ValueChange"/></exception>
    public NotifyListChangingEventArgs(
        NotifyListChangeAction changeAction,
        T oldItem,
        T newItem,
        int index
    )
    {
        if (changeAction is not NotifyListChangeAction.ValueChange)
            throw new ArgumentException(
                string.Format(
                    ExceptionsMessage.Format_CanOnlyBeUsedFor,
                    NotifyListChangeAction.ValueChange
                ),
                nameof(changeAction)
            );
        Action = changeAction;
        Item = oldItem;
        NewItem = newItem;
        Index = index;
    }
}
