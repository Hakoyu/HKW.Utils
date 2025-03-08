namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤集合
/// </summary>
/// <typeparam name="T">项类型</typeparam>
/// <typeparam name="TCollection">集合类型</typeparam>
/// <typeparam name="TFilteredCollection">已过滤集合类型</typeparam>
public interface IFilterCollection<T, TCollection, TFilteredCollection> : ICollection<T>
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
    /// 集合
    /// <para>使用此属性修改集合时不会同步至 <see cref="FilteredCollection"/></para>
    /// </summary>
    public TCollection BaseCollection { get; }

    /// <summary>
    /// 已过滤集合
    /// </summary>
    public TFilteredCollection FilteredCollection { get; }

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh();
}
