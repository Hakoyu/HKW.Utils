using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观察集合
/// </summary>
public interface IReadOnlyObservableSet<T>
    : IReadOnlySet<T>,
        IReadOnlyObservableCollection<T>,
        INotifySetChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
