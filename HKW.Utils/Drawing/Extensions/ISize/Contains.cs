using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public static partial class DrawingExtensions
{
    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="size">大小</param>
    /// <param name="otherSize">其它大小</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlySize<T> size, IReadOnlySize<T> otherSize)
        where T : struct, INumber<T>
    {
        if (otherSize.Width > size.Width)
            return false;
        if (otherSize.Height > size.Height)
            return false;
        return true;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="size">大小</param>
    /// <param name="pt">点</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlySize<T> size, IReadOnlyPoint<T> pt)
        where T : struct, INumber<T>
    {
        return Contains(size, pt.X, pt.Y);
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="size">大小</param>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlySize<T> size, T x, T y)
        where T : struct, INumber<T>
    {
        if (x > size.Width || y > size.Height)
            return false;
        return true;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="size">大小</param>
    /// <param name="rectangle">矩形</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlySize<T> size, IReadOnlyRectangle<T> rectangle)
        where T : struct, INumber<T>
    {
        if (Contains(size, rectangle.X, rectangle.Y) is false)
            return false;
        if (rectangle.RightBottom.X > size.Width)
            return false;
        if (rectangle.RightBottom.Y > size.Height)
            return false;
        return true;
    }
}
