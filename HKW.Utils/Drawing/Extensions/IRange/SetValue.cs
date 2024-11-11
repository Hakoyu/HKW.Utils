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
    /// <param name="sourceRange">源范围</param>
    /// <param name="range">范围</param>
    public static void SetValue<T>(this IRange<T> sourceRange, IReadOnlyRange<T> range)
        where T : struct, INumber<T>
    {
        sourceRange.Min = range.Min;
        sourceRange.Max = range.Max;
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="sourceRange">源范围</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public static void SetValue<T>(this IRange<T> sourceRange, T min, T max)
        where T : struct, INumber<T>
    {
        sourceRange.Min = min;
        sourceRange.Max = max;
    }
}
