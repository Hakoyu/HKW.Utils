using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤集合
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
/// <typeparam name="TFilteredSet">已过滤集合类型</typeparam>
/// <remarks>
/// 仅限单线程使用. 包装器写入及刷新期间拒绝重入写入, 普通实现不监听外部源或元素属性变化.
/// 刷新先计算完整结果, 谓词作用于实际源代表值. 目标比较器必须保留所有通过过滤的不同源元素,
/// 不兼容的比较器或后端提交失败会使 IsFilterValid 为 false, FilteredSet 读取及后续写入将抛错,
/// 必须成功 Refresh 才能恢复. 已持有的原始后端引用不受检查保护, 源读取仍可用于诊断.
/// 源和目标不得共享底层存储, 无法识别的外部包装别名由调用方保证. AutoFilter 关闭时冻结视图, Clear 除外.
/// BatchUpdate 即使取消或抛错也刷新并恢复开关, 更新和恢复均失败时用 AggregateException 保留两者.
/// 自定义后端, 比较器及谓词必须稳定且不得重入或直接修改源和目标. 不承诺任意外部后端的事务回滚.
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredSetWrapper<TItem, TSet, TFilteredSet>
#pragma warning restore S2436
    : ISet<TItem>,
        IReadOnlySet<TItem>,
        IFilteredCollectionWrapper<TItem, TSet, TFilteredSet>,
        ISetWrapper<TItem, TSet>,
        IFilterStorage
    where TSet : ISet<TItem>
    where TFilteredSet : ISet<TItem>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredSetWrapper(
        TSet set,
        TFilteredSet filteredSet,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(filteredSet);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(set);
        ArgumentException.ThrowIfReadOnlyCollection(filteredSet);

        FilterLifecycle.Validate(set, filteredSet);
        _filteredSet = filteredSet;
        _sourceSet = set;
        _filter = filter;
        _autoFilter = autoFilter;
        Refresh();
    }
    #endregion

    private readonly FilterLifecycle _lifecycle = new();
    private readonly TFilteredSet _filteredSet;
    private bool _autoFilter;

    /// <summary>
    /// 过滤视图是否有效. 失效后必须成功调用 Refresh 才能继续使用过滤视图.
    /// </summary>
    public bool IsFilterValid => _lifecycle.IsValid;

    /// <inheritdoc/>
    public bool AutoFilter
    {
        get => _autoFilter;
        set
        {
            using var operation = _lifecycle.Enter();
            _autoFilter = value;
        }
    }
    private Predicate<TItem> _filter;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => _filter;
        set
        {
            using var operation = _lifecycle.Enter();
            ArgumentNullException.ThrowIfNull(value);
            var result = Compute(_sourceSet, value);
            _filter = value;
            Publish(result);
        }
    }
    private TSet _sourceSet;
    object IFilterStorage.SourceStorage => _sourceSet;

    /// <inheritdoc/>
    public TSet SourceSet
    {
        get => _sourceSet;
        set
        {
            using var operation = _lifecycle.Enter();
            FilterLifecycle.Validate(value, _filteredSet, this);
            var result = Compute(value, _filter);
            var previous = _sourceSet;
            _sourceSet = value;
            OnSourceChanged(previous);
            Publish(result);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TSet ICollectionWrapper<TItem, TSet>.SourceCollection => SourceSet;

    /// <summary>
    /// 过滤完成的集合
    /// </summary>
    public TFilteredSet FilteredSet
    {
        get
        {
            _lifecycle.EnsureReadable();
            return _filteredSet;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredSet IFilteredCollectionWrapper<TItem, TSet, TFilteredSet>.FilteredCollection =>
        FilteredSet;

    /// <inheritdoc/>
    public virtual void BatchUpdate(Action<TSet> updateAction)
    {
        using var operation = _lifecycle.Enter();
        ArgumentNullException.ThrowIfNull(updateAction);
        var autoFilter = _autoFilter;
        _autoFilter = false;
        try
        {
            _lifecycle.Update(() => updateAction(_sourceSet), RefreshCore);
        }
        finally
        {
            _autoFilter = autoFilter;
        }
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        using var operation = _lifecycle.Enter(recovery: true);
        RefreshCore();
    }

    private static List<TItem> Compute(TSet source, Predicate<TItem> filter)
    {
        var result = new List<TItem>(source.Count);
        foreach (var item in source)
        {
            if (filter(item))
                result.Add(item);
        }
        return result;
    }

    private void RefreshCore()
    {
        List<TItem> result;
        try { result = Compute(_sourceSet, _filter); }
        catch { _lifecycle.IsValid = false; throw; }
        Publish(result);
    }

    private void Publish(List<TItem> result)
    {
        _lifecycle.IsValid = false;
        try
        {
            _lifecycle.EnsureSourceUnchanged();
            // 谓词只作用于源的实际代表值, 不作用于集合运算的输入代表值.
            if (Matches(result))
            {
                _lifecycle.IsValid = true;
                return;
            }
            _filteredSet.Clear();
            foreach (var item in result)
                _filteredSet.Add(item);
            _lifecycle.EnsureSourceUnchanged();
            if (Matches(result) is false)
                throw new InvalidOperationException("The filtered backend must preserve every source representative.");
            _lifecycle.IsValid = true;
        }
        catch
        {
            try { _lifecycle.IsValid = _lifecycle.SourceInvalidated is false && Matches(result); }
            catch { _lifecycle.IsValid = false; }
            throw;
        }
    }

    private bool Matches(List<TItem> result)
    {
        if (_filteredSet.Count != result.Count)
            return false;
        // 引用类型比较实际代表实例, 不能让目标比较器掩盖代表值变化.
        return new HashSet<TItem>(_filteredSet, FilterLifecycle.ItemComparer<TItem>.Instance).SetEquals(result);
    }

    private void Update(Action update, bool forceRefresh = false)
    {
        using var operation = _lifecycle.Enter();
        _lifecycle.Update(update, _autoFilter || forceRefresh ? RefreshCore : null);
    }

    private void UpdateSet(IEnumerable<TItem> other, Action<IEnumerable<TItem>> update)
    {
        using var operation = _lifecycle.Enter();
        ArgumentNullException.ThrowIfNull(other);
        var input = other.ToArray();
        _lifecycle.Update(() => update(input), _autoFilter ? RefreshCore : null);
    }

    /// <summary>
    /// 源切换完成后更新订阅.
    /// </summary>
    /// <param name="previous">先前的源</param>
    protected virtual void OnSourceChanged(TSet previous) { }

    /// <summary>
    /// 处理外部源通知. 包装器自身写入在操作结束时统一刷新.
    /// </summary>
    /// <param name="cleared">是否清空源</param>
    protected void SourceChanged(bool cleared)
    {
        if (_lifecycle.IsUpdatingSource)
            return;
        if (_lifecycle.IsActive)
        {
            _lifecycle.InvalidateSource();
            throw new InvalidOperationException("The source changed during filtered view publication.");
        }
        if (_autoFilter || cleared)
            Refresh();
    }

    /// <summary>
    /// 释放订阅并拒绝后续写入及过滤视图读取.
    /// </summary>
    /// <param name="unsubscribe">取消订阅操作</param>
    protected void DisposeFilter(Action unsubscribe) => _lifecycle.Dispose(unsubscribe);

    #region ISet
    /// <inheritdoc/>
    public int Count => SourceSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceSet.IsReadOnly;

    /// <inheritdoc/>
    public virtual bool Add(TItem item)
    {
        var result = false;
        Update(() => result = _sourceSet.Add(item));
        return result;
    }

    /// <inheritdoc/>
    public virtual bool Remove(TItem item)
    {
        var result = false;
        Update(() => result = _sourceSet.Remove(item));
        return result;
    }

    /// <inheritdoc/>
    public virtual void Clear() => Update(_sourceSet.Clear, forceRefresh: true);

    /// <inheritdoc/>
    public virtual void ExceptWith(IEnumerable<TItem> other) => UpdateSet(other, _sourceSet.ExceptWith);

    /// <inheritdoc/>
    public virtual void IntersectWith(IEnumerable<TItem> other) => UpdateSet(other, _sourceSet.IntersectWith);

    /// <inheritdoc/>
    public virtual void SymmetricExceptWith(IEnumerable<TItem> other) => UpdateSet(other, _sourceSet.SymmetricExceptWith);

    /// <inheritdoc/>
    public virtual void UnionWith(IEnumerable<TItem> other) => UpdateSet(other, _sourceSet.UnionWith);

    void ICollection<TItem>.Add(TItem item)
    {
        Add(item);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TItem> other)
    {
        return SourceSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TItem> other)
    {
        return SourceSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceSet.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TItem> other)
    {
        return SourceSet.SetEquals(other);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }
    #endregion
}
