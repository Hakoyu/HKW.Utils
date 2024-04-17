using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知集合已改变事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("SetChanged, Action = {Action}")]
public class NotifySetChangeEventArgs<T> : EventArgs
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
    public IList<T>? OtherItems { get; }

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="SetChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifySetChangeEventArgs(SetChangeAction action)
    {
        if (action != SetChangeAction.Clear)
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(SetChangeAction.Clear)}",
                nameof(action)
            );
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeAction.Add"/>
    /// <see cref="SetChangeAction.Remove"/>
    /// <see cref="SetChangeAction.Clear"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="items">旧项目</param>
    public NotifySetChangeEventArgs(SetChangeAction action, IList<T> items)
    {
        if (
            action != SetChangeAction.Add
            && action != SetChangeAction.Remove
            && action != SetChangeAction.Clear
        )
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(SetChangeAction.Add)} or {nameof(SetChangeAction.Remove)} or {nameof(SetChangeAction.Clear)}",
                nameof(action)
            );
        Action = action;
        IList<T> list;
        if (items.IsReadOnly)
            list = items;
        else
            list = new SimpleReadOnlyList<T>(items);
        if (action is SetChangeAction.Add)
            NewItems = list;
        else
            OldItems = list;
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
    public NotifySetChangeEventArgs(
        SetChangeAction action,
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldItems
    )
    {
        if (
            action != SetChangeAction.Intersect
            && action != SetChangeAction.Except
            && action != SetChangeAction.SymmetricExcept
            && action != SetChangeAction.Union
        )
            throw new ArgumentException(
                $"{ExceptionMessage.MustBe} {nameof(SetChangeAction.Intersect)} or {nameof(SetChangeAction.Except)} or {nameof(SetChangeAction.SymmetricExcept)} or {SetChangeAction.Union}",
                nameof(action)
            );
        Action = action;
        if (otherItems.IsReadOnly)
            OtherItems = otherItems;
        else
            OtherItems = new SimpleReadOnlyList<T>(otherItems);
        if (newItems is null)
            NewItems = null;
        else if (newItems.IsReadOnly)
            NewItems = newItems;
        else
            NewItems = new SimpleReadOnlyList<T>(newItems);
        if (oldItems is null)
            OldItems = null;
        else if (oldItems.IsReadOnly)
            OldItems = oldItems;
        else
            OldItems = new SimpleReadOnlyList<T>(oldItems);
    }
    #endregion
}
