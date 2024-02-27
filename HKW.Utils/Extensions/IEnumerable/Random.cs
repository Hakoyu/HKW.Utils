using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取枚举中一个随机的值
    /// </summary>
    /// <typeparam name="TSource">值类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>随机的一个值</returns>
    public static TSource Random<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return source.ElementAt(System.Random.Shared.Next(source.Count()));
    }

    /// <summary>
    /// 获取枚举中一个随机的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="random">随机</param>
    /// <returns>随机的一个值</returns>
    public static T Random<T>(this IEnumerable<T> source, Random random)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(random, nameof(random));
        return source.ElementAt(random.Next(source.Count()));
    }
}
