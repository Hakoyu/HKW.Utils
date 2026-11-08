using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 三维矩形(立方体)接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
/// <remarks>
/// 直接坐标与 Location, 直接尺寸与 Size 必须保持同步; 实现者应拒绝负尺寸, 但允许负坐标.
/// </remarks>
public interface IRectangle3D<T>
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

    /// <summary>
    /// 坐标Z
    /// </summary>
    public T Z { get; set; }

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

    /// <summary>
    /// 大小
    /// </summary>
    public ISize3D<T> Size { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public IPoint3D<T> Location { get; set; }
}
