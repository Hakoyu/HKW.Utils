using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableDictionary<TKey, TValue>
    : ObservableSelectableDictionaryWrapper<TKey, TValue, Dictionary<TKey, TValue>>
    where TKey : notnull
{
    /// <inheritdoc/>
    public ObservableSelectableDictionary()
        : base([]) { }

    /// <inheritdoc/>
    /// <param name="selectedKey">选中的键</param>
    public ObservableSelectableDictionary(TKey selectedKey)
        : base([], selectedKey) { }

    /// <inheritdoc/>
    /// <param name="pairs">项目</param>
    public ObservableSelectableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        : base(new(pairs)) { }

    /// <inheritdoc/>
    /// <param name="pairs">项目</param>
    /// <param name="selectedKey">选中的键</param>
    public ObservableSelectableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs,
        TKey selectedKey
    )
        : base(new(pairs), selectedKey) { }
}
