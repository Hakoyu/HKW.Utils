using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public partial struct Size<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从同类型值复制, 不装箱.
    /// </summary>
    public Size(Size<T> other)
        : this(other.Width, other.Height) { }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherSize">其他大小</param>
    /// <exception cref="ArgumentNullException"><paramref name="otherSize"/> 为 null</exception>
    public void SetValue(IReadOnlySize<T> otherSize)
    {
        ArgumentNullException.ThrowIfNull(otherSize);
        SetValue(otherSize.Width, otherSize.Height);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public void SetValue(T width, T height)
    {
        GeometryMath<T>.ValidateSize(width, nameof(width));
        GeometryMath<T>.ValidateSize(height, nameof(height));
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherSize">其他大小</param>
    public void SetValue(Size<T> otherSize)
    {
        SetValue(otherSize.Width, otherSize.Height);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(T x, T y, bool includeBoundary = false)
    {
        return GeometryMath<T>.ContainsPoint(T.Zero, T.Zero, Width, Height, x, y, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    /// <exception cref="ArgumentNullException"><paramref name="point"/> 为 null</exception>
    public readonly bool Contains(IReadOnlyPoint<T> point, bool includeBoundary = false)
    {
        ArgumentNullException.ThrowIfNull(point);
        return Contains(point.X, point.Y, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(Point<T> point, bool includeBoundary = false)
    {
        return Contains(point.X, point.Y, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="otherSize">其它大小</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    /// <exception cref="ArgumentNullException"><paramref name="otherSize"/> 为 null</exception>
    /// <remarks>
    /// 按宽高比较尺寸向量, 即检查另一尺寸的右下端点; 不把共同原点作为严格边界排除.
    /// </remarks>
    public readonly bool Contains(IReadOnlySize<T> otherSize, bool includeBoundary = false)
    {
        ArgumentNullException.ThrowIfNull(otherSize);
        return Contains(otherSize.Width, otherSize.Height, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="otherSize">其它大小</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    /// <remarks>
    /// 按宽高比较尺寸向量, 即检查另一尺寸的右下端点; 不把共同原点作为严格边界排除.
    /// </remarks>
    public readonly bool Contains(Size<T> otherSize, bool includeBoundary = false)
    {
        return Contains(otherSize.Width, otherSize.Height, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="rectangle">矩形</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rectangle"/> 为 null</exception>
    public readonly bool Contains(IReadOnlyRectangle<T> rectangle, bool includeBoundary = false)
    {
        ArgumentNullException.ThrowIfNull(rectangle);
        return GeometryMath<T>.ContainsRectangle(
            T.Zero,
            T.Zero,
            Width,
            Height,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height,
            includeBoundary
        );
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="rectangle">矩形</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(Rectangle<T> rectangle, bool includeBoundary = false)
    {
        return GeometryMath<T>.ContainsRectangle(
            T.Zero,
            T.Zero,
            Width,
            Height,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height,
            includeBoundary
        );
    }
}
