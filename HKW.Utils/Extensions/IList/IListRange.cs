using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 范围添加
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="collection">集合</param>
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
    {
        if (list is List<T> baseList)
        {
            baseList.AddRange(collection);
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.AddRange(listRange);
        }
        else
        {
            foreach (var item in collection)
                list.Add(item);
        }
    }

    /// <summary>
    /// 范围插入
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">索引</param>
    /// <param name="collection">集合</param>
    public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection)
    {
        if (index <= 0)
            throw new IndexOutOfRangeException("Non-negative number required.");
        if (index > list.Count)
            throw new IndexOutOfRangeException(" Index must be within the bounds of the List.");
        if (list is List<T> baseList)
        {
            baseList.InsertRange(index, collection);
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.InsertRange(index, listRange);
        }
        else
        {
            foreach (var item in collection)
                list.Insert(index++, item);
        }
    }

    /// <summary>
    /// 删除范围
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">起始索引</param>
    /// <param name="count">数量</param>
    public static void RemoveRange<T>(this IList<T> list, int index, int count)
    {
        if (index < 0)
            throw new IndexOutOfRangeException("Non-negative number required.");
        if (index > list.Count)
            throw new IndexOutOfRangeException(" Index must be within the bounds of the List.");
        if (count < 0)
            throw new IndexOutOfRangeException("Non-negative number required.");
        if (count + index > list.Count)
            throw new IndexOutOfRangeException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."
            );
        if (count == 0)
            return;
        if (list is List<T> baseList)
        {
            baseList.RemoveRange(index, count);
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.RemoveRange(index, count);
        }
        else
        {
            for (var i = index + count; i >= index; i--)
                list.RemoveAt(i);
        }
    }

    /// <summary>
    /// 删除全部符合条件的项目
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="match">条件</param>
    public static void RemoveAll<T>(this IList<T> list, Predicate<T> match)
    {
        if (list is List<T> baseList)
        {
            baseList.RemoveAll(match);
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.RemoveAll(match);
        }
        else
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                    list.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 反转列表
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    public static void Reverse<T>(this IList<T> list)
    {
        if (list is List<T> baseList)
        {
            baseList.Reverse();
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.Reverse();
        }
        else
        {
            var count = list.Count / 2;
            for (int i = 0, j = list.Count - 1; i < count; i++, j--)
            {
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }

    /// <summary>
    /// 反转列表
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">起始索引</param>
    /// <param name="count">数量</param>
    public static void Reverse<T>(this IList<T> list, int index, int count)
    {
        if (index <= 0)
            throw new IndexOutOfRangeException("Non-negative number required.");
        if (index > list.Count)
            throw new IndexOutOfRangeException(" Index must be within the bounds of the List.");
        if (count <= 0)
            throw new IndexOutOfRangeException("Non-negative number required.");
        if (count + index > list.Count)
            throw new IndexOutOfRangeException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."
            );
        if (count == 1)
            return;
        if (list is List<T> baseList)
        {
            baseList.Reverse(index, count);
        }
        else if (list is IListRange<T> listRange)
        {
            listRange.Reverse(index, count);
        }
        else
        {
            var newCount = (index + count) / 2;
            for (int i = index, j = index + count - 1; i < newCount; i++, j--)
            {
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
