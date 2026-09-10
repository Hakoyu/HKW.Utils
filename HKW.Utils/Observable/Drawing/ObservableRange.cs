using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测范围
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public sealed class ObservableRange<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        IEquatable<IReadOnlyRange<T>>,
        ICloneable<ObservableRange<T>>,
        IRange<T>
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public ObservableRange() { }

    /// <inheritdoc/>
    /// <param name="range">范围</param>
    public ObservableRange(IReadOnlyRange<T> range)
        : this(range.Min, range.Max) { }

    /// <inheritdoc/>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public ObservableRange(T min, T max)
    {
        Min = min;
        Max = max;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _min;

    /// <inheritdoc/>
    public T Min
    {
        get => _min;
        set
        {
            if (_min == value)
                return;
            if (PropertyChanging is not null)
            {
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_Min);
                PropertyChanging.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            }
            _min = value;
            if (PropertyChanged is not null)
            {
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_Min);
                PropertyChanged.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _max;

    /// <inheritdoc/>
    public T Max
    {
        get => _max;
        set
        {
            if (_max == value)
                return;
            PropertyChanging?.Invoke(this, PropertyChangingEventArgs.Cache_Max);
            PropertyChanging?.Invoke(this, PropertyChangingEventArgs.Cache_IsEmpty);
            _max = value;
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Max);
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_IsEmpty);
        }
    }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public bool IsEmpty => Min == T.Zero && Max == T.Zero;

    #region ICloneable
    /// <inheritdoc/>
    public ObservableRange<T> Clone()
    {
        return new(Min, Max);
    }

    object ICloneable.Clone() => Clone();
    #endregion

    #region IEquatable

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyRange<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyRange<T>? other)
    {
        if (other is null)
            return false;
        return this == other;
    }
    #endregion

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{Min={Min},Max={Max}}}";
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
