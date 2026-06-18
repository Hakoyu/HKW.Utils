using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="collection">集合</param>
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// 包含索引值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < collection.Count;
        }

        /// <summary>
        /// 含有值
        /// <para>等价于 <c>Count > 0</c></para>
        /// </summary>
        /// <returns>含有值为 <see langword="true"/> 否则为 <see langword="false"/></returns>
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
        /// <returns>带有索引的枚举值(索引, 枚举值)</returns>
        public IEnumerable<(int Index, T Item)> ReverseEnumerateIndex()
        {
            var index = collection.Count - 1;
            foreach (var item in collection.Reverse())
                yield return (index--, item);
        }
    }

    /// <param name="collection">集合</param>
    extension(ICollection collection)
    {
        /// <summary>
        /// 倒序枚举出带有索引值的枚举值
        /// </summary>
        /// <returns>带有索引的枚举值(索引, 枚举值)</returns>
        public IEnumerable<(int Index, object Item)> ReverseEnumerateIndex()
        {
            var index = collection.Count - 1;
            foreach (var item in collection.Cast<object>().Reverse())
                yield return (index--, item);
        }
    }

    ///// <summary>
    ///// 删除全部符合条件的项目
    ///// </summary>
    ///// <typeparam name="T">项类型</typeparam>
    ///// <param name="collection">集合</param>
    ///// <param name="match">条件</param>
    //public static void RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
    //{
    //    if (collection is IList<T> list)
    //    {
    //        list.RemoveAll(match);
    //    }
    //    else
    //    {
    //        new HashSet<int>().RemoveWhere();
    //    }
    //}
}
