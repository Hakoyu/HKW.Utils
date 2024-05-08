using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 大小
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Width}, {Height})")]
public struct Size<T> : IEquatable<Size<T>>, ISize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static Size<T> Empty = new(default, default);

    /// <inheritdoc/>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    /// <inheritdoc/>
    public T Width { get; set; }

    /// <inheritdoc/>
    public T Height { get; set; }

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
        return Equals((Size<T>)obj);
    }

    /// <inheritdoc/>
    public bool Equals(Size<T> other)
    {
        return Width.Equals(other.Width) && Height.Equals(other.Height);
    }

    /// <inheritdoc/>
    public static bool operator ==(Size<T> a, IReadOnlySize<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(Size<T> a, IReadOnlySize<T> b)
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
