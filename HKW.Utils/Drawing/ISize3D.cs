using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 三维大小接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISize3D<T>
    where T : INumber<T>
{
    /// <summary>
    /// X轴大小
    /// </summary>
    public T SizeX { get; set; }

    /// <summary>
    /// Y轴大小
    /// </summary>
    public T SizeY { get; set; }

    /// <summary>
    /// Z轴大小
    /// </summary>
    public T SizeZ { get; set; }
}
