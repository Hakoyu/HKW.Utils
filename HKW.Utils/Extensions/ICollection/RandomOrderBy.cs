using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 随机排序
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection">集合</param>
    /// <returns>随机后的枚举值</returns>
    public static IOrderedEnumerable<T> RandomOrder<T>(this ICollection<T> collection)
    {
        return collection.OrderBy(x => System.Random.Shared.Next(collection.Count));
    }

    /// <summary>
    /// 随机排序
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="random">随机值</param>
    /// <returns>随机的一个值</returns>
    public static IOrderedEnumerable<T> RandomOrder<T>(
        this ICollection<T> collection,
        Random random
    )
    {
        return collection.OrderBy(x => random.Next(collection.Count));
    }
}
