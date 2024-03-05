using System.Numerics;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察矩形位置
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ObservableRectangleLocation<T>
    : ViewModelBase<ObservableRectangleLocation<T>>,
        IEquatable<ObservableRectangleLocation<T>>,
        ICloneable<ObservableRectangleLocation<T>>
    where T : INumber<T>
{
    /// <inheritdoc/>
    public ObservableRectangleLocation() { }

    /// <inheritdoc/>
    /// <param name="rectangle">矩形</param>
    /// <param name="location">位置</param>
    public ObservableRectangleLocation(
        ObservableRectangle<T> rectangle,
        ObservablePoint<T> location
    )
    {
        Rectangle = rectangle;
        Location = location;
    }

    #region Rectangle
    private ObservableRectangle<T> _rectangle = default!;

    /// <summary>
    /// 矩形
    /// </summary>
    public ObservableRectangle<T> Rectangle
    {
        get => _rectangle;
        set
        {
            SetProperty(ref _rectangle, value);
            EndLocation = new(_location.X + Rectangle.Width, _location.Y + Rectangle.Height);
        }
    }
    #endregion

    #region Location
    private ObservablePoint<T> _location = new();

    /// <summary>
    /// 位置
    /// </summary>
    public ObservablePoint<T> Location
    {
        get => _location;
        set
        {
            SetProperty(ref _location, value);
            EndLocation = new(_location.X + Rectangle.Width, _location.Y + Rectangle.Height);
        }
    }
    #endregion

    #region EndLocation
    private ObservablePoint<T> _endLocation = default!;

    /// <summary>
    /// 结束位置
    /// </summary>
    public ObservablePoint<T> EndLocation
    {
        get => _endLocation;
        private set => SetProperty(ref _endLocation, value);
    }
    #endregion

    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <returns>在矩形内为 <see langword="true"/> 不在为 <see langword="false"/></returns>
    public bool InRectangle(T x, T y)
    {
        if (x < Location.X || y < Location.Y)
            return false;
        if (x > EndLocation.X || y > EndLocation.Y)
            return false;
        return true;
    }

    /// <summary>
    /// 复制一个新的对象
    /// </summary>
    /// <returns>新对象</returns>
    public ObservableRectangleLocation<T> Clone()
    {
        return new(Rectangle.Clone(), Location.Clone());
    }

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Rectangle, Location);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableRectangleLocation<T>);
    }

    /// <inheritdoc/>
    public bool Equals(ObservableRectangleLocation<T>? other)
    {
        if (other is null)
            return false;
        return Rectangle.Equals(other.Rectangle) && Location.Equals(other.Location);
    }

    /// <inheritdoc/>
    public static bool operator ==(
        ObservableRectangleLocation<T> a,
        ObservableRectangleLocation<T> b
    )
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(
        ObservableRectangleLocation<T> a,
        ObservableRectangleLocation<T> b
    )
    {
        return a.Equals(b) is not true;
    }
    #endregion
}
