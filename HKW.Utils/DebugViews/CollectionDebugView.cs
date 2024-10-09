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
        get => _collection is Array array ? array : _collection.Cast<object>().ToArray();
    }

    private readonly IEnumerable _collection;

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public CollectionDebugView(IEnumerable collection)
    {
        _collection = collection;
    }
}
