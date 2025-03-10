﻿using System.Numerics;

namespace HKW.HKWUtils.Drawing;

public static partial class DrawingExtensions
{
    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourceSize">源大小</param>
    /// <param name="size">大小</param>
    public static void SetValue<T>(this ISize<T> sourceSize, IReadOnlySize<T> size)
        where T : struct, INumber<T>
    {
        sourceSize.Width = size.Width;
        sourceSize.Height = size.Height;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourceSize">源大小</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public static void SetValue<T>(this ISize<T> sourceSize, T width, T height)
        where T : struct, INumber<T>
    {
        sourceSize.Width = width;
        sourceSize.Height = height;
    }
}
