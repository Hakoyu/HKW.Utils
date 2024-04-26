using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察矩形位置
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y}, {Width}, {Height})")]
public class ObservableRectangle<T>
    : ObservableObjectX,
        IEquatable<ObservableRectangle<T>>,
        ICloneable<ObservableRectangle<T>>,
        IRectangle<T>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangle()
    {
        NotifyPropertyOnPropertyChanged(
            [nameof(Width), nameof(Height), nameof(X), nameof(Y)],
            [nameof(TopLeft), nameof(TopRight), nameof(BottomLeft), nameof(BottomRight)]
        );
    }

    /// <inheritdoc/>
    /// <param name="size">矩形</param>
    /// <param name="point">位置</param>
    public ObservableRectangle(IPoint<T> point, ISize<T> size)
        : this()
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
    public ObservableRectangle(T x, T y, T width, T height)
        : this()
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    #region Rectangle
    #region Width
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width = default!;

    /// <summary>
    /// 宽度
    /// </summary>
    public T Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }
    #endregion

    #region Height
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height = default!;

    /// <summary>
    /// 高度
    /// </summary>
    public T Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }
    #endregion
    #endregion

    #region Location
    #region X
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _x = default!;

    /// <summary>
    /// 坐标X
    /// </summary>
    public T X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }
    #endregion

    #region Y
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y = default!;

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
    #endregion
    #endregion

    /// <inheritdoc/>
    public T Left => throw new NotImplementedException();

    /// <inheritdoc/>
    public T Right => throw new NotImplementedException();

    /// <inheritdoc/>
    public T Top => throw new NotImplementedException();

    /// <inheritdoc/>
    public T Bottom => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISize<T> Size
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public IPoint<T> Location
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IPoint<T> TopLeft => throw new NotImplementedException();

    public IPoint<T> TopRight => throw new NotImplementedException();

    public IPoint<T> BottomLeft => throw new NotImplementedException();

    public IPoint<T> BottomRight => throw new NotImplementedException();

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
        if (x > BottomRight.X || y > BottomRight.Y)
            return false;
        return true;
    }

    /// <summary>
    /// 复制一个新的对象
    /// </summary>
    /// <returns>新对象</returns>
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
    public static bool operator ==(ObservableRectangle<T> a, ObservableRectangle<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableRectangle<T> a, ObservableRectangle<T> b)
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
