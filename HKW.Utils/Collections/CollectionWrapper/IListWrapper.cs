namespace HKW.HKWUtils;

/// <summary>
/// 列表包装器接口
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
public interface IListWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <summary>
    /// 基础列表
    /// </summary>
    public TList BaseList { get; }
}
