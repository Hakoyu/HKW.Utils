﻿using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察矩形位置
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("X = {X}, Y = {Y}, Width = {Width}, Height = {Height}")]
public class ObservableRectangleLocation<T>
    : ObservableObjectX<ObservableRectangleLocation<T>>,
        IEquatable<ObservableRectangleLocation<T>>,
        ICloneable<ObservableRectangleLocation<T>>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangleLocation() { }

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    /// <param name="location">位置</param>
    public ObservableRectangleLocation(
        ObservableRectangle<T> rectangle,
        ObservablePoint<T> location
    )
    {
        Width = rectangle.Width;
        Height = rectangle.Height;
        X = location.X;
        Y = location.Y;
    }

    /// <inheritdoc/>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public ObservableRectangleLocation(T width, T height, T x, T y)
    {
        Width = width;
        Height = height;
        X = x;
        Y = y;
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
        set
        {
            SetProperty(ref _width, value);
            EndX = X + Width;
        }
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
        set
        {
            SetProperty(ref _height, value);
            EndY = Y + Height;
        }
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
        set
        {
            SetProperty(ref _x, value);
            EndX = X + Width;
        }
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
        set
        {
            SetProperty(ref _y, value);
            EndY = Y + Height;
        }
    }
    #endregion
    #endregion

    #region EndLocation
    #region EndX
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _endX = default!;

    /// <summary>
    /// 坐标X
    /// </summary>
    public T EndX
    {
        get => _endX;
        private set => SetProperty(ref _endX, value);
    }
    #endregion

    #region EndY
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _endY = default!;

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T EndY
    {
        get => _endY;
        private set => SetProperty(ref _endY, value);
    }
    #endregion
    #endregion

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <returns>在矩形内为 <see langword="true"/> 不在为 <see langword="false"/></returns>
    public bool InRectangle(T x, T y)
    {
        if (x < X || y < Y)
            return false;
        if (x > EndX || y > EndY)
            return false;
        return true;
    }

    /// <summary>
    /// 复制一个新的对象
    /// </summary>
    /// <returns>新对象</returns>
    public ObservableRectangleLocation<T> Clone()
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
        return Equals(obj as ObservableRectangleLocation<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableRectangleLocation<T>? other)
    {
        if (other is null)
            return false;
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc/>
    public static bool operator ==(
        ObservableRectangleLocation<T> a,
        ObservableRectangleLocation<T> b
    )
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(
        ObservableRectangleLocation<T> a,
        ObservableRectangleLocation<T> b
    )
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
