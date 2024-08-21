using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 随机排序
    /// </summary>
    /// <typeparam name="TSource">值类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>随机后的枚举值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<TSource> RandomOrder<TSource>(this IEnumerable<TSource> source)
    {
        var count = source.Count();
        return source.OrderBy(x => System.Random.Shared.Next(count));
    }

    /// <summary>
    /// 随机排序
    /// </summary>
    /// <typeparam name="TSource">值类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="random">随机值</param>
    /// <returns>随机的一个值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<TSource> RandomOrder<TSource>(
        this IEnumerable<TSource> source,
        Random random
    )
    {
        ArgumentNullException.ThrowIfNull(random, nameof(random));
        var count = source.Count();
        return source.OrderBy(x => random.Next(count));
    }
}
