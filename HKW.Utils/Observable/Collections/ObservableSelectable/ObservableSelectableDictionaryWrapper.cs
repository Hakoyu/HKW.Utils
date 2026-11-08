using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class ObservableSelectableDictionaryWrapper<TKey, TValue, TDictionary>
#pragma warning restore S2436
    : ObservableDictionaryWrapper<TKey, TValue, TDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    /// <param name="dictionary">集合</param>
    /// <param name="comparer">比较器, 必须与 <paramref name="dictionary"/> 的比较器相同</param>
    public ObservableSelectableDictionaryWrapper(
        TDictionary dictionary,
        IEqualityComparer<TKey>? comparer = null
    )
        : base(dictionary, comparer) { }

    private bool _hasSelection;

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection => _hasSelection;

    /// <summary>
    /// 选中的键
    /// </summary>
    public TKey SelectedKey
    {
        get => SelectedItem.Key;
        set
        {
            using var operation = BeginOperation();
            if (SourceDictionary.TryGetValue(value, out var storedValue))
                SetSelectedItemCore(new(value, storedValue));
            else
                ClearSelectionCore();
        }
    }

    /// <summary>
    /// 选中的值
    /// </summary>
    public TValue SelectedValue => _selectedItem.Value;

    private KeyValuePair<TKey, TValue> _selectedItem = default!;

    /// <summary>
    /// 选中的项目
    /// </summary>
    public KeyValuePair<TKey, TValue> SelectedItem
    {
        get => _selectedItem;
        set
        {
            using var operation = BeginOperation();
            SetSelectedItemCore(value);
        }
    }

    /// <inheritdoc/>
    protected override void OnDictionaryRemoved(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair,
        int removeIndex
    )
    {
        if (_hasSelection && Comparer.Equals(_selectedItem.Key, pair.Key))
            ClearSelectionCore();
        else if (CommitSelectionIndex())
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);

        base.OnDictionaryRemoved(args, pair, removeIndex);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryReplaced(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (_hasSelection && Comparer.Equals(_selectedItem.Key, oldPair.Key))
            CommitSelection(newPair, true);

        base.OnDictionaryReplaced(args, newPair, oldPair);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryCleared()
    {
        ClearSelectionCore();
        base.OnDictionaryCleared();
    }

    /// <summary>
    /// 在当前操作作用域内选择键值对, 缺失或值不匹配时清除选择.
    /// </summary>
    private protected void SetSelectedItemCore(KeyValuePair<TKey, TValue> item)
    {
        if (
            SourceDictionary.TryGetValue(item.Key, out var value) is false
            || EqualityComparer<TValue>.Default.Equals(value, item.Value) is false
        )
        {
            ClearSelectionCore();
            return;
        }

        if (SourceDictionary is OrderedDictionary<TKey, TValue> ordered)
            item = ordered.GetAt(ordered.IndexOf(item.Key));
        else
            item = GetStoredSelection(item.Key);
        CommitSelection(item, true);
    }

    /// <summary>
    /// 清除选择, 不删除键值对. 默认键值对仍可通过选择属性选中.
    /// </summary>
    public void ClearSelection()
    {
        using var operation = BeginOperation();
        ClearSelectionCore();
    }

    /// <summary>
    /// 在当前操作作用域内清除选择.
    /// </summary>
    private protected void ClearSelectionCore() => CommitSelection(default, false);

    private void CommitSelection(KeyValuePair<TKey, TValue> item, bool hasSelection)
    {
        var selectionChanged = _hasSelection != hasSelection;
        var keyChanged =
            EqualityComparer<TKey>.Default.Equals(_selectedItem.Key, item.Key) is false;
        var valueChanged =
            EqualityComparer<TValue>.Default.Equals(_selectedItem.Value, item.Value) is false;
        if (selectionChanged is false && keyChanged is false && valueChanged is false)
            return;

        _selectedItem = item;
        _hasSelection = hasSelection;
        var indexChanged = CommitSelectionIndex();
        OnPropertyChanged(PropertyChangedArgsCache.SelectedItem);
        if (selectionChanged || keyChanged)
            OnPropertyChanged(PropertyChangedArgsCache.SelectedKey);
        if (selectionChanged || valueChanged)
            OnPropertyChanged(PropertyChangedArgsCache.SelectedValue);
        if (selectionChanged)
            OnPropertyChanged(PropertyChangedArgsCache.HasSelection);
        if (indexChanged)
            OnPropertyChanged(PropertyChangedArgsCache.SelectedIndex);
        OnSelectionChanged();
    }

    private KeyValuePair<TKey, TValue> GetStoredSelection(TKey key)
    {
        foreach (var pair in SourceDictionary)
            if (Comparer.Equals(pair.Key, key))
                return pair;
        throw new InvalidOperationException(
            "The source dictionary comparer must match the supplied comparer."
        );
    }

    private int _selectionIndex = -1;

    /// <summary>
    /// 获取有序后端的选择索引, 无索引后端返回 -1.
    /// </summary>
    protected virtual int GetSelectionIndex() => -1;

    private bool CommitSelectionIndex()
    {
        var index = GetSelectionIndex();
        var changed = index != _selectionIndex;
        _selectionIndex = index;
        return changed;
    }

    /// <summary>
    /// 选择状态改变后
    /// </summary>
    protected virtual void OnSelectionChanged() { }
}
