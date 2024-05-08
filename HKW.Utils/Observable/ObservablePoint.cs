using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察点
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y})")]
public class ObservablePoint<T>
    : ObservableObjectX,
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

    #region X
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _x = default!;

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }
    #endregion

    #region Y
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y = default!;

    /// <inheritdoc/>
    public T Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
    #endregion

    /// <inheritdoc/>
    public ObservablePoint<T> Clone()
    {
        return new(X, Y);
    }

    object ICloneable.Clone() => Clone();

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
        return X.Equals(other.X) && Y.Equals(other.Y);
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
