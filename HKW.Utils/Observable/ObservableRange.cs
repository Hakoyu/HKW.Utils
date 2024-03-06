﻿using System.Numerics;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察范围
/// </summary>
/// <typeparam name="T">类型</typeparam>
public class ObservableRange<T>
    : ViewModelBase<ObservableRange<T>>,
        IEquatable<ObservableRange<T>>,
        ICloneable<ObservableRange<T>>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservableRange() { }

    /// <inheritdoc/>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public ObservableRange(T min, T max)
    {
        Min = min;
        Max = max;
    }

    #region Min
    private T _min = default!;

    /// <summary>
    /// 最小值
    /// </summary>
    public T Min
    {
        get => _min;
        set => SetProperty(ref _min, value);
    }
    #endregion

    #region Max
    private T _max = default!;

    /// <summary>
    /// 最大值
    /// </summary>
    public T Max
    {
        get => _max;
        set => SetProperty(ref _max, value);
    }
    #endregion

    /// <inheritdoc/>
    public ObservableRange<T> Clone()
    {
        return new(Min, Max);
    }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableRange<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableRange<T>? other)
    {
        if (other is null)
            return false;
        return EqualityComparer<T>.Default.Equals(Min, other.Min)
            && EqualityComparer<T>.Default.Equals(Max, other.Max);
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableRange<T> a, ObservableRange<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableRange<T> a, ObservableRange<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
}