using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中集合包装器
/// </summary>
/// <typeparam name="T">项类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableSet<T> : ObservableSelectableSetWrapper<T, HashSet<T>>
{
    /// <inheritdoc/>
    public ObservableSelectableSet()
        : base([]) { }

    /// <inheritdoc/>
    /// <param name="items">项目</param>
    public ObservableSelectableSet(IEnumerable<T> items)
        : base(items.ToHashSet()) { }

    /// <inheritdoc/>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableSet(T seletedItem)
        : base([])
    {
        SelectedItem = seletedItem;
    }

    /// <inheritdoc/>
    /// <param name="items">项目</param>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableSet(IEnumerable<T> items, T seletedItem)
        : base(items.ToHashSet())
    {
        SelectedItem = seletedItem;
    }
}
