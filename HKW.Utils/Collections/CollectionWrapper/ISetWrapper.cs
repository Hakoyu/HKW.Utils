namespace HKW.HKWUtils;

/// <summary>
/// 集合包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
public interface ISetWrapper<TItem, TSet>
    where TSet : ISet<TItem>
{
    /// <summary>
    /// 基础集合
    /// </summary>
    public TSet BaseSet { get; }
}
