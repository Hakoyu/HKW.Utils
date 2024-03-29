﻿namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 判断集合中是否含有值
    /// <para>等价于 <c>Count != 0</c></para>
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="collection">集合</param>
    /// <returns>含有值为 <see langword="true"/> 否则为 <see langword="false"/></returns>
    public static bool HasValue<T>(this ICollection<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        return collection.Count != 0;
    }
}
