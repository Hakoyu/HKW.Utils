namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表改变前接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifyListChanging<T>
{
    /// <summary>
    /// 列表改变前事件
    /// </summary>
    public event ObservableListChangingEventHandler<T>? ListChanging;
}
