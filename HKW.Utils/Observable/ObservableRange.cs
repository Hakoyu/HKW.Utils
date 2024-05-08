using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察范围
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Min}, {Max})")]
public class ObservableRange<T>
    : ObservableObjectX,
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

    #region Min
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _min = default!;

    /// <inheritdoc/>
    public T Min
    {
        get => _min;
        set => SetProperty(ref _min, value);
    }
    #endregion

    #region Max
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _max = default!;

    /// <inheritdoc/>
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

    object ICloneable.Clone() => Clone();

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
        return Min.Equals(other.Min) && Max.Equals(other.Max);
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
