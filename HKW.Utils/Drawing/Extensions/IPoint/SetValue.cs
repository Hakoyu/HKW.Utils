using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Drawing;

public static partial class DrawingExtensions
{
    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourcePoint">源点</param>
    /// <param name="point">点</param>
    public static void SetValue<T>(this IPoint<T> sourcePoint, IReadOnlyPoint<T> point)
        where T : struct, INumber<T>
    {
        sourcePoint.X = point.X;
        sourcePoint.Y = point.Y;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourcePoint">源点</param>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public static void SetValue<T>(this IPoint<T> sourcePoint, T x, T y)
        where T : struct, INumber<T>
    {
        sourcePoint.X = x;
        sourcePoint.Y = y;
    }
}
