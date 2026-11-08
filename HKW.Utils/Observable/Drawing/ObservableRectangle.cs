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
public sealed partial class ObservableRectangle<T>
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

    private bool _isChanging;

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_x, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.X);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
                changing.Invoke(this, PropertyChangingArgsCache.Left);
                changing.Invoke(this, PropertyChangingArgsCache.Right);
                changing.Invoke(this, PropertyChangingArgsCache.LeftTop);
                changing.Invoke(this, PropertyChangingArgsCache.RightTop);
                changing.Invoke(this, PropertyChangingArgsCache.LeftBottom);
                changing.Invoke(this, PropertyChangingArgsCache.RightBottom);
            }
            _x = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.X);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
                changed.Invoke(this, PropertyChangedArgsCache.Left);
                changed.Invoke(this, PropertyChangedArgsCache.Right);
                changed.Invoke(this, PropertyChangedArgsCache.LeftTop);
                changed.Invoke(this, PropertyChangedArgsCache.RightTop);
                changed.Invoke(this, PropertyChangedArgsCache.LeftBottom);
                changed.Invoke(this, PropertyChangedArgsCache.RightBottom);
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
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_y, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Y);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
                changing.Invoke(this, PropertyChangingArgsCache.Top);
                changing.Invoke(this, PropertyChangingArgsCache.Bottom);
                changing.Invoke(this, PropertyChangingArgsCache.LeftTop);
                changing.Invoke(this, PropertyChangingArgsCache.RightTop);
                changing.Invoke(this, PropertyChangingArgsCache.LeftBottom);
                changing.Invoke(this, PropertyChangingArgsCache.RightBottom);
            }
            _y = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Y);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
                changed.Invoke(this, PropertyChangedArgsCache.Top);
                changed.Invoke(this, PropertyChangedArgsCache.Bottom);
                changed.Invoke(this, PropertyChangedArgsCache.LeftTop);
                changed.Invoke(this, PropertyChangedArgsCache.RightTop);
                changed.Invoke(this, PropertyChangedArgsCache.LeftBottom);
                changed.Invoke(this, PropertyChangedArgsCache.RightBottom);
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
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            GeometryMath<T>.ValidateSize(value, nameof(Width));
            if (EqualityComparer<T>.Default.Equals(_width, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Width);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
                changing.Invoke(this, PropertyChangingArgsCache.Right);
                changing.Invoke(this, PropertyChangingArgsCache.RightTop);
                changing.Invoke(this, PropertyChangingArgsCache.RightBottom);
            }
            _width = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Width);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
                changed.Invoke(this, PropertyChangedArgsCache.Right);
                changed.Invoke(this, PropertyChangedArgsCache.RightTop);
                changed.Invoke(this, PropertyChangedArgsCache.RightBottom);
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
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            GeometryMath<T>.ValidateSize(value, nameof(Height));
            if (EqualityComparer<T>.Default.Equals(_height, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Height);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
                changing.Invoke(this, PropertyChangingArgsCache.Bottom);
                changing.Invoke(this, PropertyChangingArgsCache.LeftBottom);
                changing.Invoke(this, PropertyChangingArgsCache.RightBottom);
            }
            _height = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Height);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
                changed.Invoke(this, PropertyChangedArgsCache.Bottom);
                changed.Invoke(this, PropertyChangedArgsCache.LeftBottom);
                changed.Invoke(this, PropertyChangedArgsCache.RightBottom);
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
    public T Right => GeometryMath<T>.End(X, Width);

    /// <inheritdoc/>
    public T Bottom => GeometryMath<T>.End(Y, Height);

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
        return EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y)
            && EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);
    }

    /// <summary>
    /// 比较具体矩形值, 避免将值类型参数装箱为接口.
    /// </summary>
    public bool Equals(Rectangle<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y)
            && EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);
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
