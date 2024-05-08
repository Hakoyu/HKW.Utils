using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 可观察矩形位置
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y}, {Width}, {Height})")]
public class Rectangle<T> : IEquatable<Rectangle<T>>, IRectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static Rectangle<T> Empty = new(default, default, default, default);

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    public Rectangle(IReadOnlyRectangle<T> rectangle)
    {
        Width = rectangle.Width;
        Height = rectangle.Height;
        X = rectangle.X;
        Y = rectangle.Y;
    }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    /// <param name="point">位置</param>
    public Rectangle(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
    {
        Width = size.Width;
        Height = size.Height;
        X = point.X;
        Y = point.Y;
    }

    /// <inheritdoc/>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public Rectangle(T x, T y, T width, T height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    #region Size

    /// <inheritdoc/>
    public T Width { get; set; }

    /// <inheritdoc/>
    public T Height { get; set; }
    #endregion

    #region Location


    /// <inheritdoc/>
    public T X { get; set; }

    /// <inheritdoc/>
    public T Y { get; set; }
    #endregion

    /// <inheritdoc/>
    public T Left => X;

    /// <inheritdoc/>
    public T Top => Y;

    /// <inheritdoc/>
    public T Right => unchecked(X + Width);

    /// <inheritdoc/>
    public T Bottom => unchecked(Y + Height);

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <returns>在矩形内为 <see langword="true"/> 不在为 <see langword="false"/></returns>
    public bool Contains(T x, T y)
    {
        if (x < X || y < Y)
            return false;
        if (x > Right || y > Bottom)
            return false;
        return true;
    }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height, X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Rectangle<T>);
    }

    /// <inheritdoc/>
    public bool Equals(Rectangle<T>? other)
    {
        if (other is null)
            return false;
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc/>
    public static bool operator ==(Rectangle<T> a, IReadOnlyRectangle<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(Rectangle<T> a, IReadOnlyRectangle<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"X = {X}, Y = {Y}, Width = {Width}, Height = {Height}";
    }
}
