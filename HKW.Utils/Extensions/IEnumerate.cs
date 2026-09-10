using System.Collections;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class EnumerableExtensions
{
    extension(IEnumerable source)
    {
        /// <summary>
        /// 枚举出带有索引值的枚举值
        /// </summary>
        /// <returns>带有索引的枚举值(枚举值, 索引)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(object Item, int Index)> WithIndex()
        {
            if (source is IList list)
            {
                for (var i = 0; i < list.Count; i++)
                    yield return (list[i]!, i);
            }
            else
            {
                var index = 0;
                foreach (var item in source)
                    yield return (item, index++);
            }
        }

        /// <summary>
        /// 判断两个集合的值是否全部相等 (无视顺序)
        /// </summary>
        /// <param name="target">集合2</param>
        /// <returns>是否相等</returns>
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
        /// <returns>是否相等</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable S3776
        public bool SequenceEqual(IEnumerable second, Func<object?, object?, bool> comparer)
#pragma warning restore S3776
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

    extension(Enumerable)
    {
        /// <summary>
        /// 字符串范围
        /// </summary>
        /// <param name="start">开始</param>
        /// <param name="count">数量</param>
        /// <returns>范围的字符串枚举</returns>
        /// <remarks><![CDATA[
        /// Enumerable.StringRange(1,3);
        /// return:
        /// ["1", "2", "3"]
        /// ]]></remarks>
        public static IEnumerable<string> StringRange(int start, int count)
        {
            long max = (long)start + (long)count - 1;
            ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(max, int.MaxValue);

            var end = start + count;
            for (var i = start; i < end; i++)
                yield return i.ToString();
        }
    }
}
