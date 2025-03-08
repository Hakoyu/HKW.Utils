using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测点
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y})")]
public partial class ObservablePoint<T>
    : ReactiveObjectX,
        IEquatable<ObservablePoint<T>>,
        ICloneable<ObservablePoint<T>>,
        IPoint<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservablePoint() { }

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
    public T X { get; set; } = default!;

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Y { get; set; } = default!;

    #region Clone
    /// <inheritdoc/>
    public ObservablePoint<T> Clone()
    {
        return new(X, Y);
    }

    object ICloneable.Clone() => Clone();
    #endregion

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservablePoint<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservablePoint<T>? other)
    {
        if (other is null)
            return false;
        return X == other.X && Y == other.Y;
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservablePoint<T> a, IReadOnlyPoint<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservablePoint<T> a, IReadOnlyPoint<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"X = {X}, Y = {Y}";
    }
}
