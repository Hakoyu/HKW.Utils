using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 异步执行事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate Task AsyncExecuteEventHandler(ICommand sender, EventArgs e);

/// <summary>
/// 异步执行事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate Task AsyncExecuteEventHandler<T>(ICommand sender, CommandParameterArgs<T> e);
