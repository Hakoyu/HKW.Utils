using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// 自动过滤
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
    /// <param name="forcedRefresh">强制刷新, 非强制状态下会先比较初始列表和过滤列表是否相等</param>
    public void Refresh(bool forcedRefresh);
}
