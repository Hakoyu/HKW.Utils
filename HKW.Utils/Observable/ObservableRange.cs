using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察范围
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Min}, {Max})")]
public class ObservableRange<T>
    : ReactiveObjectX,
        IEquatable<ObservableRange<T>>,
        ICloneable<ObservableRange<T>>,
        IRange<T>
    where T : struct, INumber<T>
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

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Min { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Max { get; set; }

    #region Clone
    /// <inheritdoc/>
    public ObservableRange<T> Clone()
    {
        return new(Min, Max);
    }

    object ICloneable.Clone() => Clone();
    #endregion

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
        return Min == other.Min && Max == other.Max;
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableRange<T> a, IReadOnlyRange<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableRange<T> a, IReadOnlyRange<T> b)
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
