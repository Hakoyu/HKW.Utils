using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 列表已改变事件处理器
/// </summary>
/// <typeparam name="T">类型</typeparam>
/// <param name="sender">源</param>
/// <param name="e">事件参数</param>
public delegate void NotifyListChangedEventHandler<T>(
    object? sender,
    NotifyListChangedEventArgs<T> e
);
