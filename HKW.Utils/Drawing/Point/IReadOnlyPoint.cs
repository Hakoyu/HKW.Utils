using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读点接口
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public interface IReadOnlyPoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 坐标X
    /// </summary>
    public T X { get; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y { get; }
}
