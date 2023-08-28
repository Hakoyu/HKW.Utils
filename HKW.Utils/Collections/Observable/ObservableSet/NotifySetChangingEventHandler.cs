using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集合改变时事件处理器
/// </summary>
/// <typeparam name="T">类型</typeparam>
/// <param name="sender">源</param>
/// <param name="e">事件参数</param>
public delegate void NotifySetChangingEventHandler<T>(
    object? sender,
    NotifySetChangingEventArgs<T> e
);
