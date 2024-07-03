using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读矩形接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IReadOnlyRectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 宽
    /// </summary>
    public T Width { get; }

    /// <summary>
    /// 高
    /// </summary>
    public T Height { get; }

    /// <summary>
    /// 坐标X
    /// </summary>
    public T X { get; set; }

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y { get; set; }

    /// <summary>
    /// 左坐标
    /// </summary>
    public T Left { get; }

    /// <summary>
    /// 右坐标
    /// </summary>
    public T Right { get; }

    /// <summary>
    /// 上坐标
    /// </summary>
    public T Top { get; }

    /// <summary>
    /// 下坐标
    /// </summary>
    public T Bottom { get; }

    /// <summary>
    /// 左上角
    /// </summary>
    public ReadOnlyPoint<T> LeftTop { get; }

    /// <summary>
    /// 右上角
    /// </summary>
    public ReadOnlyPoint<T> RightTop { get; }

    /// <summary>
    /// 左下角
    /// </summary>
    public ReadOnlyPoint<T> LeftBottom { get; }

    /// <summary>
    /// 右下角
    /// </summary>
    public ReadOnlyPoint<T> RightBottom { get; }
}
