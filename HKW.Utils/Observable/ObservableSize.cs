using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察大小
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("({Width}, {Height})")]
public class ObservableSize<T>
    : ObservableObjectX,
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

    #region Width
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width = default!;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }
    #endregion

    #region Height
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height = default!;

    /// <inheritdoc/>
    public T Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }
    #endregion

    /// <inheritdoc/>
    public ObservableSize<T> Clone()
    {
        return new(Width, Height);
    }

    object ICloneable.Clone() => Clone();

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
        return other is ObservableSize<T> temp
            && Width.Equals(temp.Width)
            && Height.Equals(temp.Height);
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
