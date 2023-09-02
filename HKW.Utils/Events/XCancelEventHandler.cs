using System.ComponentModel;

namespace HKW.HKWUtils.Events;

/// <summary>
/// 可取消事件处理器
/// </summary>
/// <typeparam name="TCancelArgs">参数类型</typeparam>
/// <param name="cancelArgs">可取消参数</param>
public delegate void XCancelEventHandler<in TCancelArgs>(TCancelArgs cancelArgs)
    where TCancelArgs : CancelEventArgs;

/// <summary>
/// 可取消事件处理器
/// </summary>
/// <typeparam name="T">发送者类型</typeparam>
/// <typeparam name="TCancelArgs">参数类型</typeparam>
/// <param name="args">参数</param>
/// <param name="cancelArgs">可取消参数</param>
public delegate void XCancelEventHandler<in T, in TCancelArgs>(T args, TCancelArgs cancelArgs)
    where TCancelArgs : CancelEventArgs;
