using System.Collections.Specialized;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
[DebuggerDisplay("Count = {SourceSet.Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public partial class ObservableSelectableSetWrapper<TItem, TSet>
    : ObservableSelectableCollectionWrapper<TItem, TSet>,
        ISetWrapper<TItem, TSet>
    where TItem : notnull
    where TSet : ISet<TItem>, INotifyCollectionChanged
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    public ObservableSelectableSetWrapper(TSet set)
        : base(set) { }

    /// <inheritdoc/>
    public TSet SourceSet => SourceCollection;
}
