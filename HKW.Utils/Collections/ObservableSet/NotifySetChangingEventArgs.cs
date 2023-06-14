﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变时事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Action = {ChangeAction}")]
public class NotifySetChangingEventArgs<T> : CancelEventArgs
{
    /// <summary>
    /// 改变方案
    /// </summary>
    public NotifySetChangeAction Action { get; }

    /// <summary>
    /// 项
    /// </summary>
    public T? Item { get; } = default;

    /// <summary>
    /// 集合操作项
    /// <para>
    /// 仅用于
    /// <see cref="ObservableSet{T}.IntersectWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.ExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.SymmetricExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.UnionWith(IEnumerable{T})"/>
    /// </para>
    /// </summary>
    public IEnumerable<T>? Items { get; } = default;

    /// <summary>
    /// 取消
    /// </summary>
    public new bool Cancel { get; set; } = false;

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="ObservableSet{T}.Clear"/></summary>
    /// <param name="changeAction">改变方案</param>
    /// <exception cref="ArgumentException"><paramref name="changeAction"/> 不是 <see cref="NotifySetChangeAction.Clear"/></exception>
    public NotifySetChangingEventArgs(NotifySetChangeAction changeAction)
    {
        if (changeAction is not NotifySetChangeAction.Clear)
            throw new ArgumentException(
                string.Format(
                    ExceptionsMessage.Format_CanOnlyBeUsedFor,
                    NotifySetChangeAction.Clear
                ),
                nameof(changeAction)
            );
        Action = changeAction;
    }

    /// <inheritdoc/>
    /// <param name="changeAction">改变方案</param>
    /// <param name="item">改变的条目</param>
    public NotifySetChangingEventArgs(NotifySetChangeAction changeAction, T item)
    {
        Action = changeAction;
        Item = item;
    }

    /// <inheritdoc/>
    /// <summary>仅用于
    /// <see cref="ObservableSet{T}.IntersectWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.ExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.SymmetricExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.UnionWith(IEnumerable{T})"/>
    /// </summary>
    /// <param name="changeAction">改变方案</param>
    /// <param name="collection">集合</param>
    public NotifySetChangingEventArgs(NotifySetChangeAction changeAction, IEnumerable<T> collection)
    {
        Action = changeAction;
        Items = collection;
    }
}
