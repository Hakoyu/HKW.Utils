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
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TFilteredCollection">已过滤集合类型</typeparam>
public interface IFilterCollection<TItem, TFilteredCollection> : ICollection<TItem>
    where TFilteredCollection : ICollection<TItem>
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter { get; set; }

    /// <summary>
    /// 已过滤集合
    /// </summary>
    public TFilteredCollection FilteredCollection { get; }

    /// <summary>
    /// 刷新过滤集合
    /// </summary>
    public void Refresh();
}
