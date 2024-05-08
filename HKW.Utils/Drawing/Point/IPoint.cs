using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 点接口
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public interface IPoint<T> : IReadOnlyPoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 坐标X
    /// </summary>
    public new T X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public new T Y { get; set; }
}
