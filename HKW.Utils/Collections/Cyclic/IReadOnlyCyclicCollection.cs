namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读循环集合接口
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
public interface IReadOnlyCyclicCollection<T> : IReadOnlyCollection<T>, ICyclic<T> { }
