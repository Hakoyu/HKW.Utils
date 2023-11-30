namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读可观察集合接口
/// </summary>
public interface IReadOnlyObservableCollection<T> : IReadOnlyCollection<T>
{
    /// <summary>
    /// 关闭 (从原始可观察集合上注销所有事件)
    /// <para>
    /// !!!注意!!! 这将导致此只读可观察集合失效 (无法响应任何原始集合的事件)
    /// </para>
    /// </summary>
    public void Close();
}
