using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable.Collections;

/// <summary>
/// 可观察过滤字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
public class ObservableFilterDictionary<TKey, TValue, TFilteredDictionary>
    : ObservableDictionary<TKey, TValue>,
        IFilterCollection<KeyValuePair<TKey, TValue>, TFilteredDictionary>
    where TKey : notnull
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    #region Ctor
    /// <inheritdoc/>
    public ObservableFilterDictionary()
        : base(null!, null) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableFilterDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : base(collection, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableFilterDictionary(IEqualityComparer<TKey> comparer)
        : base(null!, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableFilterDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
        : base(collection, comparer) { }
    #endregion

    #region IFilterCollection

    private Predicate<KeyValuePair<TKey, TValue>> _filter = null!;

    /// <inheritdoc/>
    public required Predicate<KeyValuePair<TKey, TValue>> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }
    private TFilteredDictionary _filteredDictionary = default!;

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public required TFilteredDictionary FilteredDictionary
    {
        get => _filteredDictionary;
        init
        {
            _filteredDictionary = value;
            Refresh();
            OnPropertyChanged(nameof(FilteredDictionary));
        }
    }
    TFilteredDictionary IFilterCollection<
        KeyValuePair<TKey, TValue>,
        TFilteredDictionary
    >.FilteredCollection => FilteredDictionary;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (FilteredDictionary is null || Filter is null)
            return;
        FilteredDictionary.Clear();
        if (this.HasValue())
            FilteredDictionary.AddRange(this.Where(i => Filter(i)));
    }
    #endregion

    #region DictionaryChanging
    /// <inheritdoc/>
    protected override void OnDictionaryAdded(KeyValuePair<TKey, TValue> pair)
    {
        base.OnDictionaryAdded(pair);
        if (Filter(pair))
            FilteredDictionary.Add(pair);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryRemoved(KeyValuePair<TKey, TValue> pair)
    {
        base.OnDictionaryRemoved(pair);
        FilteredDictionary.Remove(pair);
    }

    /// <inheritdoc/>
    protected override void OnDictionaryReplaced(
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        base.OnDictionaryReplaced(newPair, oldPair);
        if (Filter(newPair))
        {
            if (FilteredDictionary.ContainsKey(newPair.Key))
                FilteredDictionary[newPair.Key] = newPair.Value;
        }
        else
        {
            FilteredDictionary.Remove(newPair);
        }
    }

    /// <inheritdoc/>
    protected override void OnDictionaryCleared()
    {
        base.OnDictionaryCleared();
        FilteredDictionary.Clear();
    }
    #endregion
}
