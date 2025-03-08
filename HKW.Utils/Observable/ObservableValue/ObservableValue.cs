using System.Diagnostics;
using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测值
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{Value}")]
public partial class ObservableValue<T> : ReactiveObjectX, IEquatable<ObservableValue<T>>
{
    /// <summary>
    /// 值
    /// </summary>
    [ReactiveProperty]
    public T Value { get; set; }

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
        return obj is ObservableValue<T> value
            && EqualityComparer<T>.Default.Equals(Value, value.Value);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableValue<T>? other)
    {
        return other is ObservableValue<T> value
            && EqualityComparer<T>.Default.Equals(Value, value.Value);
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservableValue<T> a, ObservableValue<T> b)
    {
        return EqualityComparer<T>.Default.Equals(a.Value, b.Value);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableValue<T> a, ObservableValue<T> b)
    {
        return (a == b) is not true;
    }
    #endregion
}
