using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测值
/// </summary>
/// <typeparam name="T">值类型</typeparam>
[DebuggerDisplay("{Value}")]
public sealed class ObservableValue<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        IEquatable<ObservableValue<T>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _value = default!;

    private bool _isChanging;

    /// <summary>
    /// 值
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;
            var changing = mutation.Changing;
            var changed = mutation.Changed;
            changing?.Invoke(this, PropertyChangingArgsCache.Value);
            _value = value;
            changed?.Invoke(this, PropertyChangedArgsCache.Value);
        }
    }

    #region Ctor
    /// <inheritdoc/>
    public ObservableValue()
    {
        Value = default!;
    }

    /// <inheritdoc/>
    /// <param name="value">初始值</param>
    public ObservableValue(T value)
    {
        Value = value;
    }
    #endregion

    #region IEquatable
    /// <inheritdoc/>
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableValue<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableValue<T>? other)
    {
        return other is ObservableValue<T> value
            && EqualityComparer<T>.Default.Equals(Value, value.Value);
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableValue<T>? a, ObservableValue<T>? b)
    {
        return ReferenceEquals(a, b) || (a is not null && a.Equals(b));
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableValue<T>? a, ObservableValue<T>? b)
    {
        return (a == b) is false;
    }
    #endregion

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
