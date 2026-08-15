using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class EnumerableExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="source">源</param>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// 枚举出带有索引值的枚举值
        /// </summary>
        /// <returns>带有索引的枚举值(枚举值, 索引)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(T Item, int Index)> WithIndex()
        {
            if (source is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                    yield return (list[i], i);
            }
            else
            {
                var index = 0;
                foreach (var item in source)
                    yield return (item, index++);
            }
        }

        /// <summary>
        /// 尝试使用索引值获取项目
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="item">项目</param>
        /// <returns>是否获取成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryElementAt(int index, [MaybeNullWhen(false)] out T item)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            if (source is IList<T> list)
            {
                item = list[index];
                return true;
            }
            var tempIndex = 0;
            using var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                if (tempIndex++ == index)
                {
                    item = e.Current;
                    return true;
                }
            }
            item = default;
            return false;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <typeparam name="TArg">参数类型</typeparam>
        /// <param name="arg">参数</param>
        /// <param name="match">匹配</param>
        /// <returns>是否包含</returns>
        public bool Contains<TArg>(TArg arg, Func<T, TArg, bool> match)
        {
            ArgumentNullException.ThrowIfNull(match);

            if (source is IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (match(list[i], arg))
                        return true;
                }
            }
            else
            {
#pragma warning disable S3267
                foreach (var item in source)
                {
                    if (match(item, arg))
                        return true;
                }
#pragma warning restore S3267
            }
            return false;
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="item">项目</param>
        /// <returns>项目的索引, 若项目不存在则为 <see langword="-1"/> </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            if (source is IList<T> list)
            {
                return list.IndexOf(item);
            }
            else
            {
                foreach (var (e, i) in source.WithIndex())
                {
                    if (EqualityComparer<T>.Default.Equals(item, e))
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="match">匹配</param>
        /// <returns>项目的索引, 若项目不存在则为 <see langword="-1"/> </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf<TArg>(TArg args, Func<T, TArg, bool> match)
        {
            if (source is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                    if (match(list[i], args) is true)
                        return i;
            }
            else
            {
                foreach (var (e, i) in source.WithIndex())
                {
                    if (match(e, args) is true)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断两个集合的值是否全部相等 (无视顺序)
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>是否相等</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable S3776
        public bool UnorderedEqual(IEnumerable<T> target)
#pragma warning restore S3776
        {
            ArgumentNullException.ThrowIfNull(target);

            if (ReferenceEquals(source, target))
                return true;

            var count = 0;
            if (source is ICollection<T> sourceCol && target is ICollection<T> targetCol)
            {
                if (sourceCol.Count != targetCol.Count)
                    return false;

                if (sourceCol.Count == 0)
                    return true;

                if (sourceCol is ISet<T> sourceSet && targetCol is ISet<T> targetSet)
                    return sourceSet.SetEquals(targetSet);

                count = int.Max(sourceCol.Count, targetCol.Count);
            }

#pragma warning disable CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。
            var counts = count > 0 ? new Dictionary<T, int>(count) : new Dictionary<T, int>();

            foreach (var item in source)
            {
                ref var countRef = ref CollectionsMarshal.GetValueRefOrAddDefault(
                    counts,
                    item,
                    out var exists
                );
                if (exists)
                    countRef++;
                else
                    countRef = 1;
            }

            foreach (var item in target)
            {
                ref var countRef = ref CollectionsMarshal.GetValueRefOrNullRef(counts, item);
                if (Unsafe.IsNullRef(ref countRef))
                    return false;

                countRef--;
                if (countRef == 0)
                    counts.Remove(item);
            }
#pragma warning restore CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。

            return counts.Count == 0;
        }

        /// <summary>
        /// 获取枚举中一个随机的值
        /// </summary>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>随机的一个值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Random(Random? random = null)
        {
            random ??= System.Random.Shared;
            return source.ElementAt(random.Next(source.Count()));
        }

        /// <summary>
        /// 获取枚举的随机排序
        /// </summary>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>枚举的随机排序</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<T> RandomOrder(Random? random = null)
        {
            random ??= System.Random.Shared;
            return source.OrderBy(x => random.Next(source.Count()));
        }

        /// <summary>
        /// 可使用匿名方法作为比较器的序列相等
        /// </summary>
        /// <param name="second">第二个集合</param>
        /// <param name="comparer">比较器</param>
        /// <returns>是否相等</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable S3776
        public bool SequenceEqual(IEnumerable<T> second, Func<T, T, bool> comparer)
#pragma warning restore S3776
        {
            ArgumentNullException.ThrowIfNull(second);
            ArgumentNullException.ThrowIfNull(comparer);
            if (source is ICollection<T> sourceCol && second is ICollection<T> secondCol)
            {
                if (sourceCol.Count != secondCol.Count)
                    return false;

                if (sourceCol is IList<T> firstList && secondCol is IList<T> secondList)
                {
                    for (var i = 0; i < sourceCol.Count; i++)
                    {
                        if (comparer(firstList[i], secondList[i]) is false)
                            return false;
                    }

                    return true;
                }
            }

            using var e1 = source.GetEnumerator();
            using var e2 = second.GetEnumerator();

            while (e1.MoveNext())
            {
                if ((e2.MoveNext() && comparer(e1.Current, e2.Current)) is false)
                {
                    return false;
                }
            }

            return e2.MoveNext() is false;
        }

        /// <summary>
        /// 基于依据筛选项目
        /// </summary>
        /// <typeparam name="TArg">参数类型</typeparam>
        /// <param name="arg">参数</param>
        /// <param name="predicate">依据</param>
        /// <returns>项目枚举</returns>
        public IEnumerable<T> Where<TArg>(TArg arg, Func<T, TArg, bool> predicate)
        {
            if (source is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (predicate(item, arg))
                        yield return item;
                }
            }
            else
            {
#pragma warning disable S3267
                foreach (var item in source)
                {
                    if (predicate(item, arg))
                        yield return item;
                }
#pragma warning restore S3267
            }
        }
    }

    /// <summary>
    /// 处理枚举中的全部项目
    /// </summary>
    /// <typeparam name="T">可处理项目</typeparam>
    /// <param name="values">集合</param>
    public static void DisposeAll<T>(this IEnumerable<T> values)
        where T : IDisposable
    {
        foreach (var value in values)
            value.Dispose();
    }

    /// <summary>
    /// 尝试获取区块
    /// </summary>
    /// <typeparam name="T">源类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="span">区块</param>
    /// <returns>是否获取成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetSpan<T>(this IEnumerable<T> source, out ReadOnlySpan<T> span)
    {
        bool result = true;

        if (source is T[])
        {
            span = Unsafe.As<T[]>(source);
        }
        else if (source is List<T>)
        {
            span = CollectionsMarshal.AsSpan(Unsafe.As<List<T>>(source));
        }
        else
        {
            span = default(ReadOnlySpan<T>);
            result = false;
        }

        return result;
    }
}
