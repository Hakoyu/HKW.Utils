namespace HKW.HKWUtils;

/// <summary>
/// 可克隆接口
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface ICloneable<T> : ICloneable
{
    /// <summary>
    /// 克隆当前对象
    /// </summary>
    /// <returns>新对象</returns>
    public new T Clone();
}
