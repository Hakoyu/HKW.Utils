using System.Collections;
using System.Diagnostics;

namespace HKW.HKWUtils.DebugViews;

/// <summary>
/// 集合调试视图
/// </summary>
public class IEnumerableDebugView
{
    /// <summary>
    /// 集合
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Array Array
    {
#pragma warning disable S2365
        get => _collection is Array array ? array : _collection.Cast<object>().ToArray();
#pragma warning restore S2365
    }

    private readonly IEnumerable _collection;

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public IEnumerableDebugView(IEnumerable collection)
    {
        _collection = collection;
    }
}
