namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察列表改变后事件
/// </summary>
/// <typeparam name="T">类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void ObservableListChangedEventHandler<T>(
    IObservableList<T> sender,
    NotifyListChangeEventArgs<T> e
);
