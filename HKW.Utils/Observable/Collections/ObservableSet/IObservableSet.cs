using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察集合接口
/// </summary>
public interface IObservableSet<T>
    : ISet<T>,
        IObservableCollection<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T> { }
