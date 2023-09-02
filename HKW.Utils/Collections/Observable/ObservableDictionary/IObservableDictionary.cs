using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用可视化字典接口
/// </summary>
public interface IObservableDictionary
    : IDictionary,
        INotifyDictionaryChanging,
        INotifyDictionaryChanged,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
