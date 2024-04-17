using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// (IObservableList, IList)
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
    /// <param name="UnBinding">解除绑定</param>
    public static void BindingList<T>(
        this IObservableList<T> sourceList,
        IList<T> targetList,
        bool UnBinding = false
    )
    {
        if (UnBinding)
        {
            sourceList.ListChanged -= SourceList_ListChanged;
            if (_bindingLists.TryGetValue(sourceList, out var tlists))
                tlists.Remove(targetList);
            return;
        }
        if (sourceList.SequenceEqual(targetList) is false)
        {
            throw new ArgumentException(
                "Source list and target list item sequences are not equal",
                nameof(targetList)
            );
        }
        sourceList.ListChanged -= SourceList_ListChanged;
        sourceList.ListChanged += SourceList_ListChanged;
        if (_bindingLists.TryGetValue(sourceList, out var lists) is false)
            lists = _bindingLists[sourceList] = new();
        lists.Add(targetList);

        static void SourceList_ListChanged(
            IObservableList<T> sender,
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
}
