using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TCollection">集合类型</typeparam>
public partial class ObservableSelectableCollectionWrapper<TItem, TCollection>
    : ReactiveObject,
        ICollectionWrapper<TItem, TCollection>,
        IDisposable
    where TCollection : ICollection<TItem>, INotifyCollectionChanged
{
    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSelectableCollectionWrapper(TCollection collection)
    {
        SourceCollection = collection;
        collection.CollectionChanged += Collection_CollectionChanged;
    }

    private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (HasSelection is false)
            return;

        if (e.Action is NotifyCollectionChangedAction.Replace)
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
                HasSelection = false;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            SelectedItem = default!;
            HasSelection = false;
        }
    }

    /// <inheritdoc/>
    public TCollection SourceCollection { get; }

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection { get; private set; } = false;

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
            _selectedItem = value;
            if (SourceCollection.Contains(value))
            {
                HasSelection = true;
            }
            else
            {
                HasSelection = false;
            }
            this.RaisePropertyChanged(nameof(SelectedItem));
            this.RaisePropertyChanged(nameof(HasSelection));
        }
    }

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableSelectableCollectionWrapper() => Dispose(false);

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
            HasSelection = false;
            SelectedItem = default!;
            SourceCollection.CollectionChanged -= Collection_CollectionChanged;
        }
        _disposed = true;
    }
    #endregion
}
