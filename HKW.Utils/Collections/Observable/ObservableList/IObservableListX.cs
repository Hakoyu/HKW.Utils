namespace HKW.HKWUtils.Collections;

/// <summary>
/// 高级可观测列表接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface IObservableListX<T> : IObservableList<T>
{
    /// <summary>
    /// 添加多个项目
    /// </summary>
    /// <param name="items">项目</param>
    public void AddRange(IEnumerable<T> items);

    /// <summary>
    /// 范围插入
    /// </summary>
    /// <param name="index">起始位置</param>
    /// <param name="collection">集合</param>
    public void InsertRange(int index, IEnumerable<T> collection);

    /// <summary>
    /// 范围删除
    /// </summary>
    /// <param name="index">起始位置</param>
    /// <param name="count">数量</param>
    public void RemoveRange(int index, int count);

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
    /// <param name="index">起始位置</param>
    public void ChangeRange(IEnumerable<T> collection, int index = 0);
}
