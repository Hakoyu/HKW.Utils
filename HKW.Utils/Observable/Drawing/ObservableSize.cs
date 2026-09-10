using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测大小
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservableSize<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _width;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set
        {
            if (_width == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Width);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            }
            _width = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Width);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _height;

    /// <inheritdoc/>
    public T Height
    {
        get => _height;
        set
        {
            if (_height == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Height);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            }
            _height = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Height);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
            }
        }
    }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
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

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
