using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ObservableList<T> : ObservableListWrapper<T, List<T>>
{
    /// <inheritdoc/>
    public ObservableList()
        : base(new()) { }

    /// <inheritdoc/>
    public ObservableList(int capacity)
        : base(new(capacity)) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableList(IEnumerable<T> collection)
        : base(new(collection)) { }
}
