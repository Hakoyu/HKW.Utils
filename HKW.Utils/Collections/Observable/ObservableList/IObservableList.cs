using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用可观测列表接口
/// </summary>
public interface IObservableList
    : IList,
        INotifyListChanging,
        INotifyListChanged,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
