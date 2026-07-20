using System.Collections.Specialized;
using System.Diagnostics;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合包装器
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">集合类型</typeparam>
[DebuggerDisplay("Count = {SourceDictionary.Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public partial class ObservableSelectableDictionaryWrapper<TKey, TValue, TDictionary>
#pragma warning restore S2436
    : ReactiveObject,
        IDictionaryWrapper<TKey, TValue, TDictionary>,
        IDisposable
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>, INotifyCollectionChanged
{
    /// <inheritdoc/>
    /// <param name="dictionary">集合</param>
    public ObservableSelectableDictionaryWrapper(TDictionary dictionary)
    {
        SourceDictionary = dictionary;
        dictionary.CollectionChanged += Collection_CollectionChanged;
    }

    private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (HasSelection is false)
            return;

        if (e.Action is NotifyCollectionChangedAction.Replace)
        {
            if (
                e.OldItems?[0] is KeyValuePair<TKey, TValue> item
                && EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(SelectedItem, item)
            )
            {
                SelectedItem = (KeyValuePair<TKey, TValue>)e.NewItems?[0]!;
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
    public TDictionary SourceDictionary { get; }

    /// <inheritdoc/>
    TDictionary ICollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary>.SourceCollection =>
        SourceDictionary;

    /// <summary>
    /// 已选中
    /// </summary>
    public bool HasSelection { get; private set; } = false;

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
        set
        {
            if (EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_selectedItem, value))
                return;
            _selectedItem = value;
            if (SourceDictionary.Contains(value))
                HasSelection = true;
            else
                HasSelection = false;
            this.RaisePropertyChanged(nameof(SelectedItem));
            this.RaisePropertyChanged(nameof(SelectedKey));
            this.RaisePropertyChanged(nameof(SelectedValue));
            this.RaisePropertyChanged(nameof(HasSelection));
        }
    }

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableSelectableDictionaryWrapper() => Dispose(false);

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
            SourceDictionary.CollectionChanged -= Collection_CollectionChanged;
        }
        _disposed = true;
    }
    #endregion
}
