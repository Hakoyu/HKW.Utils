using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public partial class ObservableSelectableSet<T>
    : ObservableSelectableSetWrapper<T, OrderedHashSet<T>>
    where T : notnull
{
    /// <inheritdoc/>
    public ObservableSelectableSet()
        : base(new()) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
        : base(new(collection, comparer), comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableSet(int capacity, IEqualityComparer<T>? comparer = null)
        : base(new(capacity, comparer), comparer) { }

    /// <summary>
    /// 选中的索引
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (HasSelection is false)
                return -1;

            return SourceSet.IndexOf(SelectedItem);
        }
        set
        {
            if (value == -1)
            {
                ClearSelection();
                return;
            }

            ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(SourceSet, value);
            SelectedItem = SourceSet[value];
        }
    }

    /// <inheritdoc/>
    protected override void OnSelectionChanged()
    {
        OnPropertyChanged(nameof(SelectedIndex));
    }
}
