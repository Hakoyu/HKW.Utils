using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// 值相等
/// </summary>
public static partial class HKWExtensions
{
    /// <summary>
    /// 可使用匿名方法作为比较器的序列相等
    /// </summary>
    /// <typeparam name="TSource">值类型</typeparam>
    /// <param name="first">第一个集合</param>
    /// <param name="second">第二个集合</param>
    /// <param name="comparer">比较器</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TSource, bool> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        ArgumentNullException.ThrowIfNull(comparer, nameof(comparer));
        var c = EqualityComparer<TSource>.Create(comparer!);
        return first.SequenceEqual(second, c);
    }

    /// <summary>
    /// 可使用匿名方法作为比较器的序列相等
    /// </summary>
    /// <param name="first">第一个集合</param>
    /// <param name="second">第二个集合</param>
    /// <param name="comparer">比较器</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual(
        this IEnumerable first,
        IEnumerable second,
        Func<object, object, bool> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        ArgumentNullException.ThrowIfNull(comparer, nameof(comparer));
        var firstEnumerator = first.GetEnumerator();
        var secondEnumerator = second.GetEnumerator();
        while (firstEnumerator.MoveNext())
        {
            if (
                (
                    secondEnumerator.MoveNext()
                    && comparer(firstEnumerator.Current, secondEnumerator.Current)
                )
                is false
            )
                return false;
        }
        return secondEnumerator.MoveNext() is false;
    }
}
