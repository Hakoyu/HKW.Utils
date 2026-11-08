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
        IEquatable<IReadOnlySize<T>>,
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

    private bool _isChanging;

    /// <inheritdoc/>
    public T Width
    {
        get => _width;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            GeometryMath<T>.ValidateSize(value, nameof(Width));
            if (EqualityComparer<T>.Default.Equals(_width, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Width);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _width = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Width);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
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
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            GeometryMath<T>.ValidateSize(value, nameof(Height));
            if (EqualityComparer<T>.Default.Equals(_height, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Height);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _height = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Height);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
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
        return Equals(obj as IReadOnlySize<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableSize<T>? other) => Equals((IReadOnlySize<T>?)other);

    /// <summary>
    /// 按数值字段比较不同尺寸实现, NaN 使用数值相等语义.
    /// </summary>
    public bool Equals(IReadOnlySize<T>? other)
    {
        return other is not null
            && EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);
    }

    /// <summary>
    /// 比较具体尺寸值, 避免将值类型参数装箱为接口.
    /// </summary>
    public bool Equals(Size<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);
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
