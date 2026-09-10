using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class CollectionExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="collection">集合</param>
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// 有效的索引值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>是否有效</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < collection.Count;
        }

        /// <summary>
        /// 有值
        /// <para>等价于 <c>Count > 0</c></para>
        /// </summary>
        /// <returns>是否有值</returns>
        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => collection.Count > 0;
        }

        /// <summary>
        /// 范围添加
        /// </summary>
        /// <param name="items">项目</param>
        public void AddRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (collection is List<T> list)
            {
                list.AddRange(items);
            }
            else if (items is IList<T> iList)
            {
                for (var i = 0; i < iList.Count; i++)
                    collection.Add(iList[i]);
            }
            else
            {
                foreach (var item in items)
                    collection.Add(item);
            }
        }

        /// <summary>
        /// 倒序枚举出带有索引值的枚举值
        /// </summary>
        /// <returns>带有索引的枚举值(枚举值, 索引)</returns>
        public IEnumerable<(T Item, int Index)> ReverseWithIndex()
        {
            if (collection is List<T> list)
            {
                for (var i = list.Count - 1; i >= 0; i--)
                    yield return (list[i], i);
            }
            else
            {
                var index = collection.Count - 1;
                foreach (var item in collection.Reverse())
                    yield return (item, index--);
            }
        }

        /// <summary>
        /// 从后往前获取项目的索引
        /// </summary>
        /// <param name="item">项目</param>
        /// <returns>第一个找到的项目</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T item)
        {
            if (collection is IList<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (EqualityComparer<T>.Default.Equals(item, list[i]))
                        return i;
                }
            }
            else
            {
                var i = collection.Count;
                foreach (var e in collection.Reverse())
                {
                    i--;
                    if (EqualityComparer<T>.Default.Equals(item, e))
                        return i;
                }
            }
            collection.ReverseWithIndex();
            return -1;
        }

        /// <summary>
        /// 删除所有对象
        /// </summary>
        /// <param name="items">对象</param>
        public void RemoveAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is IList<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                    collection.Remove(list[i]);
            }
            else
            {
                foreach (var item in collection.Reverse())
                    collection.Remove(item);
            }
        }
    }

    ///// <param name="collection">集合</param>
    //extension(ICollection collection)
    //{
    //    /// <summary>
    //    /// 倒序枚举出带有索引值的枚举值
    //    /// </summary>
    //    /// <returns>带有索引的枚举值(枚举值, 索引)</returns>
    //    public IEnumerable<(object Item, int Index)> ReverseWithIndex()
    //    {
    //        var index = collection.Count - 1;
    //        foreach (var item in collection.Cast<object>().Reverse())
    //            yield return (item, index--);
    //    }
    //}
}
