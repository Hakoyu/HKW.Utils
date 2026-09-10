using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Exceptions;
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
    /// 清空事件
    /// </summary>
    public static NotifySetChangeEventArgs<T> Cache_Clear { get; } = new(SetChangeAction.Clear);

    /// <summary>
    /// 改变行动
    /// </summary>
    public SetChangeAction Action { get; }

    /// <summary>
    /// 新项目
    /// </summary>
    public ICollection<T>? NewItems { get; }

    /// <summary>
    /// 旧项目
    /// </summary>
    public ICollection<T>? OldItems { get; }

    /// <summary>
    /// 集合操作项
    /// <para>
    /// 仅用于:
    /// <see cref="SetChangeAction.Intersect"/>,
    /// <see cref="SetChangeAction.Except"/>,
    /// <see cref="SetChangeAction.SymmetricExcept"/>,
    /// <see cref="SetChangeAction.Union"/>
    /// </para>
    /// </summary>
    public ICollection<T>? OtherItems { get; }

    #region Ctor
    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="SetChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifySetChangeEventArgs(SetChangeAction action)
    {
        ArgumentException.ThrowIfNotEquals(action, SetChangeAction.Clear);
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="SetChangeAction.Add"/>
    /// <see cref="SetChangeAction.Remove"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="items">旧项目</param>
    public NotifySetChangeEventArgs(SetChangeAction action, IList<T> items)
    {
        ArgumentException.ThrowIfAllNotEquals(action, SetChangeAction.Add, SetChangeAction.Remove);
        Action = action;
        var list = items.IsReadOnly ? items : new ReadOnlyList<T>(items);
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
        ArgumentException.ThrowIfAllNotEquals(
            action,
            SetChangeAction.Intersect,
            SetChangeAction.Except,
            SetChangeAction.SymmetricExcept,
            SetChangeAction.Union
        );
        Action = action;
        OtherItems = otherItems.IsReadOnly ? otherItems : new ReadOnlyList<T>(otherItems);
        NewItems = newItems?.IsReadOnly is not false ? newItems : new ReadOnlyList<T>(newItems);
        OldItems = oldItems?.IsReadOnly is not false ? oldItems : new ReadOnlyList<T>(oldItems);
    }
    #endregion
}
