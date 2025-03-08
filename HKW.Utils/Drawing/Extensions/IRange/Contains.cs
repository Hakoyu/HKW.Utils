using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public static partial class DrawingExtensions
{
    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="range">范围</param>
    /// <param name="otherRange">其它范围</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlyRange<T> range, IReadOnlyRange<T> otherRange)
        where T : struct, INumber<T>
    {
        if (otherRange.Max > range.Max)
            return false;
        if (otherRange.Min > range.Min)
            return false;
        return true;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="range">范围</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlyRange<T> range, T min, T max)
        where T : struct, INumber<T>
    {
        if (max > range.Max || min < range.Min)
            return false;
        return true;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="range">范围</param>
    /// <param name="value">值</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlyRange<T> range, T value)
        where T : struct, INumber<T>
    {
        return range.Max > value && range.Min < value;
    }
}
