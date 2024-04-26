using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 点接口
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public interface IPoint<T>
    where T : INumber<T>
{
    /// <summary>
    /// 坐标X
    /// </summary>
    public T X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y { get; set; }
}
