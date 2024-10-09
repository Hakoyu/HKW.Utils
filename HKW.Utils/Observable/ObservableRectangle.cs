using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测矩形位置
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y}, {Width}, {Height})")]
public partial class ObservableRectangle<T>
    : ReactiveObjectX,
        IEquatable<ObservableRectangle<T>>,
        ICloneable<ObservableRectangle<T>>,
        IRectangle<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangle() { }

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    public ObservableRectangle(IReadOnlyRectangle<T> rectangle)
        : this()
    {
        Width = rectangle.Width;
        Height = rectangle.Height;
        X = rectangle.X;
        Y = rectangle.Y;
    }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    /// <param name="point">位置</param>
    public ObservableRectangle(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
        : this()
    {
        Width = size.Width;
        Height = size.Height;
        X = point.X;
        Y = point.Y;
    }

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

    #region Size
    /// <inheritdoc/>
    [ReactiveProperty]
    public T Width { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Height { get; set; }
    #endregion

    #region Locotion
    /// <inheritdoc/>
    [ReactiveProperty]
    public T X { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Y { get; set; }
    #endregion

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

    #region Clone
    /// <inheritdoc/>
    public ObservableRectangle<T> Clone()
    {
        return new()
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
        };
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
        return Equals(obj as ObservableRectangle<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableRectangle<T>? other)
    {
        if (other is null)
            return false;
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableRectangle<T> a, IReadOnlyRectangle<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableRectangle<T> a, IReadOnlyRectangle<T> b)
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
