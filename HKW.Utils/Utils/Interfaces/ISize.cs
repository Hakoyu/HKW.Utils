using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 大小接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISize<T>
    where T : INumber<T>
{
    /// <summary>
    /// 宽
    /// </summary>
    public T Width { get; set; }

    /// <summary>
    /// 高
    /// </summary>
    public T Height { get; set; }
}
