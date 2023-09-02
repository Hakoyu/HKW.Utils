using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读可观察集合
/// </summary>
public interface IReadOnlyObservableSet<T>
    : IReadOnlySet<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
