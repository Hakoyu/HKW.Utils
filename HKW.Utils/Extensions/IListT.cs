using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DynamicData;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    extension<T>(IList<T> list)
    {
        /// <summary>
        /// 按条件寻找项目和索引
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? Find(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
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
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Index, T? Value) FindPair(int startIndex, Predicate<T> match)
        {
            return FindPair(list, startIndex, list.Count - startIndex, match);
        }

        /// <summary>
        /// 按条件寻找项目和索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindPair(int startIndex, int count, Predicate<T> match)
        {
            if (count == 0)
                return (-1, default);
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(list[i]))
                    return (i, list[i]);
            }
            return (-1, default);
        }

        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(list, startIndex, list.Count - startIndex, match);
        }

        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if (count == 0)
                return -1;
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? FindLast(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                    return list[i];
            }
            return default;
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindLastPair(int startIndex, Predicate<T> match)
        {
            return FindLastPair(list, startIndex, list.Count - (list.Count - startIndex), match);
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindLastPair(int startIndex, int count, Predicate<T> match)
        {
            if (count == 0)
                return (-1, default);
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindLastIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i >= endIndex; i--)
            {
                if (match(list[i]))
                    return (i, list[i]);
            }
            return (-1, default);
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(list, startIndex, list.Count - (list.Count - startIndex), match);
        }

        /// <summary>
        /// 按条件从后往前寻找项目和索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目和索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (count == 0)
                return -1;
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindLastIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i >= endIndex; i--)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 尝试按条件寻找项目和索引
        /// </summary>
        /// <param name="match">条件</param>
        /// <param name="item">项目</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFind(Predicate<T> match, [MaybeNullWhen(false)] out T item)
        {
            var index = list.FindIndex(match);
            item = list.GetValueOrDefault(index);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 尝试按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPair(
            int startIndex,
            Predicate<T> match,
            out (int Index, T Value) indexItemPair
        )
        {
            var index = list.FindIndex(startIndex, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 尝试按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">索引</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPair(
            int startIndex,
            int count,
            Predicate<T> match,
            out (int Index, T Value) indexItemPair
        )
        {
            var index = list.FindIndex(startIndex, count, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLast(Predicate<T> match, [MaybeNullWhen(false)] out T indexItemPair)
        {
            var index = list.FindLastIndex(match);
            indexItemPair = list.GetValueOrDefault(index);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLast(
            int startIndex,
            Predicate<T> match,
            out (int Index, T Value) indexItemPair
        )
        {
            var index = list.FindLastIndex(startIndex, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLast(
            int startIndex,
            int count,
            Predicate<T> match,
            out (int Index, T Value) indexItemPair
        )
        {
            var index = list.FindLastIndex(startIndex, count, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index == -1 ? false : true;
        }

        /// <summary>
        /// 使用索引获取列表的值或默认值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取成功则返回值, 获取失败则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? GetValueOrDefault(int index, T? defaultValue = default)
        {
            if (list.IsValidIndex(index))
                return list[index];
            return defaultValue;
        }

        /// <summary>
        /// 尝试使用索引获取列表的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">项目</param>
        /// <returns>成功获取值为 <see langword="true"/> 失败为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int index, [MaybeNullWhen(false)] out T value)
        {
            if (list.IsValidIndex(index))
            {
                value = list[index];
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }

        /// <summary>
        /// 尝试使用索引获取列表的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">项目</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>成功获取值为 <see langword="true"/> 失败为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValueOrDefault(
            int index,
            [MaybeNullWhen(false)] out T value,
            T? defaultValue = default
        )
        {
            if (list.IsValidIndex(index))
            {
                value = list[index];
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// 范围插入
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="items">项目们</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(int index, IEnumerable<T> items)
        {
            ListIndexCheck(index, list.Count);
            if (list is List<T> baseList)
            {
                baseList.InsertRange(index, items);
            }
            else if (items is IList<T> itemList)
            {
                for (var i = 0; i < itemList.Count; i++)
                    list.Insert(index++, itemList[i]);
            }
            else
            {
                foreach (var item in items)
                    list.Insert(index++, item);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item">项目</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item, out int index)
        {
            index = list.IndexOf(item);
            if (index < 0)
                return false;
            list.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 删除范围
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int startIndex, int count)
        {
            if (count == 0)
                return;
            ListFindIndexCheck(startIndex, count, list.Count);
            if (list is List<T> baseList)
            {
                baseList.RemoveRange(startIndex, count);
            }
            else
            {
                for (var i = startIndex + count - 1; i >= startIndex; i--)
                    list.RemoveAt(i);
            }
        }

        /// <summary>
        /// 删除全部符合条件的项目
        /// </summary>
        /// <param name="match">条件</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAll(Predicate<T> match)
        {
            if (list is List<T> baseList)
            {
                baseList.RemoveAll(match);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse()
        {
            if (list is List<T> baseList)
            {
                baseList.Reverse();
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
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(int startIndex, int count)
        {
            if (count == 1)
                return;
            ListFindIndexCheck(startIndex, count, list.Count);
            if (list is List<T> baseList)
            {
                baseList.Reverse(startIndex, count);
            }
            else
            {
                var newCount = (startIndex + count) / 2;
                for (int i = startIndex, j = startIndex + count - 1; i < newCount; i++, j--)
                {
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }

        /// <summary>
        /// 获取列表中一个随机的值
        /// </summary>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>随机的一个值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Random(Random? random = null)
        {
            random ??= System.Random.Shared;
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// 获取列表中一个随机的索引
        /// </summary>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>随机的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RandomIndex(Random? random = null)
        {
            random ??= System.Random.Shared;
            return random.Next(list.Count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ListIndexCheck(int index, int listCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, listCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ListFindIndexCheck(int startIndex, int count, int listCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, listCount);
        int endIndex = startIndex + count;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endIndex, listCount);
        return endIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ListFindLastIndexCheck(int startIndex, int count, int listCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, listCount);
        int endIndex = startIndex - count;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endIndex, listCount);
        return endIndex;
    }
}
