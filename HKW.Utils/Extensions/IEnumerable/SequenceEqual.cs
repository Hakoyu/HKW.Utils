using System.Collections;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// 值相等
/// </summary>
public static partial class HKWExtensions
{
    /// <summary>
    /// 可使用匿名方法作为比较器的序列相等
    /// </summary>
    /// <param name="first">第一个集合</param>
    /// <param name="second">第二个集合</param>
    /// <param name="comparer">比较器</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    public static bool SequenceEqual(
        this IEnumerable first,
        IEnumerable second,
        Func<object, object, bool> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(second, nameof(second));
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
