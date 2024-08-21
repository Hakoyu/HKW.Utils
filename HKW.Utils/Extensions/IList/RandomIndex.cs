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
    /// 获取列表中一个随机的索引
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="list">列表</param>
    /// <returns>随机的索引</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RandomIndex<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        return System.Random.Shared.Next(list.Count);
    }

    /// <summary>
    /// 获取列表中一个随机的索引
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="random">随机</param>
    /// <returns>随机的索引</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RandomIndex<T>(this IList<T> list, Random random)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        ArgumentNullException.ThrowIfNull(random, nameof(random));
        return random.Next(list.Count);
    }

    ///// <summary>
    ///// 获取列表中一个随机的值
    ///// </summary>
    ///// <param name="list">列表</param>
    ///// <returns>随机的一个值</returns>
    //public static object? Random(this IList list)
    //{
    //    ArgumentNullException.ThrowIfNull(list, nameof(list));
    //    return list[System.Random.Shared.Next(list.Count)];
    //}

    ///// <summary>
    ///// 获取列表中一个随机的值
    ///// </summary>
    ///// <param name="list">列表</param>
    ///// <param name="random">随机</param>
    ///// <returns>随机的一个值</returns>
    //public static object? Random(this IList list, Random random)
    //{
    //    ArgumentNullException.ThrowIfNull(list, nameof(list));
    //    return list[random.Next(list.Count)];
    //}
}
