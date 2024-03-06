﻿using System.Numerics;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察矩形
/// </summary>
/// <typeparam name="T">类型</typeparam>
public class ObservableRectangle<T>
    : ViewModelBase<ObservableRectangle<T>>,
        IEquatable<ObservableRectangle<T>>,
        ICloneable<ObservableRectangle<T>>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangle() { }

    /// <inheritdoc/>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public ObservableRectangle(T width, T height)
    {
        Width = width;
        Height = height;
    }

    #region Width
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

    /// <inheritdoc/>
    public ObservableRectangle<T> Clone()
    {
        return new(Width, Height);
    }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ObservableRectangle<T> temp
            && EqualityComparer<T>.Default.Equals(Width, temp.Width)
            && EqualityComparer<T>.Default.Equals(Height, temp.Height);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableRectangle<T>? other)
    {
        return Equals(obj: other);
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableRectangle<T> a, ObservableRectangle<T> b)
    {
        return Equals(a, b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableRectangle<T> a, ObservableRectangle<T> b)
    {
        return Equals(a, b) is not true;
    }
    #endregion
}