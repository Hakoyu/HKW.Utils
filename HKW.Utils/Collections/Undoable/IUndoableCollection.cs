namespace HKW.HKWUtils;

/// <summary>
/// 可撤销集合接口
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public interface IUndoableCollection<T> : IUndoableCollection
{
    /// <summary>
    /// 撤销
    /// </summary>
    /// <returns>撤销的项</returns>
    new public T Undo();

    /// <summary>
    /// 撤销指定数量
    /// </summary>
    /// <param name="count">数量</param>
    new public bool Undo(int count);

    ///// <summary>
    ///// 撤销指定项
    ///// </summary>
    ///// <param name="item">项</param>
    //public bool Undo(T item);
}

/// <summary>
/// 可撤销集合接口
/// </summary>
public interface IUndoableCollection
{
    /// <summary>
    /// 撤销
    /// </summary>
    /// <returns>撤销的项</returns>
    public object Undo();

    /// <summary>
    /// 撤销指定数量
    /// </summary>
    /// <param name="count">数量</param>
    public bool Undo(int count);

    ///// <summary>
    ///// 撤销指定项
    ///// </summary>
    ///// <param name="item">项</param>
    //public bool Undo(object item);
}
