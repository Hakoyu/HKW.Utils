﻿using System.Diagnostics;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读范围
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Min}, {Max})")]
public struct ReadOnlyRange<T> : IEquatable<ReadOnlyRange<T>>, IReadOnlyRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static ReadOnlyRange<T> Empty = new(default, default);

    /// <inheritdoc/>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public ReadOnlyRange(T min, T max)
    {
        Min = min;
        Max = max;
    }

    /// <inheritdoc/>
    public T Min { get; set; }

    /// <inheritdoc/>
    public T Max { get; set; }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        return Equals((ReadOnlyRange<T>)obj);
    }

    /// <inheritdoc/>
    public bool Equals(ReadOnlyRange<T> other)
    {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    /// <inheritdoc/>
    public static bool operator ==(ReadOnlyRange<T> a, IReadOnlyRange<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ReadOnlyRange<T> a, IReadOnlyRange<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Min = {Min}, Max = {Max}";
    }
}
