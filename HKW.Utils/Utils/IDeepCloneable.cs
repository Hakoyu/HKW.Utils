namespace HKW.HKWUtils;

/// <summary>
/// 可深克隆接口
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
///
public interface IDeepCloneable<T>
{
    /// <summary>
    /// 深克隆克隆当前对象
    /// </summary>
    /// <returns>新对象</returns>
    public T DeepClone();
}
