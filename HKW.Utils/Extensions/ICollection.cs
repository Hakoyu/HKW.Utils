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
        /// 范围添加
        /// </summary>
        /// <remarks>
        /// 写入前将输入保存为快照, 支持自身, 共享底层存储的包装及延迟枚举.
        /// </remarks>
        /// <param name="items">项目</param>
        public void AddRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            var snapshot = items.ToArray();
            if (collection is List<T> list)
            {
                list.AddRange(snapshot);
            }
            else
            {
                foreach (var item in snapshot)
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
        /// <remarks>
        /// 先保存输入快照, 再从后向前逐项删除; 每个输入项仅删除目标中的一个匹配项.
        /// </remarks>
        /// <param name="items">对象</param>
        public void RemoveAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            var snapshot = items.ToArray();
            for (var i = snapshot.Length - 1; i >= 0; i--)
                collection.Remove(snapshot[i]);
        }
    }
}
