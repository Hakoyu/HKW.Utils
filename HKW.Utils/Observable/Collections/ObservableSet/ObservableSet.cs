using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSet<T> : ObservableSetWrapper<T, OrderedSet<T>>
    where T : notnull
{
    /// <inheritdoc/>
    public ObservableSet()
        : base(new()) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
        : base(new(collection, comparer), comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T>? comparer = null)
        : base(new(capacity, comparer), comparer) { }
}
