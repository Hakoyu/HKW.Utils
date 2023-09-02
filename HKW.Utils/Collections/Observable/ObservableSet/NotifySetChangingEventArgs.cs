using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变前事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("SetChanging, Action = {Action}")]
public class NotifySetChangingEventArgs<T> : CancelEventArgs
{
    /// <summary>
    /// 改变行动
    /// </summary>
    public SetChangeAction Action { get; }

    /// <summary>
    /// 新项目
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Add"/>
    /// </para>
    /// </summary>
    public T? NewItem { get; }

    /// <summary>
    /// 旧项目
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Remove"/>
    /// </para>
    /// </summary>
    public T? OldItem { get; }

    /// <summary>
    /// 集合修改时的其它集合
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </para>
    /// </summary>
    public IEnumerable<T>? OtherItems { get; }

    /// <summary>
    /// 集合修改新增的项目 要启用 <see cref="IObservableSet{T}.NotifySetModifies"/>
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </para>
    /// </summary>
    public IList<T>? NewItems { get; }

    /// <summary>
    /// 集合修改删除的项目 要启用 <see cref="IObservableSet{T}.NotifySetModifies"/>
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </para>
    /// </summary>
    public IList<T>? OldItems { get; }

    /// <inheritdoc/>
    /// <summary>仅用于 <see cref="SetChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifySetChangingEventArgs(SetChangeAction action)
    {
        Action = action;
    }

    /// <inheritdoc/>
    /// <param name="action">改变行动</param>
    /// <param name="item">项目</param>
    public NotifySetChangingEventArgs(SetChangeAction action, T item)
    {
        if (action is SetChangeAction.Add)
            NewItem = item;
        else
            OldItem = item;
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeAction.Intersect"/>
    /// <see cref="SetChangeAction.Except"/>
    /// <see cref="SetChangeAction.SymmetricExcept"/>
    /// <see cref="SetChangeAction.Union"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="other">集合</param>
    /// <param name="newItems">新项目</param>
    /// <param name="oldItems">旧项目</param>
    public NotifySetChangingEventArgs(
        SetChangeAction action,
        IEnumerable<T> other,
        IList<T>? newItems,
        IList<T>? oldItems
    )
    {
        Action = action;
        OtherItems = other;
        NewItems = newItems;
        OldItems = oldItems;
    }
}
