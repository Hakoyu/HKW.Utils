using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableList<T>
    : IList<T>,
        IList,
        IReadOnlyList<T>,
        INotifyListChanging<T>,
        INotifyListChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
