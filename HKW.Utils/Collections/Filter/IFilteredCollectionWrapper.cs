namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤集合
/// </summary>
/// <typeparam name="T">项类型</typeparam>
/// <typeparam name="TCollection">集合类型</typeparam>
/// <typeparam name="TFilteredCollection">已过滤集合类型</typeparam>
#pragma warning disable S2436
public interface IFilteredCollectionWrapper<T, TCollection, TFilteredCollection>
    : ICollection<T>,
        ICollectionWrapper<T, TCollection>
#pragma warning restore S2436
    where TCollection : ICollection<T>
    where TFilteredCollection : ICollection<T>
{
    /// <summary>
    /// 自动过滤, 为 <see langword="false"/> 时过滤列表将不会因主列表的变动而发生改变, <see cref="ICollection{T}.Clear"/> 除外
    /// </summary>
    public bool AutoFilter { get; set; }

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<T> Filter { get; set; }

    /// <summary>
    /// 已过滤集合
    /// </summary>
    public TFilteredCollection FilteredCollection { get; }

    /// <summary>
    /// 批量更新源列表。
    /// <para>执行期间会临时关闭 <see cref="AutoFilter"/>，执行结束后恢复原值并调用 <see cref="Refresh"/>。</para>
    /// </summary>
    /// <param name="updateAction">对 <see cref="ICollectionWrapper{TItem, TCollection}.SourceCollection"/> 的批量更新操作。</param>
    public void BatchUpdate(Action<TCollection> updateAction);

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh();
}
