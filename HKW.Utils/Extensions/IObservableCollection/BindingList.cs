using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// (INotifyListChanged, IList)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingLists = new();

    /// <summary>
    /// 绑定列表
    /// <para>
    /// 将源列表的修改同步至目标列表
    /// </para>
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="sourceList">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingList<T>(
        this INotifyListChanged<T> sourceList,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        if (unBinding)
        {
            sourceList.ListChanged -= SourceList_ListChanged;
            if (_bindingLists.TryGetValue(sourceList, out var tlists))
                tlists.Remove(targetList);
            return;
        }
        sourceList.ListChanged -= SourceList_ListChanged;
        sourceList.ListChanged += SourceList_ListChanged;
        _bindingLists.GetOrCreate(sourceList).Add(targetList);

        static void SourceList_ListChanged(
            INotifyListChanged<T> sender,
            NotifyListChangeEventArgs<T> e
        )
        {
            if (e.Action is ListChangeAction.Add)
            {
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems)
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list.Insert(e.Index, item);
                    }
                }
            }
            else if (e.Action is ListChangeAction.Remove)
            {
                if (e.OldItems is not null)
                {
                    for (var i = e.Index; i > e.Index - e.OldItems.Count; i--)
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list.RemoveAt(i);
                    }
                }
            }
            else if (e.Action is ListChangeAction.Replace)
            {
                if (e.NewItems is not null)
                {
                    var index = e.Index;
                    foreach (var item in e.NewItems)
                    {
                        foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                            list[index] = item;
                        index++;
                    }
                }
            }
            else if (e.Action is ListChangeAction.Clear)
            {
                foreach (var list in _bindingLists[sender].Cast<IList<T>>())
                    list.Clear();
            }
        }
    }

    /// <summary>
    /// (INotifyCollectionChanged, IList)
    /// </summary>
    private static Dictionary<INotifyCollectionChanged, HashSet<object>> _bindingCollections =
        new();

    /// <summary>
    /// 绑定列表
    /// <para>
    /// 将源列表的修改同步至目标列表
    /// </para>
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="sourceCollection">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingList<T>(
        this INotifyCollectionChanged sourceCollection,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        if (unBinding)
        {
            sourceCollection.CollectionChanged -= SourceList_CollectionChanged;
            if (_bindingLists.TryGetValue(sourceCollection, out var tlists))
                tlists.Remove(targetList);
            return;
        }
        sourceCollection.CollectionChanged -= SourceList_CollectionChanged;
        sourceCollection.CollectionChanged += SourceList_CollectionChanged;
        _bindingLists.GetOrCreate(sourceCollection).Add(targetList);

        static void SourceList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is not INotifyCollectionChanged senderCollection)
                return;
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems.Cast<T>())
                    {
                        foreach (var list in _bindingLists[senderCollection].Cast<IList<T>>())
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
                        foreach (var list in _bindingLists[senderCollection].Cast<IList<T>>())
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
                        foreach (var list in _bindingLists[senderCollection].Cast<IList<T>>())
                            list[index] = item;
                        index++;
                    }
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                foreach (var list in _bindingLists[senderCollection].Cast<IList<T>>())
                    list.Clear();
            }
        }
    }
}
