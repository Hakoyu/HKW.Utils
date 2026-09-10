using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSelectableList<T> : ObservableSelectableListWrapper<T, List<T>>
{
    /// <inheritdoc/>
    public ObservableSelectableList()
        : base(new()) { }

    /// <inheritdoc/>
    public ObservableSelectableList(int capacity)
        : base(new(capacity)) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSelectableList(IEnumerable<T> collection)
        : base(new(collection)) { }
}
