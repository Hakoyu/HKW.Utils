using System.Diagnostics;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读大小
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Width}, {Height})")]
public struct ReadOnlySize<T> : IEquatable<IReadOnlySize<T>>, IReadOnlySize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static ReadOnlySize<T> Empty = new(default, default);

    /// <inheritdoc/>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public ReadOnlySize(T width, T height)
    {
        Width = width;
        Height = height;
    }

    /// <inheritdoc/>
    public T Width { get; }

    /// <inheritdoc/>
    public T Height { get; }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        return Equals((IReadOnlySize<T>)obj);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlySize<T>? other)
    {
        if (other is null)
            return false;
        return Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc/>
    public static bool operator ==(ReadOnlySize<T> a, IReadOnlySize<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ReadOnlySize<T> a, IReadOnlySize<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Width = {Width}, Height = {Height}";
    }
}
