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
/// 可观察矩形位置
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

    #region Width
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set
        {
            this.RaiseAndSetIfChanged(ref _width, value);
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
        get => _height;
        set
        {
            this.RaiseAndSetIfChanged(ref _height, value);
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
        get => _x;
        set
        {
            this.RaiseAndSetIfChanged(ref _x, value);
            Left = X;
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
        get => _y;
        set
        {
            this.RaiseAndSetIfChanged(ref _y, value);
            Top = Y;
            Bottom = unchecked(Y + Height);
            LeftTop = new(Left, Top);
            RightTop = new(Right, Top);
            RightBottom = new(Right, Bottom);
        }
    }
    #endregion

    #endregion

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Left { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Top { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Right { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Bottom { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public Point<T> LeftTop { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public Point<T> RightTop { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public Point<T> LeftBottom { get; private set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public Point<T> RightBottom { get; private set; }

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
