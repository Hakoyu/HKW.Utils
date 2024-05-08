using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读点
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y})")]
public struct Point<T> : IEquatable<IReadOnlyPoint<T>>, IPoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static Point<T> Empty = new(default, default);

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public Point(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    /// <param name="point">点接口</param>
    public Point(IReadOnlyPoint<T> point)
    {
        X = point.X;
        Y = point.Y;
    }

    /// <inheritdoc/>
    public T X { get; set; }

    /// <inheritdoc/>
    public T Y { get; set; }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        return Equals((IReadOnlyPoint<T>)obj);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyPoint<T>? other)
    {
        if (other is null)
            return false;
        return X == other.X && Y == other.Y;
    }

    /// <inheritdoc/>
    public static bool operator ==(Point<T> a, IReadOnlyPoint<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(Point<T> a, IReadOnlyPoint<T> b)
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
