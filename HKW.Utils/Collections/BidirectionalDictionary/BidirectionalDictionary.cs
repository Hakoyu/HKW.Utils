using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 双向字典
/// </summary>
/// <typeparam name="T1">项目类型1</typeparam>
/// <typeparam name="T2">项目类型2</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class BidirectionalDictionary<T1, T2>
    : BidirectionalDictionaryWrapper<T1, T2, Dictionary<T1, T2>, Dictionary<T2, T1>>
    where T1 : notnull
    where T2 : notnull
{
    /// <inheritdoc/>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
        : base(new(comparer1), new(comparer2), comparer1, comparer2) { }

    /// <inheritdoc/>
    /// <param name="capacity">初始容量</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        int capacity,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
        : base(new(capacity, comparer1), new(capacity, comparer2), comparer1, comparer2) { }

    /// <inheritdoc/>
    /// <param name="pairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> pairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
        : base(new(comparer1), new(comparer2), comparer1, comparer2)
    {
        ArgumentNullException.ThrowIfNull(pairs);
        foreach (var pair in pairs)
        {
            ArgumentNullException.ThrowIfNull(pair.Key);
            ArgumentNullException.ThrowIfNull(pair.Value);
            _dictionary1.Add(pair.Key, pair.Value);
            _dictionary2.Add(pair.Value, pair.Key);
        }
    }
}
