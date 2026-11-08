namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可撤销集合接口
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public interface IUndoableCollection<in T>
{
    /// <summary>
    /// 撤销最后一项.
    /// </summary>
    /// <returns>是否成功; 源为空时返回 false. </returns>
    public bool Undo();

    /// <summary>
    /// 从尾部撤销指定数量的项.
    /// </summary>
    /// <param name="count">数量</param>
    /// <returns>是否成功; 数量非正或超出源项数时返回 false. </returns>
    public bool UndoCount(int count);

    /// <summary>
    /// 从尾部撤销至指定项在源中的最后一次出现, 包含该项.
    /// </summary>
    /// <param name="item">使用默认相等比较器查找的项</param>
    /// <returns>是否成功; 未找到时返回 false. </returns>
    public bool UndoThroughItem(T item);
}
