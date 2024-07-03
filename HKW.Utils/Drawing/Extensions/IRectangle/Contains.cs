using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class DrawingExtensions
{
    /// <summary>
    /// 坐标在矩形内
    /// </summary>
    /// <param name="rectangle">矩形</param>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <returns>在矩形内为 <see langword="true"/> 不在为 <see langword="false"/></returns>
    public static bool Contains<T>(this IReadOnlyRectangle<T> rectangle, T x, T y)
        where T : struct, INumber<T>
    {
        if (x < rectangle.X || y < rectangle.Y)
            return false;
        if (x > rectangle.Right || y > rectangle.Bottom)
            return false;
        return true;
    }

    /// <summary>
    /// 矩形在矩形内
    /// </summary>
    /// <param name="rectangle">矩形</param>
    /// <param name="otherRectangle">其它矩形</param>
    /// <returns>在矩形内为 <see langword="true"/> 不在为 <see langword="false"/></returns>
    public static bool Contains<T>(
        this IReadOnlyRectangle<T> rectangle,
        IReadOnlyRectangle<T> otherRectangle
    )
        where T : struct, INumber<T>
    {
        if (otherRectangle.X < rectangle.X || otherRectangle.Y < rectangle.Y)
            return false;
        if (otherRectangle.Right > rectangle.Right || otherRectangle.Bottom > rectangle.Bottom)
            return false;
        return true;
    }
}
