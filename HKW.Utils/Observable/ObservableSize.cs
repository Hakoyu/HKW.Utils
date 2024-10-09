using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测大小
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Width}, {Height})")]
public partial class ObservableSize<T>
    : ReactiveObjectX,
        IEquatable<ObservableSize<T>>,
        ICloneable<ObservableSize<T>>,
        ISize<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableSize() { }

    /// <inheritdoc/>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public ObservableSize(T width, T height)
    {
        Width = width;
        Height = height;
    }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Width { get; set; }

    /// <inheritdoc/>
    [ReactiveProperty]
    public T Height { get; set; }

    #region Clone
    /// <inheritdoc/>
    public ObservableSize<T> Clone()
    {
        return new(Width, Height);
    }

    object ICloneable.Clone() => Clone();
    #endregion

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableSize<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableSize<T>? other)
    {
        if (other is null)
            return false;
        return Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableSize<T> a, IReadOnlySize<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableSize<T> a, IReadOnlySize<T> b)
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
