using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测矩形位置
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservableRectangle<T>
    : ReactiveObjectX,
        IEquatable<IReadOnlyRectangle<T>>,
        ICloneable<ObservableRectangle<T>>,
        IRectangle<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangle() { }

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    public ObservableRectangle(IReadOnlyRectangle<T> rectangle)
        : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    /// <param name="point">位置</param>
    public ObservableRectangle(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
        : this(point.X, point.Y, size.Width, size.Height) { }

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public ObservableRectangle(T x, T y, T width, T height)
        : this()
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    #region Locotion
    /// <inheritdoc/>
    [ReactiveProperty]
    public T X { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Y { get; set; }
    #endregion

    #region Size
    /// <inheritdoc/>
    [ReactiveProperty]
    public T Width { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Height { get; set; }
    #endregion
    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y), nameof(Width), nameof(Height))]
    public bool IsEmpty => X == T.Zero && Y == T.Zero && Width == T.Zero && Height == T.Zero;

#pragma warning disable S4275
    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X))]
    public T Left => X;

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(Y))]
    public T Top => Y;

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X), nameof(Width))]
    public T Right => unchecked(X + Width);

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(Y), nameof(Height))]
    public T Bottom => unchecked(Y + Height);

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y))]
    public Point<T> LeftTop => new(Left, Top);

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y), nameof(Width))]
    public Point<T> RightTop => new(Right, Top);

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y), nameof(Height))]
    public Point<T> LeftBottom => new(Left, Bottom);

    /// <inheritdoc/>
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y), nameof(Width), nameof(Height))]
    public Point<T> RightBottom => new(Right, Bottom);
#pragma warning restore S4275

    #region Clone
    /// <inheritdoc/>
    public ObservableRectangle<T> Clone()
    {
        return new(X, Y, Width, Height);
    }

    object ICloneable.Clone() => Clone();
    #endregion

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
        return this == other;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{X={X},Y={Y},Width={Width},Height={Height}}}";
    }
}
