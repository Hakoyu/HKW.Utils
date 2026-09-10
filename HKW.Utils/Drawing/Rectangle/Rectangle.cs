using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 矩形
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public struct Rectangle<T> : IEquatable<IReadOnlyRectangle<T>>, IRectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly Rectangle<T> Empty;

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    public Rectangle(IReadOnlyRectangle<T> rectangle)
        : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    /// <param name="point">位置</param>
    public Rectangle(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
        : this(point.X, point.Y, size.Width, size.Height) { }

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
    public T Width { readonly get; set; }

    /// <inheritdoc/>
    public T Height { readonly get; set; }
    #endregion

    #region Location
    /// <inheritdoc/>
    public T X { readonly get; set; }

    /// <inheritdoc/>
    public T Y { readonly get; set; }
    #endregion

    /// <inheritdoc/>
    public T Left => X;

    /// <inheritdoc/>
    public T Top => Y;

    /// <inheritdoc/>
    public T Right => unchecked(X + Width);

    /// <inheritdoc/>
    public T Bottom => unchecked(Y + Height);

    /// <inheritdoc/>
    public Point<T> LeftTop => new(Left, Top);

    /// <inheritdoc/>
    public Point<T> RightTop => new(Right, Top);

    /// <inheritdoc/>
    public Point<T> LeftBottom => new(Left, Bottom);

    /// <inheritdoc/>
    public Point<T> RightBottom => new(Right, Bottom);

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public readonly bool IsEmpty =>
        X == T.Zero && Y == T.Zero && Width == T.Zero && Height == T.Zero;

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

    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{X={X},Y={Y},Width={Width},Height={Height}}}";
    }

    /// <summary>
    /// 解析字符串数创建矩形
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的矩形</returns>
    /// <remarks><![CDATA[
    /// Parse("123,456,777,888",',')
    /// return:
    /// {X=123,Y=456,Width=777,Height=888}
    /// ]]></remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Rectangle<T> Parse(ReadOnlySpan<char> span, char separator = ',')
    {
        var rect = new Rectangle<T>();
        var datas = span.Split(separator);
        datas.MoveNext();
        rect.X = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        rect.Y = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        rect.Width = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        rect.Height = T.Parse(span[datas.Current], null);
        return rect;
    }
}
