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
public sealed class ObservablePoint<T>
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

    /// <inheritdoc/>
    public T X
    {
        get => _x;
        set
        {
            if (_x == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_X);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            }
            _x = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_X);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
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
            if (_y == value)
                return;
            PropertyChanging?.Invoke(this, PropertyChangingEventArgs.Cache_Y);
            PropertyChanging?.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            _y = value;
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Y);
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
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
        return this == other;
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
