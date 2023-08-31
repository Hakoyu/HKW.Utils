using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表已改变事件参数
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("ChangeMode = {ChangeMode}, Index = {Index}")]
public class NotifyListChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// 改变模式
    /// </summary>
    public ListChangeMode ChangeMode { get; }

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
    /// <summary>仅用于: <see cref="ListChangeMode.Clear"/></summary>
    /// <param name="changeMode">改变方案</param>
    /// <exception cref="ArgumentException"><paramref name="changeMode"/> 不是 <see cref="ListChangeMode.Clear"/></exception>
    public NotifyListChangedEventArgs(ListChangeMode changeMode)
    {
        ChangeMode = changeMode;
    }

    /// <inheritdoc/>
    /// <summary>仅用于:
    /// <see cref="ListChangeMode.Add"/>
    /// <see cref="ListChangeMode.Remove"/>
    /// </summary>
    /// <param name="changeMode">改变方案</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    public NotifyListChangedEventArgs(ListChangeMode changeMode, T item, int index)
    {
        if (changeMode is ListChangeMode.Add)
            NewItem = item;
        else
            OldItem = item;
        ChangeMode = changeMode;
        Index = index;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 仅用于: <see cref="ListChangeMode.ValueChange"/>
    /// </summary>
    /// <param name="changeMode">改变方案</param>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    /// <exception cref="ArgumentException"><paramref name="changeMode"/> 不是 <see cref="ListChangeMode.ValueChange"/></exception>
    public NotifyListChangedEventArgs(ListChangeMode changeMode, T newItem, T oldItem, int index)
    {
        ChangeMode = changeMode;
        NewItem = newItem;
        OldItem = oldItem;
        Index = index;
    }
}
