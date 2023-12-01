namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察集合改变事件
/// </summary>
/// <typeparam name="T">类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void ObservableSetChangingEventHandler<T>(
    IObservableSet<T> sender,
    NotifySetChangingEventArgs<T> e
);
