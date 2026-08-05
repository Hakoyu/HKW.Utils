using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DynamicData;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class ListExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    extension<T>(IList<T> list)
    {
        #region Find
        /// <summary>
        /// 按条件寻找项目
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? Find<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i], arg))
                    return list[i];
            }
            return default;
        }

        #region FindIndex
        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindIndex(0, list.Count, arg, match);
        }

        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex<TArg>(int startIndex, TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindIndex(startIndex, list.Count - startIndex, arg, match);
        }

        /// <summary>
        /// 按条件寻找索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex<TArg>(int startIndex, int count, TArg arg, Func<T, TArg, bool> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindIndexCheck(startIndex, count, list.Count);
            if (endIndex == -1)
                return -1;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(list[i], arg))
                    return i;
            }
            return -1;
        }
        #endregion
        #region FindPair
        /// <summary>
        /// 按条件寻找索引项目对
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Index, T? Value) FindPair<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindPair(0, list.Count, arg, match);
        }

        /// <summary>
        /// 按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Index, T? Value) FindPair<TArg>(
            int startIndex,
            TArg arg,
            Func<T, TArg, bool> match
        )
        {
            return list.FindPair(startIndex, list.Count - startIndex, arg, match);
        }

        /// <summary>
        /// 按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindPair<TArg>(
            int startIndex,
            int count,
            TArg arg,
            Func<T, TArg, bool> match
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindIndexCheck(startIndex, count, list.Count);
            if (endIndex == -1)
                return (-1, default);

            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(list[i], arg))
                    return (i, list[i]);
            }
            return (-1, default);
        }
        #endregion

        /// <summary>
        /// 从后往前按条件寻找项目
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? FindLast<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i], arg))
                    return list[i];
            }
            return default;
        }

        #region FindLastIndex
        /// <summary>
        /// 按条件从后往前寻找项目的索引
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindLastIndex(list.Count - 1, list.Count, arg, match);
        }

        /// <summary>
        /// 按条件从后往前寻找项目的索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex<TArg>(int startIndex, TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindLastIndex(startIndex, startIndex + 1, arg, match);
        }

        /// <summary>
        /// 按条件从后往前寻找项目的索引
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的项目的索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex<TArg>(
            int startIndex,
            int count,
            TArg arg,
            Func<T, TArg, bool> match
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindLastIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(list[i], arg))
                    return i;
            }
            return -1;
        }
        #endregion

        #region FindLastPair
        /// <summary>
        /// 按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindLastPair<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            return list.FindLastPair(list.Count - 1, list.Count, arg, match);
        }

        /// <summary>
        /// 按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindLastPair<TArg>(
            int startIndex,
            TArg arg,
            Func<T, TArg, bool> match
        )
        {
            return list.FindLastPair(startIndex, startIndex + 1, arg, match);
        }

        /// <summary>
        /// 按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <returns>第一个找到的索引项目对</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 或 <paramref name="count"/> 错误</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int index, T? value) FindLastPair<TArg>(
            int startIndex,
            int count,
            TArg arg,
            Func<T, TArg, bool> match
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var endIndex = ListFindLastIndexCheck(startIndex, count, list.Count);
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(list[i], arg))
                    return (i, list[i]);
            }
            return (-1, default);
        }
        #endregion

        #region TryFind
        /// <summary>
        /// 尝试按条件寻找项目的索引
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="item">项目</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFind<TArg>(
            TArg arg,
            Func<T, TArg, bool> match,
            [MaybeNullWhen(false)] out T item
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var index = list.FindIndex(arg, match);
            item = list.GetValueOrDefault(index);
            return index != -1;
        }

        /// <summary>
        /// 尝试按条件寻找索引项目对
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPair<TArg>(
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            return list.TryFindPair(0, list.Count, arg, match, out indexItemPair);
        }

        /// <summary>
        /// 尝试按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPair<TArg>(
            int startIndex,
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            return list.TryFindPair(
                startIndex,
                list.Count - startIndex,
                arg,
                match,
                out indexItemPair
            );
        }

        /// <summary>
        /// 尝试按条件寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPair<TArg>(
            int startIndex,
            int count,
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var index = list.FindIndex(startIndex, count, arg, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index != -1;
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLast<TArg>(
            TArg arg,
            Func<T, TArg, bool> match,
            [MaybeNullWhen(false)] out T indexItemPair
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var index = list.FindLastIndex(arg, match);
            indexItemPair = list.GetValueOrDefault(index);
            return index != -1;
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLastPair<TArg>(
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            return list.TryFindLastPair(list.Count - 1, list.Count, arg, match, out indexItemPair);
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLastPair<TArg>(
            int startIndex,
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            return list.TryFindLastPair(startIndex, startIndex + 1, arg, match, out indexItemPair);
        }

        /// <summary>
        /// 尝试按条件从后往前寻找索引项目对
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        /// <param name="indexItemPair">索引项目对</param>
        /// <returns>是否找到项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLastPair<TArg>(
            int startIndex,
            int count,
            TArg arg,
            Func<T, TArg, bool> match,
            out (int Index, T Value) indexItemPair
        )
        {
            ArgumentNullException.ThrowIfNull(match);
            var index = list.FindLastIndex(startIndex, count, arg, match);
            indexItemPair = (index, list.GetValueOrDefault(index)!);
            return index != -1;
        }
        #endregion
        #endregion

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
        /// <returns>是否成功</returns>
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
        /// <returns>是否成功</returns>
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
            ArgumentNullException.ThrowIfNull(items);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, list.Count);
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
        /// <param name="arg">参数</param>
        /// <param name="match">条件</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAll<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            if (list is List<T> baseList)
            {
                baseList.RemoveAll(x => match(x, arg));
            }
            else
            {
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (match(list[i], arg))
                        list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 反转列表
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReverseSelf()
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
            ListFindLastIndexCheck(startIndex, count, list.Count);
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
        /// <exception cref="ArgumentException"><paramref name="list"/> 为空</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RandomItem(Random? random = null)
        {
            ArgumentException.ThrowIfEmptyCollection(list);
            random ??= System.Random.Shared;
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// 获取列表中一个随机的索引
        /// </summary>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>随机的索引</returns>
        /// <exception cref="ArgumentException"><paramref name="list"/> 为空</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RandomIndex(Random? random = null)
        {
            ArgumentException.ThrowIfEmptyCollection(list);
            random ??= System.Random.Shared;
            return random.Next(list.Count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ListFindIndexCheck(int startIndex, int count, int listCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, listCount);
        int endIndex = startIndex + count;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endIndex, listCount);
        return endIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ListFindLastIndexCheck(int startIndex, int count, int listCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, listCount);
        int endIndex = startIndex - count;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endIndex, listCount);
        return endIndex;
    }
}
//public List<string> Strs { get; set; } =
//    Enumerable.Range(0, 1_000_000).Select(x => x.ToString()).ToList();

//public string data1 = "999999";
//public string data2 = "888888";
//public string data3 = "777777";

//[Benchmark]
//public object? Test1()
//{
//    return ListExtensions.FindIndex(
//        Strs,
//        0,
//        Strs.Count,
//        x => x == data1 || x == data2 || x == data3
//    );
//}

//[Benchmark]
//public object? Test2()
//{
//    return ListExtensions.FindIndex(
//        Strs,
//        0,
//        Strs.Count,
//        (data1, data2, data3),
//        static (x, d) => x == d.data1 || x == d.data2 || x == d.data3
//    );
//}

//[Benchmark]
//public object? Test3()
//{
//    for (int i = 0; i < Strs.Count; i++)
//    {
//        var str = Strs[i];
//        if (str == data1 || str == data2 || str == data3)
//            return i;
//    }
//    return -1;
//}
//| Method | Mean     | Error     | StdDev    | Allocated |
//|------- |---------:|----------:|----------:|----------:|
//| Test1  | 5.023 ms | 0.0998 ms | 0.1694 ms |      24 B |
//| Test2  | 4.319 ms | 0.0863 ms | 0.1152 ms |      24 B |
//| Test3  | 4.428 ms | 0.0876 ms | 0.1729 ms |      24 B |
