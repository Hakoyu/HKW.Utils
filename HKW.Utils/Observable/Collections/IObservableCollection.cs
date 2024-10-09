using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测集合接口
/// </summary>
public interface IObservableCollection<T>
    : ICollection<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
