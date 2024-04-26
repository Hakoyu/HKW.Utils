using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 三维矩形(立方体)接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IRectangle3D<T>
    where T : INumber<T>
{
    /// <summary>
    /// 坐标X
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// 坐标Z
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// X轴大小
    /// </summary>
    public double SizeX { get; set; }

    /// <summary>
    /// Y轴大小
    /// </summary>
    public double SizeY { get; set; }

    /// <summary>
    /// Z轴大小
    /// </summary>
    public double SizeZ { get; set; }

    /// <summary>
    /// 大小
    /// </summary>
    public ISize3D<T> Size { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public IPoint3D<T> Location { get; set; }
}
