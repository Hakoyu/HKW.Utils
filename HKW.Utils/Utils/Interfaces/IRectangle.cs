using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 矩形接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IRectangle<T>
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

    /// <summary>
    /// 坐标X
    /// </summary>
    public T X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y { get; set; }

    /// <summary>
    /// 左边长
    /// </summary>
    public T Left { get; }

    /// <summary>
    /// 右边长
    /// </summary>
    public T Right { get; }

    /// <summary>
    /// 上边长
    /// </summary>
    public T Top { get; }

    /// <summary>
    /// 下边长
    /// </summary>
    public T Bottom { get; }

    /// <summary>
    /// 大小
    /// </summary>
    public ISize<T> Size { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public IPoint<T> Location { get; set; }

    /// <summary>
    /// 左上角位置
    /// </summary>
    public IPoint<T> TopLeft { get; }

    /// <summary>
    /// 右上角位置
    /// </summary>
    public IPoint<T> TopRight { get; }

    /// <summary>
    /// 左下角位置
    /// </summary>
    public IPoint<T> BottomLeft { get; }

    /// <summary>
    /// 右下角位置
    /// </summary>
    public IPoint<T> BottomRight { get; }
}
