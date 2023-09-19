using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察只读列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IReadOnlyObservableList<T>
    : IReadOnlyList<T>,
        INotifyListChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
