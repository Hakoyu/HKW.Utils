using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace HKW.HKWUtils;

// Copyright (c) 2026 ReactiveUI. 保留所有权利
// ReactiveUI 根据一个或多个协议对本文件进行许可
// ReactiveUI 按 MIT 许可证向你授权此文件

/// <summary>
/// 表示一个带记忆化能力的最近最常使用（MRU）缓存
/// </summary>
/// <remarks>
/// <para>
/// 该缓存会记忆化计算函数：给定相同键时，返回缓存值而不重复计算
/// 由于缓存依赖记忆化语义，计算函数应当是数学意义上的“纯函数”：
/// 同一个键必须始终映射到同一个值，缓存结果才是正确的
/// </para>
/// <para>
/// 线程安全：字典和 MRU 链表的所有结构性修改都通过同一个同步门进行同步
/// 该缓存刻意不尝试无锁化，因为字典与 MRU 链表更新需要协同一致
/// </para>
/// </remarks>
/// <typeparam name="TParam">键类型</typeparam>
/// <typeparam name="TVal">缓存值类型</typeparam>
public sealed class MemoizingMRUCache<TParam, TVal>
    where TParam : notnull
{
#if NET9_0_OR_GREATER
    /// <summary>
    /// 用于 MRU 链表和缓存字典所有修改操作的同步门
    /// </summary>
    private readonly Lock _gate = new();
#else
    /// <summary>
    /// 用于 MRU 链表和缓存字典所有修改操作的同步门
    /// </summary>
    private readonly object _gate = new();
#endif

    /// <summary>
    /// 用于在键缺失时生成值的计算函数
    /// </summary>
    private readonly Func<TParam, object?, TVal> _calculationFunction;

    /// <summary>
    /// 当条目被驱逐或显式失效时调用的可选回调
    /// </summary>
    private readonly Action<TVal>? _releaseFunction;

    /// <summary>
    /// 缓存中保留的最大条目数
    /// </summary>
    private readonly int _maxCacheSize;

    /// <summary>
    /// 键比较器
    /// </summary>
    private readonly IEqualityComparer<TParam> _comparer;

    /// <summary>
    /// 键的 MRU 链表头部为最近使用，尾部为最久未使用
    /// </summary>
    private LinkedList<TParam> _mruList;

    /// <summary>
    /// 从键到其 MRU 节点与缓存值的字典
    /// </summary>
    private Dictionary<TParam, (LinkedListNode<TParam> node, TVal value)> _entries;

    /// <summary>
    /// 初始化 <see cref="MemoizingMRUCache{TParam, TVal}"/> 类的新实例
    /// </summary>
    /// <param name="calculationFunc">用于在键缺失时生成值的计算函数</param>
    /// <param name="maxSize">缓存中保留的最大条目数必须 &gt; 0</param>
    /// <remarks><![CDATA[
    /// var cache = new MemoizingMRUCache<string, int>(
    ///     static (key, _) => key.Length,
    ///     maxSize: 128
    /// );
    ///
    /// var value = cache.Get("hello"); // 5
    /// ]]></remarks>
    /// <exception cref="ArgumentNullException">当 <paramref name="calculationFunc"/> 为 <see langword="null"/> 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="maxSize"/> 小于或等于 0 时抛出</exception>
    public MemoizingMRUCache(Func<TParam, object?, TVal> calculationFunc, int maxSize)
        : this(calculationFunc, maxSize, onRelease: null, EqualityComparer<TParam>.Default) { }

    /// <summary>
    /// 初始化 <see cref="MemoizingMRUCache{TParam, TVal}"/> 类的新实例
    /// </summary>
    /// <param name="calculationFunc">用于在键缺失时生成值的计算函数</param>
    /// <param name="maxSize">缓存中保留的最大条目数必须 &gt; 0</param>
    /// <param name="onRelease">当条目被驱逐或失效时调用的回调</param>
    /// <remarks><![CDATA[
    /// var cache = new MemoizingMRUCache<int, string>(
    ///     static (key, _) => $"v:{key}",
    ///     maxSize: 64,
    ///     onRelease: static value => Console.WriteLine($"释放: {value}")
    /// );
    /// ]]></remarks>
    /// <exception cref="ArgumentNullException">当 <paramref name="calculationFunc"/> 或 <paramref name="onRelease"/> 为 <see langword="null"/> 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="maxSize"/> 小于或等于 0 时抛出</exception>
    public MemoizingMRUCache(
        Func<TParam, object?, TVal> calculationFunc,
        int maxSize,
        Action<TVal> onRelease
    )
        : this(calculationFunc, maxSize, onRelease, EqualityComparer<TParam>.Default) { }

    /// <summary>
    /// 初始化 <see cref="MemoizingMRUCache{TParam, TVal}"/> 类的新实例
    /// </summary>
    /// <param name="calculationFunc">用于在键缺失时生成值的计算函数</param>
    /// <param name="maxSize">缓存中保留的最大条目数必须 &gt; 0</param>
    /// <param name="paramComparer">键比较器</param>
    /// <remarks><![CDATA[
    /// var cache = new MemoizingMRUCache<string, string>(
    ///     static (key, _) => key.ToUpperInvariant(),
    ///     maxSize: 32,
    ///     paramComparer: StringComparer.OrdinalIgnoreCase
    /// );
    ///
    /// _ = cache.Get("abc");
    /// _ = cache.Get("ABC"); // 命中同一键（忽略大小写）
    /// ]]></remarks>
    /// <exception cref="ArgumentNullException">当 <paramref name="calculationFunc"/> 或 <paramref name="paramComparer"/> 为 <see langword="null"/> 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="maxSize"/> 小于或等于 0 时抛出</exception>
    public MemoizingMRUCache(
        Func<TParam, object?, TVal> calculationFunc,
        int maxSize,
        IEqualityComparer<TParam> paramComparer
    )
        : this(calculationFunc, maxSize, onRelease: null, paramComparer) { }

    /// <summary>
    /// 初始化 <see cref="MemoizingMRUCache{TParam, TVal}"/> 类的新实例
    /// </summary>
    /// <param name="calculationFunc">用于在键缺失时生成值的计算函数</param>
    /// <param name="maxSize">缓存中保留的最大条目数必须 &gt; 0</param>
    /// <param name="onRelease">当条目被驱逐或失效时调用的可选回调</param>
    /// <param name="paramComparer">键比较器</param>
    /// <remarks><![CDATA[
    /// var cache = new MemoizingMRUCache<string, int>(
    ///     static (key, ctx) => key.Length + (ctx is int n ? n : 0),
    ///     maxSize: 2,
    ///     onRelease: static value => Console.WriteLine($"释放值: {value}"),
    ///     paramComparer: StringComparer.Ordinal
    /// );
    ///
    /// _ = cache.Get("a", context: 1);
    /// _ = cache.Get("bb", context: 1);
    /// _ = cache.Get("ccc", context: 1); // 触发 MRU 淘汰
    /// ]]></remarks>
    /// <exception cref="ArgumentNullException">当 <paramref name="calculationFunc"/> 或 <paramref name="paramComparer"/> 为 <see langword="null"/> 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 <paramref name="maxSize"/> 小于或等于 0 时抛出</exception>
    public MemoizingMRUCache(
        Func<TParam, object?, TVal> calculationFunc,
        int maxSize,
        Action<TVal>? onRelease,
        IEqualityComparer<TParam> paramComparer
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxSize, 0);
        ArgumentNullException.ThrowIfNull(calculationFunc);
        ArgumentNullException.ThrowIfNull(paramComparer);

        _calculationFunction = calculationFunc;
        _releaseFunction = onRelease;
        _maxCacheSize = maxSize;
        _comparer = paramComparer;

        _mruList = [];
        _entries = new(_comparer);
    }

    /// <summary>
    /// 获取 <paramref name="key"/> 对应的缓存值
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>缓存值</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 <see langword="null"/> 时抛出</exception>
    /// <exception cref="KeyNotFoundException">当 <paramref name="key"/> 不存在时抛出</exception>
    public TVal Get(TParam key)
    {
        ArgumentNullException.ThrowIfNull(key);
        lock (_gate)
        {
            return _entries[key].value;
        }
    }

    /// <summary>
    /// 获取 <paramref name="key"/> 对应的缓存值；必要时会计算并写入缓存
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="context">传递给计算函数的可选上下文</param>
    /// <returns>缓存值或新计算得到的值</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 <see langword="null"/> 时抛出</exception>
    public TVal GetOrCreate(TParam key, object? context = null)
    {
        ArgumentNullException.ThrowIfNull(key);

        // 先在锁内尝试命中缓存
        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var found))
            {
                Refresh_NoThrow(found.node);
                return found.value;
            }
        }

        // 未命中：在锁外执行计算
        var computed = _calculationFunction(key, context);

        // 在锁内写入（双重检查，避免其他线程已写入）
        TVal? evictedToRelease = default;
        List<TVal>? evictedBatch = null;
        var hasSingleEvicted = false;

        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var found))
            {
                Refresh_NoThrow(found.node);
                return found.value;
            }

            var node = new LinkedListNode<TParam>(key);
            _mruList.AddFirst(node);
            _entries[key] = (node, computed);

            // 在锁内维护容量，但不在锁内调用释放回调
            if (_entries.Count > _maxCacheSize)
            {
                EvictToSize_NoThrow(ref evictedToRelease, ref evictedBatch, ref hasSingleEvicted);
            }
        }

        // 在锁外调用释放回调
        if (_releaseFunction is not null)
        {
            if (evictedBatch is not null)
            {
                for (var i = 0; i < evictedBatch.Count; i++)
                {
                    _releaseFunction(evictedBatch[i]);
                }
            }
            else if (hasSingleEvicted)
            {
                _releaseFunction(evictedToRelease!);
            }
        }

        return computed;
    }

    /// <summary>
    /// 尝试获取缓存值，不会触发计算
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="result">命中时接收缓存值；否则为默认值</param>
    /// <returns>若键已缓存则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 <see langword="null"/> 时抛出</exception>
    public bool TryGet(TParam key, [MaybeNullWhen(false)] out TVal result)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var found))
            {
                Refresh_NoThrow(found.node);
                result = found.value;
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// 使指定键失效，确保下次查询该键时会调用计算函数
    /// </summary>
    /// <param name="key">要失效的键</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="key"/> 为 <see langword="null"/> 时抛出</exception>
    public void Invalidate(TParam key)
    {
        ArgumentNullException.ThrowIfNull(key);

        TVal? toRelease = default;
        var hasRelease = false;

        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var found) is false)
            {
                return;
            }

            _mruList.Remove(found.node);
            _ = _entries.Remove(key);

            if (_releaseFunction is not null)
            {
                toRelease = found.value;
                hasRelease = true;
            }
        }

        // 在锁外执行释放
        if (hasRelease)
        {
            _releaseFunction!.Invoke(toRelease!);
        }
    }

    /// <summary>
    /// 使缓存中的所有项失效
    /// </summary>
    /// <param name="aggregateReleaseExceptions">
    /// 当为 <see langword="true"/> 时，释放过程中的异常会被收集，
    /// 在所有条目处理完成后以 <see cref="AggregateException"/> 重新抛出
    /// </param>
    public void InvalidateAll(bool aggregateReleaseExceptions = false)
    {
        Dictionary<TParam, (LinkedListNode<TParam> node, TVal value)>? oldEntries = null;

        lock (_gate)
        {
            if (_entries.Count == 0)
            {
                return;
            }

            // 在锁内快速交换缓存容器
            oldEntries = _entries;
            _mruList = [];
            _entries = new(_comparer);
        }

        if (_releaseFunction is null || oldEntries is null)
        {
            return;
        }

        if (aggregateReleaseExceptions is false)
        {
            foreach (var item in oldEntries)
            {
                _releaseFunction(item.Value.value);
            }

            return;
        }

        List<Exception>? exceptions = null;

        foreach (var item in oldEntries)
        {
            try
            {
                _releaseFunction(item.Value.value);
            }
            catch (Exception e)
            {
                (exceptions ??= new()).Add(e);
            }
        }

        if (exceptions is not null && exceptions.Count != 0)
        {
            throw new AggregateException(
                "Exceptions thrown during MRU Cache InvalidateAll item release.",
                exceptions
            );
        }
    }

    /// <summary>
    /// 返回当前缓存中所有值的快照
    /// </summary>
    /// <returns>缓存值的不可变快照</returns>
    public IEnumerable<TVal> CachedValues()
    {
        lock (_gate)
        {
            if (_entries.Count == 0)
            {
                return [];
            }

            var result = new TVal[_entries.Count];
            var i = 0;

            foreach (var entry in _entries)
            {
                result[i++] = entry.Value.value;
            }

            return result;
        }
    }

    /// <summary>
    /// 将被驱逐的值记录到单值槽位或批量列表中
    /// </summary>
    /// <param name="value">被驱逐的值</param>
    /// <param name="singleEvicted">单值槽位</param>
    /// <param name="batchEvicted">用于多个驱逐值的批量列表</param>
    /// <param name="hasSingleEvicted">已被驱逐</param>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void RecordEvictedValueNoThrow(
        TVal value,
        ref TVal? singleEvicted,
        ref List<TVal>? batchEvicted,
        ref bool hasSingleEvicted
    )
    {
        if (batchEvicted is not null)
        {
            batchEvicted.Add(value);
            return;
        }

        // 第一次驱逐写入单值槽位
        if (hasSingleEvicted is false)
        {
            singleEvicted = value;
            hasSingleEvicted = true;
            return;
        }

        // 第二次驱逐升级为列表存储
        batchEvicted = new(capacity: 4) { singleEvicted!, value };
        singleEvicted = default;
        hasSingleEvicted = false;
    }

    /// <summary>
    /// 将节点移动到 MRU 链表头部
    /// </summary>
    /// <param name="node">要刷新的节点</param>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void Refresh_NoThrow(LinkedListNode<TParam> node)
    {
        // 只有一个条目时无需进行链表调整
        if (_entries.Count <= 1)
        {
            return;
        }

        _mruList.Remove(node);
        _mruList.AddFirst(node);
    }

    /// <summary>
    /// 持续驱逐最久未使用条目，直到缓存大小不超过 <see cref="_maxCacheSize"/>
    /// </summary>
    /// <param name="singleEvicted">当仅发生一次驱逐时接收该值</param>
    /// <param name="batchEvicted">当发生多次驱逐时接收所有驱逐值</param>
    /// <param name="hasSingleEvicted">已被驱逐</param>
    /// <remarks>
    /// 调用方必须持有 <c>lock(_gate)</c>此方法不会调用释放回调
    /// </remarks>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void EvictToSize_NoThrow(
        ref TVal? singleEvicted,
        ref List<TVal>? batchEvicted,
        ref bool hasSingleEvicted
    )
    {
        while (_entries.Count > _maxCacheSize)
        {
            var lastNode = _mruList.Last;
            if (lastNode is null)
            {
                return;
            }

            var key = lastNode.Value;

#if NET8_0_OR_GREATER
            ref var entry = ref CollectionsMarshal.GetValueRefOrNullRef(_entries, key);
            if (Unsafe.IsNullRef(ref entry) is false)
            {
                RecordEvictedValueNoThrow(
                    entry.value,
                    ref singleEvicted,
                    ref batchEvicted,
                    ref hasSingleEvicted
                );
            }
#else
            RecordEvictedValueNoThrow(
                _entries[key].value,
                ref singleEvicted,
                ref batchEvicted,
                ref hasSingleEvicted
            );
#endif

            _ = _entries.Remove(key);
            _mruList.RemoveLast();
        }
    }

    /// <summary>
    /// 确保缓存不变量得到维护
    /// </summary>
    [ContractInvariantMethod]
    private void Invariants()
    {
        Contract.Invariant(_entries.Count == _mruList.Count);
        Contract.Invariant(_entries.Count <= _maxCacheSize);
    }
}
