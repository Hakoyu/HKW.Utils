using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 矩形
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y}, {Width}, {Height})")]
public struct Rectangle<T> : IEquatable<IReadOnlyRectangle<T>>, IRectangle<T>
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
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public Rectangle(T width, T height)
    {
        Width = width;
        Height = height;
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

    /// <inheritdoc/>
    /// <param name="data">数据</param>
    /// <param name="separator">分割符</param>
    public Rectangle(string data, char separator = ',')
    {
        var datas = data.AsSpan().Split(separator);
        datas.MoveNext();
        X = T.Parse(datas.Current, null);
        datas.MoveNext();
        Y = T.Parse(datas.Current, null);
        datas.MoveNext();
        Width = T.Parse(datas.Current, null);
        datas.MoveNext();
        Height = T.Parse(datas.Current, null);
    }

    #region Size

    #region Width
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width;

    /// <inheritdoc/>
    public T Width
    {
        readonly get => _width;
        set
        {
            _width = value;
            Right = unchecked(X + Width);
        }
    }
    #endregion

    #region Height
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height;

    /// <inheritdoc/>
    public T Height
    {
        readonly get => _height;
        set
        {
            _height = value;
            Bottom = unchecked(Y + Height);
        }
    }
    #endregion
    #endregion

    #region Location

    #region X
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _x;

    /// <inheritdoc/>
    public T X
    {
        readonly get => _x;
        set
        {
            _x = value;
            Right = unchecked(X + Width);
            LeftTop = new(Left, Top);
            LeftBottom = new(Left, Bottom);
            RightBottom = new(Right, Bottom);
        }
    }
    #endregion

    #region Y
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y;

    /// <inheritdoc/>
    public T Y
    {
        readonly get => _y;
        set
        {
            _y = value;
            Bottom = unchecked(Y + Height);
            LeftTop = new(Left, Top);
            RightTop = new(Right, Top);
            RightBottom = new(Right, Bottom);
        }
    }
    #endregion

    #endregion

    /// <inheritdoc/>
    public T Left => X;

    /// <inheritdoc/>
    public T Top => Y;

    /// <inheritdoc/>
    public T Right { readonly get; private set; }

    /// <inheritdoc/>
    public T Bottom { readonly get; private set; }

    /// <inheritdoc/>
    public Point<T> LeftTop { readonly get; private set; }

    /// <inheritdoc/>
    public Point<T> RightTop { readonly get; private set; }

    /// <inheritdoc/>
    public Point<T> LeftBottom { readonly get; private set; }

    /// <inheritdoc/>
    public Point<T> RightBottom { readonly get; private set; }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height, X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyRectangle<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyRectangle<T>? other)
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
        return $"{{X = {X}, Y = {Y}, Width = {Width}, Height = {Height}}}";
    }
}
