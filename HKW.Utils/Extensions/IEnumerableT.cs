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
        /// <returns>是否获取成功; 索引越过末尾时返回 false 和默认值</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引为负数</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryElementAt(int index, [MaybeNullWhen(false)] out T item)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            if (source is IList<T> list)
            {
                if (index < list.Count)
                {
                    item = list[index];
                    return true;
                }
                item = default;
                return false;
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
        /// <param name="match">匹配</param>
        /// <returns>项目的索引, 若项目不存在则为 <see langword="-1"/> </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(Func<T, bool> match)
        {
            if (source is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                    if (match(list[i]) is true)
                        return i;
            }
            else
            {
                foreach (var (e, i) in source.WithIndex())
                {
                    if (match(e) is true)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断两个集合的值是否全部相等 (无视顺序)
        /// </summary>
        /// <remarks>
        /// 按默认相等比较器比较多重集, 包含重复次数和 null; 不使用输入集合自身的比较器.
        /// </remarks>
        /// <param name="target">目标</param>
        /// <returns>是否相等</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable S3776
        public bool UnorderedEqual(IEnumerable<T> target)
#pragma warning restore S3776
        {
            ArgumentNullException.ThrowIfNull(source);
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

                count = sourceCol.Count;
            }

#pragma warning disable CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。
            var counts = count > 0 ? new Dictionary<T, int>(count) : new Dictionary<T, int>();

            var nullCount = 0;
            foreach (var item in source)
            {
                if (item is null)
                {
                    nullCount++;
                    continue;
                }
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
                if (item is null)
                {
                    if (nullCount == 0)
                        return false;
                    nullCount--;
                    continue;
                }
                ref var countRef = ref CollectionsMarshal.GetValueRefOrNullRef(counts, item);
                if (Unsafe.IsNullRef(ref countRef))
                    return false;

                countRef--;
                if (countRef == 0)
                    counts.Remove(item);
            }
#pragma warning restore CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。

            return counts.Count == 0 && nullCount == 0;
        }

        /// <summary>
        /// 获取枚举中一个随机的值
        /// </summary>
        /// <remarks>
        /// 列表直接随机索引; 其他输入仅枚举一次, 以蓄水池采样等概率选择项目.
        /// </remarks>
        /// <exception cref="InvalidOperationException">序列为空</exception>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>随机的一个值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Random(Random? random = null)
        {
            ArgumentNullException.ThrowIfNull(source);
            random ??= System.Random.Shared;
            if (source is IList<T> list)
            {
                var count = list.Count;
                if (count == 0)
                    throw new InvalidOperationException("The sequence contains no elements.");
                return list[random.Next(count)];
            }

            var seen = 0;
            T result = default!;
            foreach (var item in source)
            {
                if (random.Next(checked(++seen)) == 0)
                    result = item;
            }
            if (seen == 0)
                throw new InvalidOperationException("The sequence contains no elements.");
            return result;
        }

        /// <summary>
        /// 获取枚举的随机排序
        /// </summary>
        /// <remarks>
        /// 调用时一次物化输入并以 Fisher–Yates 等概率洗牌; 不修改输入, 重复枚举保留同一排列.
        /// 返回普通序列而非有序序列, 不支持通过 ThenBy 追加排序键.
        /// </remarks>
        /// <param name="random">随机类, 若为 <see langword="null"/> 则使用 <see cref="System.Random.Shared"/></param>
        /// <returns>枚举的随机排序</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> RandomOrder(Random? random = null)
        {
            ArgumentNullException.ThrowIfNull(source);
            random ??= System.Random.Shared;
            var snapshot = source.ToArray();
            for (var i = snapshot.Length - 1; i > 0; i--)
            {
                var index = random.Next(i + 1);
                (snapshot[i], snapshot[index]) = (snapshot[index], snapshot[i]);
            }
            return snapshot;
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
    /// <remarks>
    /// 返回数组或 List 的底层存储视图, 不创建快照, 也不进行枚举器版本校验.
    /// 使用 List 的区块期间不得添加, 删除, 清空元素, 调整容量或并发写入;
    /// 结构修改可能使区块继续引用旧缓冲区或读取被清空, 覆盖的数据, 区块长度不会随列表变化.
    /// 只读区块不阻止其他引用修改底层元素; 需要稳定快照时请先调用 ToArray.
    /// </remarks>
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
