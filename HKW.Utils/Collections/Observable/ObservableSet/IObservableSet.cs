using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察集合接口
/// </summary>
public interface IObservableSet<T>
    : ISet<T>,
        IReadOnlySet<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
{
    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<T>? Comparer { get; }
}
