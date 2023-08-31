using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Events;

/// <summary>
/// 返回结果的事件处理器
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
/// <returns>结果</returns>
public delegate TResult XResultEventHandler<TResult>();

/// <summary>
/// 返回结果的事件处理器
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="arg">参数</param>
/// <returns>结果</returns>
public delegate TResult XResultEventHandler<in T, TResult>(T arg);

/// <summary>
/// 返回结果的事件处理器
/// </summary>
/// <typeparam name="T1">参数1类型</typeparam>
/// <typeparam name="T2">参数2类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="arg1">参数1</param>
/// <param name="arg2">参数2</param>
/// <returns>结果</returns>
public delegate TResult XResultEventHandler<in T1, in T2, TResult>(T1 arg1, T2 arg2);
