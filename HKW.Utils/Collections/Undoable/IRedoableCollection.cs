namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可重做集合接口
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public interface IRedoableCollection<in T>
{
    /// <summary>
    /// 恢复下一项.
    /// </summary>
    /// <returns>是否成功; 恢复栈为空时返回 false. </returns>
    public bool Redo();

    /// <summary>
    /// 按原顺序恢复指定数量的项.
    /// </summary>
    /// <param name="count">数量</param>
    /// <returns>是否成功; 数量非正或超出恢复项数时返回 false. </returns>
    public bool RedoCount(int count);

    /// <summary>
    /// 恢复至指定项在恢复栈枚举顺序中的第一次出现, 包含该项.
    /// </summary>
    /// <param name="item">使用默认相等比较器查找的项</param>
    /// <returns>是否成功; 未找到时返回 false. </returns>
    public bool RedoThroughItem(T item);
}
