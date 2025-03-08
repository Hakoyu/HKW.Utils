namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableList<T>
    : IList<T>,
        IObservableCollection<T>,
        INotifyListChanging<T>,
        INotifyListChanged<T>
{
    /// <summary>
    /// 从索引获取或设置值
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="skipCheck">跳过检查</param>
    /// <returns>值</returns>
    public T this[int index, bool skipCheck] { get; set; }
}
