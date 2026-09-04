using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测矩形位置
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed class ObservableRectangle<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _x;

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set
        {
            if (_x == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_X);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Left);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Right);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_LeftTop);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightTop);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_LeftBottom);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightBottom);
            }
            _x = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_X);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Left);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Right);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_LeftTop);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightTop);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_LeftBottom);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightBottom);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y;

    /// <inheritdoc/>
    public T Y
    {
        get => _y;
        set
        {
            if (_y == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Y);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Top);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Bottom);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_LeftTop);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightTop);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_LeftBottom);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightBottom);
            }
            _y = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Y);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Top);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Bottom);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_LeftTop);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightTop);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_LeftBottom);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightBottom);
            }
        }
    }
    #endregion

    #region Size
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set
        {
            if (_width == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Width);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Right);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightTop);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightBottom);
            }
            _width = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Width);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Right);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightTop);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightBottom);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height;

    /// <inheritdoc/>
    public T Height
    {
        get => _height;
        set
        {
            if (_height == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Height);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Bottom);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_LeftBottom);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_RightBottom);
            }
            _height = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Height);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Bottom);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_LeftBottom);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_RightBottom);
            }
        }
    }
    #endregion

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public bool IsEmpty => X == T.Zero && Y == T.Zero && Width == T.Zero && Height == T.Zero;

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

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
