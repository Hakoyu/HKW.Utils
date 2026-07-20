using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测点
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservablePoint<T>
    : ReactiveObjectX,
        IEquatable<IReadOnlyPoint<T>>,
        ICloneable<ObservablePoint<T>>,
        IPoint<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservablePoint() { }

    /// <inheritdoc/>
    /// <param name="point">点</param>
    public ObservablePoint(IReadOnlyPoint<T> point)
        : this(point.X, point.Y) { }

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public ObservablePoint(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T X { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Y { get; set; }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    [NotifyPropertyChangeFrom(nameof(X), nameof(Y))]
    public bool IsEmpty => X == T.Zero && Y == T.Zero;

    #region ICloneable
    /// <inheritdoc/>
    public ObservablePoint<T> Clone()
    {
        return new(X, Y);
    }

    object ICloneable.Clone() => Clone();
    #endregion

    #region IEquatable

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyPoint<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyPoint<T>? other)
    {
        if (other is null)
            return false;
        return this == other;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{X={X},Y={Y}}}";
    }
}
