using System.Collections;
using System.Diagnostics;

namespace HKW.HKWUtils.DebugViews;

/// <summary>
/// 集合调试视图
/// </summary>
public class CollectionDebugView
{
    /// <summary>
    /// 集合
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Array Array
    {
        get => r_collection.Cast<object>().ToArray();
    }

    private readonly IEnumerable r_collection;

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public CollectionDebugView(IEnumerable collection)
    {
        r_collection = collection;
    }
}
