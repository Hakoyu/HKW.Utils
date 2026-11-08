using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public partial struct Rectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从同类型值复制, 不装箱.
    /// </summary>
    public Rectangle(Rectangle<T> other)
        : this(other.X, other.Y, other.Width, other.Height) { }

    /// <summary>
    /// 从位置和尺寸创建矩形, 不装箱.
    /// </summary>
    public Rectangle(Point<T> point, Size<T> size)
        : this(point.X, point.Y, size.Width, size.Height) { }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherRectangle">矩形</param>
    /// <exception cref="ArgumentNullException"><paramref name="otherRectangle"/> 为 null</exception>
    public void SetValue(IReadOnlyRectangle<T> otherRectangle)
    {
        ArgumentNullException.ThrowIfNull(otherRectangle);

        SetValue(otherRectangle.X, otherRectangle.Y, otherRectangle.Width, otherRectangle.Height);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="point">位置</param>
    /// <param name="size">大小</param>
    /// <exception cref="ArgumentNullException"><paramref name="point"/> 或 <paramref name="size"/> 为 null</exception>
    public void SetValue(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
    {
        ArgumentNullException.ThrowIfNull(point);
        ArgumentNullException.ThrowIfNull(size);

        SetValue(point.X, point.Y, size.Width, size.Height);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public void SetValue(T x, T y, T width, T height)
    {
        GeometryMath<T>.ValidateSize(width, nameof(width));
        GeometryMath<T>.ValidateSize(height, nameof(height));
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherRectangle">矩形</param>
    public void SetValue(Rectangle<T> otherRectangle)
    {
        SetValue(otherRectangle.X, otherRectangle.Y, otherRectangle.Width, otherRectangle.Height);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="point">位置</param>
    /// <param name="size">大小</param>
    public void SetValue(Point<T> point, Size<T> size)
    {
        SetValue(point.X, point.Y, size.Width, size.Height);
    }

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    public readonly bool Contains(T x, T y, bool includeBoundary = false)
    {
        return GeometryMath<T>.ContainsPoint(X, Y, Width, Height, x, y, includeBoundary);
    }

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="point">坐标</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    /// <exception cref="ArgumentNullException"><paramref name="point"/> 为 null</exception>
    public readonly bool Contains(IReadOnlyPoint<T> point, bool includeBoundary = false)
    {
        ArgumentNullException.ThrowIfNull(point);
        return Contains(point.X, point.Y, includeBoundary);
    }

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="point">坐标</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    public readonly bool Contains(Point<T> point, bool includeBoundary = false)
    {
        return Contains(point.X, point.Y, includeBoundary);
    }

    /// <summary>
    /// 矩形在矩形内
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    public readonly bool Contains(T x, T y, T width, T height, bool includeBoundary = false)
    {
        return GeometryMath<T>.ContainsRectangle(
            X,
            Y,
            Width,
            Height,
            x,
            y,
            width,
            height,
            includeBoundary
        );
    }

    /// <summary>
    /// 矩形在矩形内
    /// </summary>
    /// <param name="otherRectangle">其它矩形</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    /// <exception cref="ArgumentNullException"><paramref name="otherRectangle"/> 为 null</exception>
    public readonly bool Contains(
        IReadOnlyRectangle<T> otherRectangle,
        bool includeBoundary = false
    )
    {
        ArgumentNullException.ThrowIfNull(otherRectangle);
        return Contains(
            otherRectangle.X,
            otherRectangle.Y,
            otherRectangle.Width,
            otherRectangle.Height,
            includeBoundary
        );
    }

    /// <summary>
    /// 矩形在矩形内
    /// </summary>
    /// <param name="otherRectangle">其它矩形</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否在内</returns>
    public readonly bool Contains(Rectangle<T> otherRectangle, bool includeBoundary = false)
    {
        return Contains(
            otherRectangle.X,
            otherRectangle.Y,
            otherRectangle.Width,
            otherRectangle.Height,
            includeBoundary
        );
    }
}
