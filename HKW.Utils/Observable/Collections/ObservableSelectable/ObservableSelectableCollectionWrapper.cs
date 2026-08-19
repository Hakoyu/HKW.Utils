using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TCollection">集合类型</typeparam>
[DebuggerDisplay("Count = {SourceCollection.Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableSelectableCollectionWrapper<TItem, TCollection>
    : ICollectionWrapper<TItem, TCollection>,
        IDisposable,
        INotifyPropertyChanged
    where TCollection : ICollection<TItem>, INotifyCollectionChanged
{
    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSelectableCollectionWrapper(TCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        SourceCollection = collection;
        collection.CollectionChanged += Collection_CollectionChanged;
    }

    private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (HasSelection is false)
            return;

        if (e.Action is NotifyCollectionChangedAction.Replace)
        {
            UpdateSelectedItem(e.OldItems, e.NewItems);
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems?.Contains(SelectedItem) is true)
                ClearSelection();
        }
        else if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            ClearSelection();
        }
    }

    /// <inheritdoc/>
    public TCollection SourceCollection { get; }

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection { get; private set; }

    private TItem _selectedItem = default!;

    /// <summary>
    /// 选中的项目
    /// </summary>
#pragma warning disable S4275
    public TItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SourceCollection.Contains(value) is false)
            {
                ClearSelection();
                return;
            }

            SetSelectedItem(value);
        }
    }
#pragma warning restore S4275

    private void SetSelectedItem(TItem item)
    {
        if (HasSelection && EqualityComparer<TItem>.Default.Equals(_selectedItem, item))
            return;

        _selectedItem = item;
        HasSelection = true;
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(HasSelection));
    }

    private void UpdateSelectedItem(IList? oldItems, IList? newItems)
    {
        if (oldItems is null || newItems is null)
            return;

        var index = oldItems.IndexOf(SelectedItem);
        if (index < 0 || index >= newItems.Count)
            return;

        _selectedItem = (TItem)newItems[index]!;
        OnPropertyChanged(nameof(SelectedItem));
    }

    private void ClearSelection()
    {
        if (
            HasSelection is false
            && EqualityComparer<TItem>.Default.Equals(_selectedItem, default!)
        )
        {
            return;
        }

        _selectedItem = default!;
        HasSelection = false;
        OnPropertyChanged(nameof(SelectedItem));
        OnPropertyChanged(nameof(HasSelection));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Dispose
    private bool _disposed;

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
            ClearSelection();
            SourceCollection.CollectionChanged -= Collection_CollectionChanged;
        }

        _disposed = true;
    }
    #endregion
}
