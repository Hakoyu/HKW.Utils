﻿using HKW.HKWUtils.Natives;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合已改变事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("SetChanged, Action = {Action}")]
public class NotifySetChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// 改变行动
    /// </summary>
    public SetChangeAction Action { get; }

    /// <summary>
    /// 新项目
    /// </summary>
    public IList<T>? NewItems { get; }

    /// <summary>
    /// 旧项目
    /// </summary>
    public IList<T>? OldItems { get; }

    /// <summary>
    /// 集合操作项
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </para>
    /// </summary>
    public IEnumerable<T>? OtherItems { get; }

    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="SetChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifySetChangedEventArgs(SetChangeAction action)
    {
        if (action != SetChangeAction.Clear)
            throw new ArgumentException(
                string.Format(MessageFormat.MustBe, nameof(SetChangeAction.Clear)),
                nameof(action)
            );
        Action = action;
    }

    /// <inheritdoc/>
    /// <param name="action">改变行动</param>
    /// <see cref="SetChangeAction.Add"/>
    /// <see cref="SetChangeAction.Remove"/>
    /// <param name="item">项目</param>
    public NotifySetChangedEventArgs(SetChangeAction action, T item)
    {
        if (action != SetChangeAction.Add && action != SetChangeAction.Remove)
            throw new ArgumentException(
                string.Format(
                    MessageFormat.MustBe,
                    $"{nameof(SetChangeAction.Add)} or {nameof(SetChangeAction.Remove)}"
                ),
                nameof(action)
            );
        Action = action;
        if (action is SetChangeAction.Add)
            NewItems = new List<T>() { item };
        else
            OldItems = new List<T>() { item };
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeAction.Add"/>
    /// <see cref="SetChangeAction.Remove"/>
    /// <see cref="SetChangeAction.Clear"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="items">旧项目</param>
    public NotifySetChangedEventArgs(SetChangeAction action, IList<T>? items)
    {
        if (
            action != SetChangeAction.Add
            && action != SetChangeAction.Remove
            && action != SetChangeAction.Clear
        )
            throw new ArgumentException(
                string.Format(
                    MessageFormat.MustBe,
                    $"{nameof(SetChangeAction.Add)} or {nameof(SetChangeAction.Remove)} or {nameof(SetChangeAction.Clear)}"
                ),
                nameof(action)
            );
        Action = action;
        if (action is SetChangeAction.Add)
            NewItems = items;
        else
            OldItems = items;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="otherItems">其它集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    public NotifySetChangedEventArgs(
        SetChangeAction action,
        IEnumerable<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems
    )
    {
        Action = action;
        OtherItems = otherItems;
        NewItems = newItems;
        OldItems = oldItems;
    }
}
