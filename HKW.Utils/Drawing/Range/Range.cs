using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 范围
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Min}, {Max})")]
public struct Range<T> : IEquatable<Range<T>>, IRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static Range<T> Empty = new(default, default);

    /// <inheritdoc/>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public Range(T min, T max)
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
        return Equals((Range<T>)obj);
    }

    /// <inheritdoc/>
    public bool Equals(Range<T> other)
    {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    /// <inheritdoc/>
    public static bool operator ==(Range<T> a, IReadOnlyRange<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(Range<T> a, IReadOnlyRange<T> b)
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
