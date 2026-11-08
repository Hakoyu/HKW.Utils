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
public sealed partial class ObservableRange<T>
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

    private bool _isChanging;

    /// <inheritdoc/>
    public T Min
    {
        get => _min;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_min, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Min);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _min = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Min);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
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
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_max, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            if (changing is not null)
            {
                changing.Invoke(this, PropertyChangingArgsCache.Max);
                changing.Invoke(this, PropertyChangingArgsCache.IsEmpty);
            }
            _max = value;
            if (changed is not null)
            {
                changed.Invoke(this, PropertyChangedArgsCache.Max);
                changed.Invoke(this, PropertyChangedArgsCache.IsEmpty);
            }
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
        return EqualityComparer<T>.Default.Equals(Min, other.Min)
            && EqualityComparer<T>.Default.Equals(Max, other.Max);
    }

    /// <summary>
    /// 比较具体范围值, 避免将值类型参数装箱为接口.
    /// </summary>
    public bool Equals(Range<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Min, other.Min)
            && EqualityComparer<T>.Default.Equals(Max, other.Max);
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
