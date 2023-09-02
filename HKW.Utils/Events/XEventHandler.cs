namespace HKW.HKWUtils.Events;

/// <summary>
/// 事件处理器
/// </summary>
public delegate void XEventHandler();

/// <summary>
/// 事件处理器
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
/// <param name="args">参数</param>
public delegate void XEventHandler<in T>(T args);

/// <summary>
/// 事件处理器
/// </summary>
/// <typeparam name="T1">参数1类型</typeparam>
/// <typeparam name="T2">参数2类型</typeparam>
/// <param name="args1">参数1</param>
/// <param name="args2">参数2</param>
public delegate void XEventHandler<in T1, in T2>(T1 args1, T2 args2);
