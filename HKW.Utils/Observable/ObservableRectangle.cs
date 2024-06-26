﻿using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Observable;

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
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangle()
    {
        NotifyPropertyChanged(
            [nameof(Width), nameof(Height), nameof(X), nameof(Y)],
            [nameof(Left), nameof(Right), nameof(Top), nameof(Bottom)]
        );
    }

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
    private T _width = default!;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }
    #endregion

    #region Height
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height = default!;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }
    #endregion

    #region Y
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y = default!;

    /// <inheritdoc/>
    public T Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
    #endregion
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
