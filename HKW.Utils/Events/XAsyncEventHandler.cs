using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Events;

/// <summary>
/// 异步事件处理器
/// </summary>
public delegate Task XAsyncEventHandler();

/// <summary>
/// 异步事件处理器
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
/// <param name="arg">参数</param>
public delegate Task XAsyncEventHandler<in T>(T arg);

/// <summary>
/// 异步事件处理器
/// </summary>
/// <typeparam name="T1">参数1类型</typeparam>
/// <typeparam name="T2">参数2类型</typeparam>
/// <param name="arg1">参数1</param>
/// <param name="arg2">参数2</param>
public delegate Task XAsyncEventHandler<in T1, in T2>(T1 arg1, T2 arg2);
