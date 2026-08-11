namespace HKW.HKWUtils.Collections;

/// <summary>
/// 列表包装器接口
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
public interface IListWrapper<TItem, TList> : ICollectionWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <summary>
    /// 原始列表
    /// </summary>
    protected TList SourceList { get; }
}
