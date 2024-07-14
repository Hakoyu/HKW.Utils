using System.ComponentModel;
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
    public T? NewItem { get; }

    /// <summary>
    /// 旧项目
    /// </summary>
    public T? OldItem { get; }

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
        if (action is not ListChangeAction.Clear)
            throw new ArgumentException(nameof(action));
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="ListChangeAction.Add"/>
    /// <see cref="ListChangeAction.Remove"/>
    /// <see cref="ListChangeAction.Clear"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangeEventArgs(ListChangeAction action, T item, int index)
    {
        if (
            action is not ListChangeAction.Add
            && action is not ListChangeAction.Remove
            && action is not ListChangeAction.Clear
        )
            throw new ArgumentException(nameof(action));
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        Action = action;
        Index = index;
        if (action is ListChangeAction.Add)
            NewItem = item;
        else
            OldItem = item;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 仅用于: <see cref="ListChangeAction.Replace"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangeEventArgs(ListChangeAction action, T newItem, T oldItem, int index)
    {
        if (action != ListChangeAction.Replace)
            throw new ArgumentException(nameof(action));
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        Action = action;
        Index = index;
        NewItem = newItem;
        OldItem = oldItem;
    }
    #endregion
}
