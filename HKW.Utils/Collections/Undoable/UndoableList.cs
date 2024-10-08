namespace HKW.HKWUtils;

/// <summary>
/// 可撤销列表
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public class UndoableList<T> : UndoableListWrapper<T, List<T>>
{
    /// <inheritdoc/>
    public UndoableList()
        : base(new()) { }

    /// <inheritdoc/>
    public UndoableList(int capacity)
        : base(new List<T>(capacity)) { }

    /// <inheritdoc/>
    public UndoableList(IEnumerable<T> items)
        : base(new List<T>(items)) { }
}
