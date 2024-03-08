using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public static T? Find<T>(this IList<T> list, Predicate<T> match)
    {
        ArgumentNullException.ThrowIfNull(nameof(list));
        ArgumentNullException.ThrowIfNull(nameof(match));
        for (int i = 0; i < list.Count; i++)
        {
            if (match(list[i]))
                return list[i];
        }
        return default;
    }

    /// <summary>
    /// 按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始索引</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public static (int Index, T? Value) Find<T>(
        this IList<T> list,
        int startIndex,
        Predicate<T> match
    )
    {
        return Find(list, startIndex, list.Count - startIndex, match);
    }

    /// <summary>
    /// 按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始索引</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public static (int index, T? value) Find<T>(
        this IList<T> list,
        int startIndex,
        int count,
        Predicate<T> match
    )
    {
        ArgumentNullException.ThrowIfNull(nameof(list));
        ArgumentNullException.ThrowIfNull(nameof(match));
        if (startIndex <= 0)
            throw new IndexOutOfRangeException(
                $"{nameof(startIndex).ToPascal()}Non-negative number required."
            );
        if (startIndex > list.Count)
            throw new IndexOutOfRangeException(
                $"{nameof(startIndex).ToPascal()} must be within the bounds of the List."
            );
        if (count <= 0)
            throw new IndexOutOfRangeException(
                $"{nameof(count).ToPascal()} non-negative number required."
            );
        int endIndex = startIndex + count;
        if (endIndex > list.Count)
            throw new IndexOutOfRangeException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."
            );
        for (int i = startIndex; i < endIndex; i++)
        {
            if (match(list[i]))
                return (i, list[i]);
        }
        return (-1, default);
    }
}
