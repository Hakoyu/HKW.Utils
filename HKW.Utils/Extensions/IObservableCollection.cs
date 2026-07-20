using System.Collections.Specialized;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
#pragma warning disable S3776
public static partial class ObservableCollectionExtensions
{
    #region BindingList
    /// <summary>
    /// (INotifyListChanged, ILists)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingListXs = [];

    /// <summary>
    /// 绑定列表, 将源列表的修改同步至目标列表
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceList">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingListX<T>(
        this INotifyListChanged<T> sourceList,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        if (_bindingListXs.TryGetValue(sourceList, out var set) is false)
        {
            set = _bindingListXs[sourceList] = new();
            sourceList.ListChanged += SourceList_ListChanged;
        }
        if (unBinding)
        {
            set.Remove(targetList);
            if (set.Count == 0)
            {
                _bindingListXs.Remove(sourceList);
                sourceList.ListChanged -= SourceList_ListChanged;
            }
            return;
        }

        set.Add(targetList);

        static void SourceList_ListChanged(
            INotifyListChanged<T> sender,
            NotifyListChangeEventArgs<T> e
        )
        {
            if (e.Action is ListChangeAction.Add)
            {
                foreach (var list in _bindingListXs[sender].Cast<IList<T>>())
                    list.Insert(e.Index, e.NewItem!);
            }
            else if (e.Action is ListChangeAction.Remove)
            {
                foreach (var list in _bindingListXs[sender].Cast<IList<T>>())
                    list.Remove(e.OldItem!);
            }
            else if (e.Action is ListChangeAction.Replace)
            {
                foreach (var list in _bindingListXs[sender].Cast<IList<T>>())
                    list[e.Index] = e.NewItem!;
            }
            else if (e.Action is ListChangeAction.Clear)
            {
                foreach (var list in _bindingListXs[sender].Cast<IList<T>>())
                    list.Clear();
            }
        }
    }

    /// <summary>
    /// (INotifyCollectionChanged, ILists)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingLists = [];

    /// <summary>
    /// 绑定列表, 将源列表的修改同步至目标列表
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceCollection">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingList<T>(
        this INotifyCollectionChanged sourceCollection,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        if (_bindingLists.TryGetValue(sourceCollection, out var set) is false)
        {
            set = _bindingLists[sourceCollection] = new();
            sourceCollection.CollectionChanged += SourceList_CollectionChanged;
        }
        if (unBinding)
        {
            set.Remove(targetList);
            if (set.Count == 0)
            {
                _bindingLists.Remove(sourceCollection);
                sourceCollection.CollectionChanged -= SourceList_CollectionChanged;
            }
            return;
        }

        set.Add(targetList);

        static void SourceList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems.Cast<T>())
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list.Insert(e.NewStartingIndex, item);
                    }
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems is not null)
                {
                    for (var i = e.OldStartingIndex; i > e.OldStartingIndex - e.OldItems.Count; i--)
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list.RemoveAt(i);
                    }
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems is not null)
                {
                    var index = e.NewStartingIndex;
                    foreach (var item in e.NewItems.Cast<T>())
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list[index] = item;
                        index++;
                    }
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                    list.Clear();
            }
        }
    }
    #endregion

    #region BindingDictionary
    /// <summary>
    /// (INotifyDictionaryChanged, IDictionarys)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingDictionaryXs = [];

    /// <summary>
    /// 绑定字典, 将源字典的修改同步至目标字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDictionary">源字典</param>
    /// <param name="targetDictionary">目标字典</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingDictionaryX<TKey, TValue>(
        this INotifyDictionaryChanged<TKey, TValue> sourceDictionary,
        IDictionary<TKey, TValue> targetDictionary,
        bool unBinding = false
    )
        where TKey : notnull
    {
        if (_bindingDictionaryXs.TryGetValue(sourceDictionary, out var set) is false)
        {
            set = _bindingDictionaryXs[sourceDictionary] = new();
            sourceDictionary.DictionaryChanged += SourceDictionary_DictionaryChanged;
        }
        if (unBinding)
        {
            set.Remove(targetDictionary);
            if (set.Count == 0)
            {
                _bindingDictionaryXs.Remove(sourceDictionary);
                sourceDictionary.DictionaryChanged -= SourceDictionary_DictionaryChanged;
            }
            return;
        }

        set.Add(targetDictionary);

        static void SourceDictionary_DictionaryChanged(
            INotifyDictionaryChanged<TKey, TValue> sender,
            NotifyDictionaryChangeEventArgs<TKey, TValue> e
        )
        {
            if (e.Action is DictionaryChangeAction.Add)
            {
                if (e.TryGetNewPair(out var newPair))
                {
                    foreach (
                        var dictionary in _bindingDictionaryXs[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Add(newPair);
                }
            }
            else if (e.Action is DictionaryChangeAction.Remove)
            {
                if (e.TryGetOldPair(out var oldPair))
                {
                    foreach (
                        var dictionary in _bindingDictionaryXs[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Remove(oldPair);
                }
            }
            else if (e.Action is DictionaryChangeAction.Replace)
            {
                if (e.TryGetNewPair(out var newPair))
                {
                    foreach (
                        var dictionary in _bindingDictionaryXs[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary[newPair.Key] = newPair.Value;
                }
            }
            else if (e.Action is DictionaryChangeAction.Clear)
            {
                foreach (
                    var dictionary in _bindingDictionaryXs[sender].Cast<IDictionary<TKey, TValue>>()
                )
                    dictionary.Clear();
            }
        }
    }

    /// <summary>
    /// (INotifyCollectionChanged, IDictionarys)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingDictionarys = [];

    /// <summary>
    /// 绑定字典, 将源字典的修改同步至目标字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDictionary">源字典</param>
    /// <param name="targetDictionary">目标字典</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingDictionary<TKey, TValue>(
        this INotifyCollectionChanged sourceDictionary,
        IDictionary<TKey, TValue> targetDictionary,
        bool unBinding = false
    )
        where TKey : notnull
    {
        if (_bindingDictionarys.TryGetValue(sourceDictionary, out var set) is false)
        {
            set = _bindingDictionarys[sourceDictionary] = new();
            sourceDictionary.CollectionChanged += SourceDictionary_CollectionChanged;
        }
        if (unBinding)
        {
            set.Remove(targetDictionary);
            if (set.Count == 0)
            {
                _bindingDictionarys.Remove(sourceDictionary);
                sourceDictionary.CollectionChanged -= SourceDictionary_CollectionChanged;
            }
            return;
        }

        set.Add(targetDictionary);

        static void SourceDictionary_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e
        )
        {
            ArgumentNullException.ThrowIfNull(sender);
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems!.Cast<KeyValuePair<TKey, TValue>>())
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Add(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems!.Cast<KeyValuePair<TKey, TValue>>())
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Remove(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems!.Cast<KeyValuePair<TKey, TValue>>())
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary[item.Key] = item.Value;
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                foreach (
                    var dictionary in _bindingDictionarys[sender].Cast<IDictionary<TKey, TValue>>()
                )
                    dictionary.Clear();
            }
        }
    }
    #endregion

    #region BindingSet
    /// <summary>
    /// (INotifySetChanged, ISets)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingSetXs = [];

    /// <summary>
    /// 绑定集合, 将源集合的修改同步至目标集合
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingSetX<T>(
        this INotifySetChanged<T> sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        if (_bindingSetXs.TryGetValue(sourceSet, out var set) is false)
        {
            set = _bindingSetXs[sourceSet] = new();
            sourceSet.SetChanged += SourceSet_SetChanged;
        }
        if (unBinding)
        {
            set.Remove(targetSet);
            if (set.Count == 0)
            {
                _bindingSetXs.Remove(sourceSet);
                sourceSet.SetChanged -= SourceSet_SetChanged;
            }
            return;
        }

        set.Add(targetSet);

        static void SourceSet_SetChanged(INotifySetChanged<T> sender, NotifySetChangeEventArgs<T> e)
        {
            if (e.Action is SetChangeAction.Add)
            {
                ArgumentNullException.ThrowIfNull(e.NewItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                {
                    foreach (var item in e.NewItems)
                        set.Add(item);
                }
            }
            else if (e.Action is SetChangeAction.Remove)
            {
                ArgumentNullException.ThrowIfNull(e.OldItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                {
                    foreach (var item in e.OldItems)
                        set.Remove(item);
                }
            }
            else if (e.Action is SetChangeAction.Clear)
            {
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                    set.Clear();
            }
            else if (e.Action is SetChangeAction.Union)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                {
                    if (set is HashSet<T> hashSet)
                        hashSet.TrimExcess();
                    set.UnionWith(e.OtherItems);
                }
            }
            else if (e.Action is SetChangeAction.Except)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                    set.ExceptWith(e.OtherItems);
            }
            else if (e.Action is SetChangeAction.Intersect)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                    set.IntersectWith(e.OtherItems);
            }
            else if (e.Action is SetChangeAction.SymmetricExcept)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems);
                foreach (var set in _bindingSetXs[sender].Cast<ISet<T>>())
                    set.SymmetricExceptWith(e.OtherItems);
            }
        }
    }

    /// <summary>
    /// (INotifyCollectionChanged, ISets)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingSets = [];

    /// <summary>
    /// 绑定集合, 将源集合的修改同步至目标集合
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingSet<T>(
        this INotifyCollectionChanged sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        if (_bindingSets.TryGetValue(sourceSet, out var set) is false)
        {
            set = _bindingSets[sourceSet] = new();
            sourceSet.CollectionChanged += SourceSet_CollectionChanged;
        }
        if (unBinding)
        {
            set.Remove(targetSet);
            if (set.Count == 0)
            {
                _bindingSets.Remove(sourceSet);
                sourceSet.CollectionChanged -= SourceSet_CollectionChanged;
            }
            return;
        }

        set.Add(targetSet);

        static void SourceSet_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                {
                    if (set is HashSet<T> hashSet)
                        hashSet.TrimExcess();
                    foreach (var item in e.NewItems!.Cast<T>())
                        set.Add(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems!.Cast<T>())
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.Remove(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.Clear();
            }
        }
    }

    #endregion
}
#pragma warning restore S3776
