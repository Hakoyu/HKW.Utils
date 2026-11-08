using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

public sealed partial class ObservablePoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从具体点值复制数值字段, 不装箱.
    /// </summary>
    public ObservablePoint(Point<T> point)
        : this(point.X, point.Y) { }

    /// <summary>
    /// 设置具体点值, 不装箱. 每个变化字段分别发出通知.
    /// </summary>
    public void SetValue(Point<T> point) => this.SetValue(point.X, point.Y);

    /// <summary>
    /// 按具体点值偏移, 不装箱. 先计算两轴再分别设置并通知.
    /// </summary>
    public void Offset(Point<T> point) => this.Offset(point.X, point.Y);

    /// <summary>
    /// 按尺寸分量增加并返回新值, 不装箱. 整数使用非受检查算术.
    /// </summary>
    public Point<T> Add(Size<T> size) => new(unchecked(X + size.Width), unchecked(Y + size.Height));

    /// <summary>
    /// 按尺寸分量减少并返回新值, 不装箱. 整数使用非受检查算术.
    /// </summary>
    public Point<T> Subtract(Size<T> size) =>
        new(unchecked(X - size.Width), unchecked(Y - size.Height));
}

public sealed partial class ObservableRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从具体范围值复制数值字段, 不装箱.
    /// </summary>
    public ObservableRange(Range<T> range)
        : this(range.Min, range.Max) { }

    /// <summary>
    /// 设置具体范围值, 不装箱. 每个变化字段分别发出通知.
    /// </summary>
    public void SetValue(Range<T> range) => this.SetValue(range.Min, range.Max);

    /// <summary>
    /// 检查具体范围是否包含在内, 不装箱. 反向或非有限范围返回 false.
    /// </summary>
    public bool Contains(Range<T> range, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsRange(Min, Max, range.Min, range.Max, includeBoundary);

    /// <summary>
    /// 按尺寸分量增加并返回新值, 不装箱. 整数使用非受检查算术.
    /// </summary>
    public Range<T> Add(Size<T> size) =>
        new(unchecked(Min + size.Width), unchecked(Max + size.Height));

    /// <summary>
    /// 按尺寸分量减少并返回新值, 不装箱. 整数使用非受检查算术.
    /// </summary>
    public Range<T> Subtract(Size<T> size) =>
        new(unchecked(Min - size.Width), unchecked(Max - size.Height));
}

public sealed partial class ObservableSize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从具体尺寸值复制数值字段, 不装箱.
    /// </summary>
    public ObservableSize(Size<T> size)
        : this(size.Width, size.Height) { }

    /// <summary>
    /// 设置具体尺寸值, 不装箱. 每个变化字段分别发出通知.
    /// </summary>
    public void SetValue(Size<T> size) => this.SetValue(size.Width, size.Height);

    /// <summary>
    /// 检查具体点是否包含在内, 不装箱. 无效几何返回 false.
    /// </summary>
    public bool Contains(Point<T> point, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsPoint(
            T.Zero,
            T.Zero,
            Width,
            Height,
            point.X,
            point.Y,
            includeBoundary
        );

    /// <summary>
    /// 按宽高比较尺寸向量, 不装箱. 共同原点不作为严格边界排除.
    /// </summary>
    public bool Contains(Size<T> size, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsPoint(
            T.Zero,
            T.Zero,
            Width,
            Height,
            size.Width,
            size.Height,
            includeBoundary
        );

    /// <summary>
    /// 检查具体矩形是否包含在内, 不装箱. 无效几何返回 false.
    /// </summary>
    public bool Contains(Rectangle<T> rectangle, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsRectangle(
            T.Zero,
            T.Zero,
            Width,
            Height,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height,
            includeBoundary
        );
}

public sealed partial class ObservableRectangle<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从具体矩形值复制数值字段, 不装箱.
    /// </summary>
    public ObservableRectangle(Rectangle<T> rectangle)
        : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

    /// <summary>
    /// 从具体位置和尺寸创建矩形, 不装箱.
    /// </summary>
    public ObservableRectangle(Point<T> point, Size<T> size)
        : this(point.X, point.Y, size.Width, size.Height) { }

    /// <summary>
    /// 设置具体矩形值, 不装箱. 每个变化字段分别发出通知.
    /// </summary>
    public void SetValue(Rectangle<T> rectangle) =>
        this.SetValue(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

    /// <summary>
    /// 设置具体位置和尺寸, 不装箱. 每个变化字段分别发出通知.
    /// </summary>
    public void SetValue(Point<T> point, Size<T> size) =>
        this.SetValue(point.X, point.Y, size.Width, size.Height);

    /// <summary>
    /// 检查具体点是否包含在内, 不装箱. 无效几何及端点溢出返回 false.
    /// </summary>
    public bool Contains(Point<T> point, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsPoint(X, Y, Width, Height, point.X, point.Y, includeBoundary);

    /// <summary>
    /// 检查具体矩形是否包含在内, 不装箱. 无效几何及端点溢出返回 false.
    /// </summary>
    public bool Contains(Rectangle<T> rectangle, bool includeBoundary = false) =>
        GeometryMath<T>.ContainsRectangle(
            X,
            Y,
            Width,
            Height,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height,
            includeBoundary
        );
}
