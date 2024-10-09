namespace HKW.HKWUtils;

/// <summary>
/// 可重做集合接口
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public interface IRedoableCollection<T> : IRedoableCollection
{
    /// <summary>
    /// 重做
    /// </summary>
    /// <returns>重做的项</returns>
    new public T Redo();

    /// <summary>
    /// 重做指定数量
    /// </summary>
    /// <param name="count">数量</param>
    new public bool Redo(int count);

    ///// <summary>
    ///// 重做到指定项
    ///// </summary>
    ///// <param name="item">项</param>
    //public bool Redo(T item);
}

/// <summary>
/// 可重做集合接口
/// </summary>
public interface IRedoableCollection
{
    /// <summary>
    /// 重做
    /// </summary>
    /// <returns>重做的项</returns>
    public object Redo();

    /// <summary>
    /// 重做指定数量
    /// </summary>
    /// <param name="count">数量</param>
    public bool Redo(int count);

    ///// <summary>
    ///// 重做到指定项
    ///// </summary>
    ///// <param name="item">项</param>
    //public bool Redo(object item);
}
