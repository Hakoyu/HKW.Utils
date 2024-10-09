using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观测集合接口
/// </summary>
public interface IReadOnlyObservableCollection<T>
    : IReadOnlyCollection<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
