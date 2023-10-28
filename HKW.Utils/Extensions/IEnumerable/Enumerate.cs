using System.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 枚举出带有索引值的枚举值
    /// </summary>
    /// <param name="collection">集合</param>
    /// <returns>带有索引的枚举值 (Index, Item)</returns>
    public static IEnumerable<(int, object)> EnumerateIndex(this IEnumerable collection)
    {
        var index = 0;
        foreach (var item in collection)
            yield return (index++, item);
    }
}
