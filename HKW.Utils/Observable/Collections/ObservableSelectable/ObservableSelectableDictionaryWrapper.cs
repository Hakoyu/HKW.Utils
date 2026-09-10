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
        set => SelectedItem = SourceDictionary.GetPair(value);
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
        set => SetSelectedItem(value);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryRemoved(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair,
        int removeIndex
    )
    {
        if (EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_selectedItem, pair))
            ClearSelection();

        base.OnDictionaryRemoved(args, pair, removeIndex);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryReplaced(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_selectedItem, oldPair))
            SetSelectedItem(newPair);

        base.OnDictionaryReplaced(args, newPair, oldPair);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryCleared()
    {
        ClearSelection();
        base.OnDictionaryCleared();
    }

    private void SetSelectedItem(KeyValuePair<TKey, TValue> item)
    {
        if (EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_selectedItem, item))
            return;

        _selectedItem = item;
        _hasSelection = EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, default)
            ? false
            : Contains(item);
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(SelectedKey));
        OnPropertyChanged(nameof(SelectedValue));
        OnPropertyChanged(nameof(HasSelection));
        OnSelectionChanged();
    }

    private void ClearSelection()
    {
        if (
            _hasSelection is false
            && EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_selectedItem, default!)
        )
            return;

        _selectedItem = default!;
        _hasSelection = false;
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(SelectedKey));
        OnPropertyChanged(nameof(SelectedValue));
        OnPropertyChanged(nameof(HasSelection));
        OnSelectionChanged();
    }

    /// <summary>
    /// 选择状态改变后
    /// </summary>
    protected virtual void OnSelectionChanged() { }
}
