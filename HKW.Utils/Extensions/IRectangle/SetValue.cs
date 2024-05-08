using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourceRectangle">源矩形</param>
    /// <param name="rectangle">矩形</param>
    public static void SetValue<T>(
        this IRectangle<T> sourceRectangle,
        IReadOnlyRectangle<T> rectangle
    )
        where T : struct, INumber<T>
    {
        sourceRectangle.X = rectangle.X;
        sourceRectangle.Y = rectangle.Y;
        sourceRectangle.Width = rectangle.Width;
        sourceRectangle.Height = rectangle.Height;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值立类型</typeparam>
    /// <param name="sourceRectangle">源矩形</param>
    /// <param name="size">大小</param>
    /// <param name="location">位置</param>
    public static void SetValue<T>(
        this IRectangle<T> sourceRectangle,
        IReadOnlySize<T> size,
        IReadOnlyPoint<T> location
    )
        where T : struct, INumber<T>
    {
        sourceRectangle.X = location.X;
        sourceRectangle.Y = location.Y;
        sourceRectangle.Width = size.Width;
        sourceRectangle.Height = size.Height;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourceRectangle">源矩形</param>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public static void SetValue<T>(this IRectangle<T> sourceRectangle, T x, T y, T width, T height)
        where T : struct, INumber<T>
    {
        sourceRectangle.X = x;
        sourceRectangle.Y = y;
        sourceRectangle.Width = width;
        sourceRectangle.Height = height;
    }
}
