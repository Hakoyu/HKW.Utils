namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读过滤集合接口
/// </summary>
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TFilteredCollection">已过滤集合类型</typeparam>
/// <typeparam name="TReadOnlyFilteredCollection">只读已过滤集合类型</typeparam>
public interface IReadOnlyFilterCollection<TItem, TFilteredCollection, TReadOnlyFilteredCollection>
    : ICollection<TItem>
    where TFilteredCollection : ICollection<TItem>
    where TReadOnlyFilteredCollection : IReadOnlyCollection<TItem>
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter { get; set; }

    /// <summary>
    /// 创建只读已过滤集合
    /// </summary>
    public Func<
        TFilteredCollection,
        TReadOnlyFilteredCollection
    > FilteredCollectionCreator { get; init; }

    /// <summary>
    /// 已过滤集合
    /// </summary>
    public TFilteredCollection FilteredCollection { get; }

    /// <summary>
    /// 只读已过滤集合
    /// </summary>
    public TReadOnlyFilteredCollection ReadOnlyFilteredCollection { get; }

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh();
}
