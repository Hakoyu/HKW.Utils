using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 使用索引值获取项目
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="index">索引值</param>
    /// <returns>项目</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出源长度</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetItemByIndex<T>(this IEnumerable<T> source, int index)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source is IList<T> list)
            return list[index];
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(index));
        var tempIndex = 0;
        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (tempIndex++ == index)
                return enumerator.Current;
        }
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
}
