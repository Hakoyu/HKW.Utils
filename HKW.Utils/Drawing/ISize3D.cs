using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 三维大小接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
/// <remarks>
/// 实现者应拒绝负的轴尺寸; 与三维矩形组合时, 轴尺寸须与矩形的对应属性保持同步.
/// </remarks>
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
