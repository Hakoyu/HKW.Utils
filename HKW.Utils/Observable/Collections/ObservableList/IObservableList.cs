﻿using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableList<T>
    : IList<T>,
        IObservableCollection<T>,
        INotifyListChanging<T>,
        INotifyListChanged<T> { }
