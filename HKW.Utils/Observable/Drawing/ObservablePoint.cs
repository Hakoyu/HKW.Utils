using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测点
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed partial class ObservablePoint<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        IEquatable<IReadOnlyPoint<T>>,
        ICloneable<ObservablePoint<T>>,
        IPoint<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservablePoint() { }

    /// <inheritdoc/>
    /// <param name="point">点</param>
    public ObservablePoint(IReadOnlyPoint<T> point)
        : this(point.X, point.Y) { }

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public ObservablePoint(T x, T y)
    {
        X = x;
        Y = y;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _x;

    private bool _isChanging;

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_x, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.X);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _x = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.X);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _y;

    /// <inheritdoc/>
    public T Y
    {
        get => _y;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_y, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Y);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _y = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Y);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
            }
        }
    }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public bool IsEmpty => X == T.Zero && Y == T.Zero;

    #region ICloneable
    /// <inheritdoc/>
    public ObservablePoint<T> Clone()
    {
        return new(X, Y);
    }

    object ICloneable.Clone() => Clone();
    #endregion

    #region IEquatable

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyPoint<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyPoint<T>? other)
    {
        if (other is null)
            return false;
        return EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    /// <summary>
    /// 比较具体点值, 避免将值类型参数装箱为接口.
    /// </summary>
    public bool Equals(Point<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{X={X},Y={Y}}}";
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
