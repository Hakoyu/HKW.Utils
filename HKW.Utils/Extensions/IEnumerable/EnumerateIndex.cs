using System.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 枚举出带有索引值的枚举值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>带有索引的枚举值 (Index, Item)</returns>
    public static IEnumerable<(int, T)> EnumerateIndex<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var index = 0;
        foreach (var item in source)
            yield return (index++, item);
    }

    /// <summary>
    /// 枚举出带有索引值的枚举值
    /// </summary>
    /// <param name="source">源</param>
    /// <returns>带有索引的枚举值 (Index, Item)</returns>
    public static IEnumerable<(int, object)> EnumerateIndex(this IEnumerable source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var index = 0;
        foreach (var item in source)
            yield return (index++, item);
    }
}
