using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测大小
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservableSize<T>
    : ReactiveObjectX,
        IEquatable<ObservableSize<T>>,
        ICloneable<ObservableSize<T>>,
        ISize<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableSize() { }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    public ObservableSize(IReadOnlySize<T> size)
        : this(size.Width, size.Height) { }

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

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    [NotifyPropertyChangeFrom(nameof(Width), nameof(Height))]
    public bool IsEmpty => Width == T.Zero && Height == T.Zero;

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
        return this == other;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{Width={Width},Height={Height}}}";
    }
}
