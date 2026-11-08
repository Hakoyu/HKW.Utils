using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSelectableDictionary<TKey, TValue>
    : ObservableSelectableDictionaryWrapper<TKey, TValue, OrderedDictionary<TKey, TValue>>
    where TKey : notnull
{
    /// <inheritdoc/>
    public ObservableSelectableDictionary()
        : base(new()) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableSelectableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : base(new(collection)) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableDictionary(IEqualityComparer<TKey> comparer)
        : base(new(comparer), comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
        : base(new(collection, comparer), comparer) { }

    /// <summary>
    /// 选中的索引
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (HasSelection is false)
                return -1;

            return SourceDictionary.IndexOf(SelectedKey);
        }
        set
        {
            using var operation = BeginOperation();
            if (value == -1)
            {
                ClearSelectionCore();
                return;
            }

            ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(SourceDictionary, value);
            SetSelectedItemCore(SourceDictionary.GetAt(value));
        }
    }

    /// <inheritdoc/>
    protected override int GetSelectionIndex() => SelectedIndex;
}
