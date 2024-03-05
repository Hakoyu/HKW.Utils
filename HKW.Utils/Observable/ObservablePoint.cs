using System.Diagnostics;
using System.Numerics;
using System.Windows;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可克隆接口
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface ICloneable<T>
{
    /// <summary>
    /// 克隆当前对象
    /// </summary>
    /// <returns>新对象</returns>
    public T Clone();
}

/// <summary>
/// 可深克隆接口
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IDeepCloneable<T>
{
    /// <summary>
    /// 深克隆克隆当前对象
    /// </summary>
    /// <returns>新对象</returns>
    public T DeepClone();
}

/// <summary>
/// 可观察点
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ObservablePoint<T>
    : ViewModelBase<ObservablePoint<T>>,
        IEquatable<ObservablePoint<T>>,
        ICloneable<ObservablePoint<T>>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservablePoint() { }

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public ObservablePoint(T x, T y)
    {
        X = x;
        Y = y;
    }

    #region X
    private T _x = default!;

    /// <summary>
    /// 坐标X
    /// </summary>
    public T X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }
    #endregion

    #region Y
    private T _y = default!;

    /// <summary>
    /// 坐标Y
    /// </summary>
    public T Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
    #endregion


    /// <inheritdoc/>
    public ObservablePoint<T> Clone()
    {
        return new(X, Y);
    }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservablePoint<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservablePoint<T>? other)
    {
        if (other is null)
            return false;
        return EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    /// <inheritdoc/>
    public static bool operator ==(ObservablePoint<T> a, ObservablePoint<T> b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservablePoint<T> a, ObservablePoint<T> b)
    {
        return a.Equals(b) is not true;
    }
    #endregion
}
