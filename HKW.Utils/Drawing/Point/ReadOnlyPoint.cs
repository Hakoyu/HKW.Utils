using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读点
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[DebuggerDisplay("({X}, {Y})")]
public struct ReadOnlyPoint<T> : IEquatable<IReadOnlyPoint<T>>, IReadOnlyPoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly ReadOnlyPoint<T> Empty = new(default(T), default(T));

    #region ctor
    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public ReadOnlyPoint(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    /// <param name="point">点接口</param>
    public ReadOnlyPoint(IReadOnlyPoint<T> point)
        : this(point.X, point.Y) { }

    /// <inheritdoc/>
    /// <param name="data">数据</param>
    /// <param name="separator">分割符</param>
    public ReadOnlyPoint(string data, char separator = ',')
    {
        var datas = data.AsSpan().Split(separator);
        datas.MoveNext();
        X = T.Parse(datas.Current, null);
        datas.MoveNext();
        Y = T.Parse(datas.Current, null);
    }
    #endregion

    /// <inheritdoc/>
    public T X { get; }

    /// <inheritdoc/>
    public T Y { get; }

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
    public static bool operator ==(ReadOnlyPoint<T> a, IReadOnlyPoint<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ReadOnlyPoint<T> a, IReadOnlyPoint<T> b)
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
