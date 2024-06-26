﻿using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知列表已改变事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("ListChange, Action = {Action}")]
public class NotifyListChangeEventArgs<T> : EventArgs
{
    /// <summary>
    /// 改变行动
    /// </summary>
    public ListChangeAction Action { get; }

    /// <summary>
    /// 新项目
    /// </summary>
    public IList<T>? NewItems { get; }

    /// <summary>
    /// 旧项目
    /// </summary>
    public IList<T>? OldItems { get; }

    /// <summary>
    /// 索引
    /// </summary>
    [DefaultValue(-1)]
    public int Index { get; } = -1;

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="ListChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifyListChangeEventArgs(ListChangeAction action)
    {
        if (action != ListChangeAction.Clear)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(ListChangeAction.Clear)}",
                nameof(action)
            );
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="ListChangeAction.Add"/>
    /// <see cref="ListChangeAction.Remove"/>
    /// <see cref="ListChangeAction.Clear"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="items">项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangeEventArgs(ListChangeAction action, IList<T> items, int index)
    {
        if (
            action != ListChangeAction.Add
            && action != ListChangeAction.Remove
            && action != ListChangeAction.Clear
        )
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(ListChangeAction.Add)} or {nameof(ListChangeAction.Remove)} or {nameof(ListChangeAction.Clear)}",
                nameof(action)
            );
        Action = action;
        Index = index;
        IList<T> list;
        if (items.IsReadOnly)
            list = items;
        else
            list = new SimpleReadOnlyList<T>(items);
        if (action is ListChangeAction.Add)
            NewItems = list;
        else
            OldItems = list;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 仅用于: <see cref="ListChangeAction.Replace"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangeEventArgs(
        ListChangeAction action,
        IList<T> newItems,
        IList<T> oldItems,
        int index
    )
    {
        if (action != ListChangeAction.Replace)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(ListChangeAction.Replace)}",
                nameof(action)
            );
        Action = action;
        Index = index;
        if (newItems.IsReadOnly)
            NewItems = newItems;
        else
            NewItems = new SimpleReadOnlyList<T>(newItems);
        if (oldItems.IsReadOnly)
            OldItems = oldItems;
        else
            OldItems = new SimpleReadOnlyList<T>(oldItems);
    }
    #endregion
}
