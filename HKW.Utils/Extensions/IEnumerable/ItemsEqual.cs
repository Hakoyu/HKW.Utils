using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 判断两个集合的值是否全部相等 (无视顺序)
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="target">目标</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ItemsEqual<T>(this IEnumerable<T> source, IEnumerable<T> target)
    {
        return source.Except(target).Any() is false;
    }

    /// <summary>
    /// 判断两个集合的值是否全部相等 (无视顺序)
    /// </summary>
    /// <param name="source">集合1</param>
    /// <param name="target">集合2</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ItemsEqual(this IEnumerable source, IEnumerable target)
    {
        return source.Cast<object>().Except(target.Cast<object>()).Any() is false;
    }
}
