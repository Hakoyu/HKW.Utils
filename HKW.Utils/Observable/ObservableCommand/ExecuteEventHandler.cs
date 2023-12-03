using System;
using System.Windows.Input;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 执行事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void ExecuteEventHandler(ICommand sender, EventArgs e);

/// <summary>
/// 执行事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void ExecuteEventHandler<T>(ICommand sender, CommandParameterEventArgs<T> e);
