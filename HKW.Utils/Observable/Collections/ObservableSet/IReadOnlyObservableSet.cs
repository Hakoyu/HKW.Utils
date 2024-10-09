using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观测集合
/// </summary>
public interface IReadOnlyObservableSet<T>
    : IReadOnlySet<T>,
        IReadOnlyObservableCollection<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T> { }
