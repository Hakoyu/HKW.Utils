using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察集合接口
/// </summary>
public interface IObservableSet<T>
    : ISet<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
{
    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }
}
