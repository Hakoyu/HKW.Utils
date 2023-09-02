using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表已改变事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("ListChanged, Action = {Action}, Index = {Index}")]
public class NotifyListChangedEventArgs<T> : EventArgs
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

    /// <inheritdoc/>
    /// <summary>仅用于: <see cref="ListChangeAction.Clear"/></summary>
    /// <param name="action">改变行动</param>
    public NotifyListChangedEventArgs(ListChangeAction action)
    {
        Action = action;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="ListChangeAction.Add"/>
    /// <see cref="ListChangeAction.Remove"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangedEventArgs(ListChangeAction action, T item, int index)
    {
        if (action is ListChangeAction.Add)
            NewItem = item;
        else
            OldItem = item;
        Action = action;
        Index = index;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 仅用于: <see cref="ListChangeAction.ValueChange"/>
    /// </summary>
    /// <param name="action">改变行动</param>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangedEventArgs(ListChangeAction action, T newItem, T oldItem, int index)
    {
        Action = action;
        NewItem = newItem;
        OldItem = oldItem;
        Index = index;
    }
}
