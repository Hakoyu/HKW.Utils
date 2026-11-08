using System.Collections;
using System.Diagnostics;

namespace HKW.HKWUtils.DebugViews;

/// <summary>
/// 集合调试视图
/// </summary>
/// <remarks>
/// 数组直接返回原引用; 其他源每次读取都会重新枚举为 object[], 时间和存储成本为 O(n), 值类型可能装箱.
/// 枚举可能触发源的副作用, 不应展开无限序列. 不缓存结果, 以保留实时视图语义.
/// </remarks>
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
