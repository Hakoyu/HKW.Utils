using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public partial struct Range<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 从同类型值复制, 不装箱.
    /// </summary>
    public Range(Range<T> other)
        : this(other.Min, other.Max) { }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherRange">其他范围</param>
    /// <exception cref="ArgumentNullException"><paramref name="otherRange"/> 为 null</exception>
    public void SetValue(IReadOnlyRange<T> otherRange)
    {
        ArgumentNullException.ThrowIfNull(otherRange);
        SetValue(otherRange.Min, otherRange.Max);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public void SetValue(T min, T max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="otherRange">其他范围</param>
    public void SetValue(Range<T> otherRange)
    {
        SetValue(otherRange.Min, otherRange.Max);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="otherRange">其它范围</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    /// <exception cref="ArgumentNullException"><paramref name="otherRange"/> 为 null</exception>
    public readonly bool Contains(IReadOnlyRange<T> otherRange, bool includeBoundary = false)
    {
        ArgumentNullException.ThrowIfNull(otherRange);

        return Contains(otherRange.Min, otherRange.Max, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="otherRange">其它范围</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(Range<T> otherRange, bool includeBoundary = false)
    {
        return Contains(otherRange.Min, otherRange.Max, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="includeBoundary">允许边界相等</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(T min, T max, bool includeBoundary = false)
    {
        return GeometryMath<T>.ContainsRange(Min, Max, min, max, includeBoundary);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>是否包含</returns>
    public readonly bool Contains(T value)
    {
        return GeometryMath<T>.ContainsRange(Min, Max, value, value, false);
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Range<T> Add(Size<T> size)
    {
        return new(unchecked(Min + size.Width), unchecked(Max + size.Height));
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Range<T> Add(IReadOnlySize<T> size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return new(unchecked(Min + size.Width), unchecked(Max + size.Height));
    }

    /// <summary>
    /// 按尺寸分量减少并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Range<T> Subtract(Size<T> size)
    {
        return new(unchecked(Min - size.Width), unchecked(Max - size.Height));
    }

    /// <summary>
    /// 按尺寸分量减少并返回新值; 整数使用非受检查算术.
    /// </summary>
    public readonly Range<T> Subtract(IReadOnlySize<T> size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return new(unchecked(Min - size.Width), unchecked(Max - size.Height));
    }

    /// <summary>
    /// 按尺寸分量增加并返回新值, 不装箱.
    /// </summary>
    public static Range<T> operator +(Range<T> value, Size<T> size) => value.Add(size);

    /// <summary>
    /// 按尺寸分量减少并返回新值, 不装箱.
    /// </summary>
    public static Range<T> operator -(Range<T> value, Size<T> size) => value.Subtract(size);
}
