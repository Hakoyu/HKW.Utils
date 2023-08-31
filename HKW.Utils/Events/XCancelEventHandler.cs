using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Events;

/// <summary>
/// 可取消事件处理器
/// </summary>
/// <typeparam name="TCancelArg">参数类型</typeparam>
/// <param name="cancelArg">可取消参数</param>
public delegate void XCancelEventHandler<in TCancelArg>(TCancelArg cancelArg)
    where TCancelArg : CancelEventArgs;

/// <summary>
/// 可取消事件处理器
/// </summary>
/// <typeparam name="T">发送者类型</typeparam>
/// <typeparam name="TCancelArgs">参数类型</typeparam>
/// <param name="arg">参数</param>
/// <param name="cancelArg">可取消参数</param>
public delegate void XCancelEventHandler<in T, in TCancelArgs>(T arg, TCancelArgs cancelArg)
    where TCancelArgs : CancelEventArgs;
