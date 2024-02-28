using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 高级可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableListX<T> : IObservableList<T>, IListX<T>
{
    /// <summary>
    /// 范围改变,
    /// <para>示例:
    /// <code><![CDATA[
    /// list == {1, 2, 3}
    /// list.ChangeRange(list.Reverse<int>());
    /// list == {3, 2, 1}
    /// ]]></code></para>
    /// </summary>
    /// <param name="collection">集合</param>
    /// <param name="index">起始索引</param>
    public void ChangeRange(IEnumerable<T> collection, int index = 0);
}
