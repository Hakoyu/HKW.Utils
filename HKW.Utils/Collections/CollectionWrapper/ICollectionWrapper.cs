namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集合包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TCollection">集合类型</typeparam>
public interface ICollectionWrapper<TItem, TCollection>
    where TCollection : ICollection<TItem>
{
    /// <summary>
    /// 原始集合
    /// </summary>
    public TCollection SourceCollection { get; }
}
