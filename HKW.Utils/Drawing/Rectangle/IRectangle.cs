using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 矩形接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IRectangle<T> : IReadOnlyRectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 宽
    /// </summary>
    public new T Width { get; set; }

    /// <summary>
    /// 高
    /// </summary>
    public new T Height { get; set; }

    /// <summary>
    /// 坐标X
    /// </summary>
    public new T X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public new T Y { get; set; }

    /// <summary>
    /// 左坐标
    /// </summary>
    public new T Left { get; }

    /// <summary>
    /// 右坐标
    /// </summary>
    public new T Right { get; }

    /// <summary>
    /// 上坐标
    /// </summary>
    public new T Top { get; }

    /// <summary>
    /// 下坐标
    /// </summary>
    public new T Bottom { get; }
}
