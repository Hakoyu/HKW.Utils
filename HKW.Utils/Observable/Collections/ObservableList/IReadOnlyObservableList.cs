﻿using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测只读列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IReadOnlyObservableList<T>
    : IReadOnlyList<T>,
        IReadOnlyObservableCollection<T>,
        INotifyListChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
