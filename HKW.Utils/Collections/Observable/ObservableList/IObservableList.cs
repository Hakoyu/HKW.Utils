using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableList<T>
    : IList<T>,
        IObservableCollection<T>,
        INotifyListChanging<T>,
        INotifyListChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
