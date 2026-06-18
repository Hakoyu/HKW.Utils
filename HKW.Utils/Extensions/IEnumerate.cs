using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    extension(IEnumerable source)
    {
        /// <summary>
        /// 枚举出带有索引值的枚举值
        /// </summary>
        /// <returns>带有索引的枚举值(索引, 枚举值)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(int Index, object Item)> EnumerateIndex()
        {
            var index = 0;
            foreach (var item in source)
                yield return (index++, item);
        }

        /// <summary>
        /// 判断两个集合的值是否全部相等 (无视顺序)
        /// </summary>
        /// <param name="target">集合2</param>
        /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ItemsEqual(IEnumerable target)
        {
            return source.Cast<object>().Except(target.Cast<object>()).Any() is false;
        }

        /// <summary>
        /// 序列相等
        /// </summary>
        /// <param name="second">第二个集合</param>
        /// <param name="comparer">比较器</param>
        /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SequenceEqual(IEnumerable second, Func<object?, object?, bool> comparer)
        {
            ArgumentNullException.ThrowIfNull(second);
            ArgumentNullException.ThrowIfNull(comparer);

            if (source is ICollection sourceCol && second is ICollection secondCol)
            {
                if (sourceCol.Count != secondCol.Count)
                    return false;

                if (sourceCol is IList firstList && secondCol is IList secondList)
                {
                    for (var i = 0; i < sourceCol.Count; i++)
                    {
                        if (comparer(firstList[i], secondList[i]) is false)
                            return false;
                    }

                    return true;
                }
            }

            var e1 = source.GetEnumerator();
            var e2 = second.GetEnumerator();
            while (e1.MoveNext())
            {
                if ((e2.MoveNext() && comparer(e1.Current, e2.Current)) is false)
                    return false;
            }
            return e2.MoveNext() is false;
        }
    }
}
