using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取一个随机索引的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection">集合</param>
    /// <returns>随机的一个值</returns>
    public static T Random<T>(this ICollection<T> collection)
    {
        var index = System.Random.Shared.Next(collection.Count);
        foreach (var item in collection)
            if (index-- == 0)
                return item;
        return default!;
    }

    /// <summary>
    /// 获取一个随机索引的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="random">随机</param>
    /// <returns>随机的一个值</returns>
    public static T Random<T>(this ICollection<T> collection, Random random)
    {
        var index = random.Next(collection.Count);
        foreach (var item in collection)
            if (index-- == 0)
                return item;
        return default!;
    }
}
