using System.Collections.Specialized;
using System.Diagnostics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TList">集合类型</typeparam>
[DebuggerDisplay("Count = {SourceList.Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public partial class ObservableSelectableListWrapper<TItem, TList>
    : ReactiveObject,
        IListWrapper<TItem, TList>,
        IDisposable
    where TList : IList<TItem>, INotifyCollectionChanged
{
    /// <inheritdoc/>
    /// <param name="list">集合</param>
    public ObservableSelectableListWrapper(TList list)
    {
        SourceList = list;
        list.CollectionChanged += Collection_CollectionChanged;
    }

    private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (HasSelection is false)
            return;

        if (e.Action is NotifyCollectionChangedAction.Add)
        {
            if (e.NewStartingIndex < SelectedIndex)
            {
                SelectedIndex += 1;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Replace)
        {
            if (
                e.OldItems?[0] is TItem item
                && EqualityComparer<TItem>.Default.Equals(SelectedItem, item)
            )
            {
                SelectedItem = (TItem)e.NewItems?[0]!;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems?.Contains(SelectedItem) is true)
            {
                SelectedItem = default!;
                SelectedIndex = -1;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            SelectedItem = default!;
            SelectedIndex = -1;
        }
    }

    /// <inheritdoc/>
    public TList SourceList { get; }

    TList ICollectionWrapper<TItem, TList>.SourceCollection => SourceList;

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection => SelectedIndex >= 0 && SelectedIndex < SourceList.Count;

    private int _selectedIndex = -1;

    /// <summary>
    /// 选中的索引
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value)
                return;
            this.RaisePropertyChanging(nameof(SelectedItem));
            this.RaisePropertyChanging(nameof(SelectedIndex));
            _selectedIndex = value;
            _selectedItem = SourceList[value];
            this.RaisePropertyChanged(nameof(SelectedItem));
            this.RaisePropertyChanged(nameof(SelectedIndex));
            this.RaisePropertyChanged(nameof(HasSelection));
        }
    }

    private TItem _selectedItem = default!;

    /// <summary>
    /// 选中的项目
    /// </summary>
    public TItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (EqualityComparer<TItem>.Default.Equals(_selectedItem, value))
                return;
            this.RaisePropertyChanging(nameof(SelectedItem));
            this.RaisePropertyChanging(nameof(SelectedIndex));
            _selectedItem = value;
            _selectedIndex = SourceList.IndexOf(value);
            this.RaisePropertyChanged(nameof(SelectedItem));
            this.RaisePropertyChanged(nameof(SelectedIndex));
            this.RaisePropertyChanged(nameof(HasSelection));
        }
    }

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableSelectableListWrapper() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            SelectedIndex = -1;
            SelectedItem = default!;
            SourceList.CollectionChanged -= Collection_CollectionChanged;
        }
        _disposed = true;
    }
    #endregion
}
