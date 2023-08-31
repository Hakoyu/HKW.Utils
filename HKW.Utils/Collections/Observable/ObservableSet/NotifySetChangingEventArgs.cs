using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变前事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("ChangeMode = {ChangeMode}")]
public class NotifySetChangingEventArgs<T> : CancelEventArgs
{
    /// <summary>
    /// 改变方案
    /// </summary>
    public SetChangeMode ChangeMode { get; }

    /// <summary>
    /// 项 目
    /// </summary>
    public T? Item { get; } = default;

    /// <summary>
    /// 集合操作项
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeMode.Intersect"/>,
    /// <see cref="SetChangeMode.Except"/>,
    /// <see cref="SetChangeMode.SymmetricExcept"/>,
    /// <see cref="SetChangeMode.Union"/>
    /// </para>
    /// </summary>
    public IEnumerable<T>? Items { get; } = default;

    /// <summary>
    /// 取消
    /// </summary>
    public new bool Cancel { get; set; } = false;

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="SetChangeMode.Clear"/></summary>
    /// <param name="changeMode">改变方案</param>
    public NotifySetChangingEventArgs(SetChangeMode changeMode)
    {
        ChangeMode = changeMode;
    }

    /// <inheritdoc/>
    /// <param name="changeMode">改变方案</param>
    /// <param name="item">项目</param>
    public NotifySetChangingEventArgs(SetChangeMode changeMode, T item)
    {
        ChangeMode = changeMode;
        Item = item;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeMode.Intersect"/>,
    /// <see cref="SetChangeMode.Except"/>,
    /// <see cref="SetChangeMode.SymmetricExcept"/>,
    /// <see cref="SetChangeMode.Union"/>
    /// </summary>
    /// <param name="changeMode">改变方案</param>
    /// <param name="collection">集合</param>
    public NotifySetChangingEventArgs(SetChangeMode changeMode, IEnumerable<T> collection)
    {
        ChangeMode = changeMode;
        Items = collection;
    }
}
