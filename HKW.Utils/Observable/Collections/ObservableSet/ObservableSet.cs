using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ObservableSet<T> : ObservableSetWrapper<T, OrderedSet<T>>
{
    /// <inheritdoc/>
    public ObservableSet()
        : base(new(), EqualityComparer<T>.Default) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public ObservableSet(int capacity)
        : base(new(capacity), EqualityComparer<T>.Default) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEqualityComparer<T> comparer)
        : base(new(comparer), comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSet(IEnumerable<T> collection)
        : base(new(collection), EqualityComparer<T>.Default) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        : base(new(collection, comparer), comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSet(int capacity, IEqualityComparer<T> comparer)
        : base(new(capacity, comparer), comparer) { }
}
