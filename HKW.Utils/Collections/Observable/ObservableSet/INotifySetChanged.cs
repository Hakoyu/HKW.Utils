namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变后接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifySetChanged<T>
{
    /// <summary>
    /// 列表改变后事件
    /// </summary>
    public event ObservableSetChangedEventHandler<T>? SetChanged;
}
