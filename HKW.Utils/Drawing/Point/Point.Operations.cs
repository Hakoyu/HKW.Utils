using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public partial struct Point<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从同类型值复制.
    /// </summary>
    public Point(Point<T> other)
        : this(other.X, other.Y) { }

    /// <summary>
    /// 从尺寸转换为点.
    /// </summary>
    public Point(Size<T> size)
        : this(size.Width, size.Height) { }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="other">其他点</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> 为 null</exception>
    public void SetValue(IReadOnlyPoint<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        SetValue(other.X, other.Y);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public void SetValue(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// 偏移
    /// </summary>
    /// <param name="dx">偏移量X</param>
    /// <param name="dy">偏移量Y</param>
    public void Offset(T dx, T dy)
    {
        var x = unchecked(X + dx);
        var y = unchecked(Y + dy);
        SetValue(x, y);
    }

    /// <summary>
    /// 偏移
    /// </summary>
    /// <param name="p">偏移量</param>
    public void Offset(IReadOnlyPoint<T> p)
    {
        ArgumentNullException.ThrowIfNull(p);
        Offset(p.X, p.Y);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="other">其他点</param>
    public void SetValue(Point<T> other)
    {
        X = other.X;
        Y = other.Y;
    }

    /// <summary>
    /// 偏移
    /// </summary>
    /// <param name="p">偏移量</param>
    public void Offset(Point<T> p)
    {
        Offset(p.X, p.Y);
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Point<T> Add(Size<T> size)
    {
        return new(unchecked(X + size.Width), unchecked(Y + size.Height));
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Point<T> Add(IReadOnlySize<T> size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return new(unchecked(X + size.Width), unchecked(Y + size.Height));
    }

    /// <summary>
    /// 按尺寸分量减少并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Point<T> Subtract(Size<T> size)
    {
        return new(unchecked(X - size.Width), unchecked(Y - size.Height));
    }

    /// <summary>
    /// 按尺寸分量减少并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Point<T> Subtract(IReadOnlySize<T> size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return new(unchecked(X - size.Width), unchecked(Y - size.Height));
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值.
    /// </summary>
    public static Point<T> operator +(Point<T> value, Size<T> size) => value.Add(size);

    /// <summary>
    /// 按尺寸分量减少并返回新值.
    /// </summary>
    public static Point<T> operator -(Point<T> value, Size<T> size) => value.Subtract(size);
}
