namespace HKW.HKWUtils.Events;

/// <summary>
/// 异步返回结果的事件处理器
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
/// <returns>异步结果</returns>
public delegate Task<TResult> XAsyncResultEventHandler<TResult>();

/// <summary>
/// 异步返回结果的事件处理器
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="args">参数</param>
/// <returns>异步结果</returns>
public delegate Task<TResult> XAsyncResultEventHandler<in T, TResult>(T args);

/// <summary>
/// 异步返回结果的事件处理器
/// </summary>
/// <typeparam name="T1">参数1类型</typeparam>
/// <typeparam name="T2">参数2类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="args1">参数1</param>
/// <param name="args2">参数2</param>
/// <returns>异步结果</returns>
public delegate Task<TResult> XAsyncResultEventHandler<in T1, in T2, TResult>(T1 args1, T2 args2);
