namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集合包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
public interface ISetWrapper<TItem, TSet> : ICollectionWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <summary>
    /// 原始集合
    /// </summary>
    public TSet SourceSet { get; }
}
