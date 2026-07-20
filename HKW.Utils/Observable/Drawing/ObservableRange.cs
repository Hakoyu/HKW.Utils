using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测范围
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservableRange<T>
    : ReactiveObjectX,
        IEquatable<IReadOnlyRange<T>>,
        ICloneable<ObservableRange<T>>,
        IRange<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableRange() { }

    /// <inheritdoc/>
    /// <param name="range">范围</param>
    public ObservableRange(IReadOnlyRange<T> range)
        : this(range.Min, range.Max) { }

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

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    [NotifyPropertyChangeFrom(nameof(Min), nameof(Max))]
    public bool IsEmpty => Min == T.Zero && Max == T.Zero;

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
        return Equals(obj as IReadOnlyRange<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyRange<T>? other)
    {
        if (other is null)
            return false;
        return this == other;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{Min={Min},Max={Max}}}";
    }
}
