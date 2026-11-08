using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <remarks>
/// 未归属的行可自由构造. 加入资源后, 结构及键由资源独占管理, 外部只能修改单元格.
/// 删除, 替换或释放资源后旧行保留数据且不再向资源转发通知. 同一行不能同时归属多个键或资源.
/// </remarks>
[DebuggerDisplay("Key = {Key}, Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ObservableCultureDataList<TKey, TValue>
    : ObservableListWrapper<TValue, List<TValue>>, IList<TValue>, System.Collections.IList
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key
    {
        get => _key;
        internal set
        {
            _key = value;
            OnPropertyChanged(PropertyChangedArgsCache.Key);
        }
    }

    private TKey _key = default!;
    internal ObservableI18nResource<TKey, TValue>? Owner { get; set; }
    internal ObservableI18nResource<TKey, TValue>? Reservation { get; set; }
    private ObservableI18nResource<TKey, TValue>.Mutation? _cellOperation;

    /// <inheritdoc/>
    public new TValue this[int index]
    {
        get => base[index];
        set
        {
            (Owner ?? Reservation)?.CheckWrite();
            if (Owner is { } owner)
                owner.SetData(Key, value, owner.GetCulture(index));
            else
                base[index] = value;
        }
    }

    /// <inheritdoc/>
    public new void Add(TValue value) { CheckStructure(); base.Add(value); }

    /// <inheritdoc/>
    public new void Insert(int index, TValue value) { CheckStructure(); base.Insert(index, value); }

    /// <inheritdoc/>
    public new bool Remove(TValue value) { CheckStructure(); return base.Remove(value); }

    /// <inheritdoc/>
    public new void RemoveAt(int index) { CheckStructure(); base.RemoveAt(index); }

    /// <inheritdoc/>
    public new void Clear() { CheckStructure(); base.Clear(); }

    object? System.Collections.IList.this[int index]
    {
        get => this[index];
        set => this[index] = CollectionInterfaceBridge.GetValue<TValue>(value, nameof(value));
    }

    bool System.Collections.IList.IsFixedSize => Owner is not null || Reservation is not null;
    int System.Collections.IList.Add(object? value)
    {
        CheckStructure();
        Add(CollectionInterfaceBridge.GetValue<TValue>(value, nameof(value)));
        return Count - 1;
    }

    void System.Collections.IList.Insert(int index, object? value)
    {
        CheckStructure();
        Insert(index, CollectionInterfaceBridge.GetValue<TValue>(value, nameof(value)));
    }

    void System.Collections.IList.Remove(object? value)
    {
        CheckStructure();
        if (CollectionInterfaceBridge.IsCompatible<TValue>(value))
            Remove((TValue)value!);
    }

    internal IDisposable Capture() => BeginOperation();
    internal void CommitCell(int index, TValue value) => SourceList[index] = value;
    internal void SetKeyCore(TKey key) => _key = key;
    internal void PublishKey() => OnPropertyChanged(PropertyChangedArgsCache.Key);
    internal void Commit(IReadOnlyList<TValue> values)
    {
        SourceList.Clear();
        SourceList.AddRange(values);
    }

    internal void Prepare(NotifyListChangeEventArgs<TValue> change) => base.OnListChanging(change);

    internal void Publish(NotifyListChangeEventArgs<TValue> change)
    {
        switch (change.Action)
        {
            case ListChangeAction.Add:
                base.OnListAdded(change, change.NewItem!, change.Index);
                break;
            case ListChangeAction.Remove:
                base.OnListRemoved(change, change.OldItem!, change.Index);
                break;
            case ListChangeAction.Replace:
                base.OnListReplaced(change, change.NewItem!, change.OldItem!, change.Index);
                break;
            case ListChangeAction.Clear:
                base.OnListCleared();
                break;
        }
    }

    private void CheckStructure()
    {
        if (Owner is not null || Reservation is not null)
            throw new InvalidOperationException("The resource exclusively owns the row structure.");
    }

    /// <inheritdoc/>
    protected override NotifyListChangeEventArgs<TValue>? OnListAdding(TValue item, int index)
    {
        CheckStructure();
        return base.OnListAdding(item, index);
    }

    /// <inheritdoc/>
    protected override NotifyListChangeEventArgs<TValue>? OnListRemoving(TValue item, int index)
    {
        CheckStructure();
        return base.OnListRemoving(item, index);
    }

    /// <inheritdoc/>
    protected override void OnListClearing()
    {
        CheckStructure();
        base.OnListClearing();
    }

    /// <inheritdoc/>
    protected override NotifyListChangeEventArgs<TValue>? OnListReplacing(TValue newItem, TValue oldItem, int index)
    {
        var operation = (Owner ?? Reservation)?.BeginMutation();
        try
        {
            var args = base.OnListReplacing(newItem, oldItem, index);
            _cellOperation = operation;
            return args;
        }
        catch
        {
            operation?.Dispose();
            throw;
        }
    }

    /// <inheritdoc/>
    protected override void OnListReplaced(NotifyListChangeEventArgs<TValue>? args, TValue newItem, TValue oldItem, int index)
    {
        var operation = _cellOperation;
        try
        {
            base.OnListReplaced(args, newItem, oldItem, index);
            operation?.PublishCell(Key, oldItem, newItem, index);
        }
        finally
        {
            _cellOperation = null;
            operation?.Dispose();
        }
    }

    /// <inheritdoc/>
    public ObservableCultureDataList(TKey key)
        : base(new())
    {
        ArgumentNullException.ThrowIfNull(key);
        Key = key;
    }

    /// <inheritdoc/>
    public ObservableCultureDataList(TKey key, IEnumerable<TValue> values)
        : base(new(values))
    {
        ArgumentNullException.ThrowIfNull(key);
        Key = key;
    }
}
