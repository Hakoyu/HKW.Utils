namespace HKW.HKWUtils.Extensions;

/// <summary>
/// 值相等
/// </summary>
public static partial class HKWExtensions
{
    /// <summary>
    /// 更好的序列相等
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="first">第一个枚举</param>
    /// <param name="second">第二个枚举</param>
    /// <param name="comparer">比较器</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    public static bool EqualOnComparer<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        Func<T, T, bool> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        using var firstEnumerator = first.GetEnumerator();
        using var secondEnumerator = second.GetEnumerator();
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
