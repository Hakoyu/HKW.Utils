using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 高级可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableListX<T> : IObservableList<T>, IListRange<T> { }
