using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 倒序枚举出带有索引值的枚举值
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>带有索引的枚举值(索引, 枚举值)</returns>
    public static IEnumerable<(int Index, T Item)> ReverseEnumerateIndex<T>(
        this ICollection<T> source
    )
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var index = source.Count - 1;
        foreach (var item in source)
            yield return (index--, item);
    }

    /// <summary>
    /// 倒序枚举出带有索引值的枚举值
    /// </summary>
    /// <param name="source">源</param>
    /// <returns>带有索引的枚举值(索引, 枚举值)</returns>
    public static IEnumerable<(int Index, object Item)> ReverseEnumerateIndex(
        this ICollection source
    )
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var index = source.Count - 1;
        foreach (var item in source)
            yield return (index--, item);
    }
}
