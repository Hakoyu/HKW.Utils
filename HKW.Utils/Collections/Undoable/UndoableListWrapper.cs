using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可撤销列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class UndoableListWrapper<TItem, TList>
    : IList<TItem>,
        IUndoableCollection<TItem>,
        IRedoableCollection<TItem>
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    public UndoableListWrapper(TList list)
    {
        ArgumentNullException.ThrowIfNull(list);
        SourceList = list;
    }

    /// <summary>
    /// 基础列表
    /// </summary>
    protected TList SourceList { get; }

    private readonly Stack<TItem> _undoStack = new();

    /// <summary>
    /// 撤销栈, 保存可供重做的尾项, 枚举顺序为恢复顺序
    /// </summary>
    public ReadOnlyStack<TItem> UndoStack => field ??= new(_undoStack);

    #region IList
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => SourceList[index];
        set
        {
            SourceList[index] = value;
            _undoStack.Clear();
        }
    }

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    /// <inheritdoc/>
    /// <remarks>
    /// 此操作会清空 <see cref="_undoStack"/>
    /// </remarks>
    public void Add(TItem item)
    {
        SourceList.Add(item);
        _undoStack.Clear();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 此操作会清空 <see cref="_undoStack"/>
    /// </remarks>
    public void Insert(int index, TItem item)
    {
        SourceList.Insert(index, item);
        _undoStack.Clear();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 此操作会清空 <see cref="_undoStack"/>
    /// </remarks>
    public bool Remove(TItem item)
    {
        var result = SourceList.Remove(item);
        if (result)
            _undoStack.Clear();
        return result;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 此操作会清空 <see cref="_undoStack"/>
    /// </remarks>
    public void RemoveAt(int index)
    {
        SourceList.RemoveAt(index);
        _undoStack.Clear();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 此操作会清空 <see cref="_undoStack"/>
    /// </remarks>
    public void Clear()
    {
        SourceList.Clear();
        _undoStack.Clear();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }

    #endregion
    #region Undo
    /// <inheritdoc/>
    public bool Undo() => UndoCount(1);

    /// <inheritdoc/>
    public bool UndoCount(int count)
    {
        var sourceCount = SourceList.Count;
        if (count <= 0 || count > sourceCount)
            return false;
        // 移除源项前预留容量, 避免恢复栈扩容失败后丢失该项.
        _undoStack.EnsureCapacity(checked(_undoStack.Count + count));
        var endIndex = sourceCount - count;
        for (var i = sourceCount - 1; i >= endIndex; i--)
        {
            var item = SourceList[i];
            SourceList.RemoveAt(i);
            _undoStack.Push(item);
        }
        return true;
    }

    /// <inheritdoc/>
    public bool UndoThroughItem(TItem item)
    {
        var index = SourceList.LastIndexOf(item);
        return index >= 0 && UndoCount(SourceList.Count - index);
    }
    #endregion
    #region Redo
    /// <inheritdoc/>
    public bool Redo()
    {
        if (_undoStack.Count == 0)
            return false;
        RedoCore();
        return true;
    }

    /// <inheritdoc/>
    public bool RedoCount(int count)
    {
        if (count <= 0 || count > _undoStack.Count)
            return false;
        for (var i = 0; i < count; i++)
            RedoCore();
        return true;
    }

    /// <inheritdoc/>
    public bool RedoThroughItem(TItem item)
    {
        var index = _undoStack.IndexOf(item);
        return index >= 0 && RedoCount(index + 1);
    }

    private void RedoCore()
    {
        var item = _undoStack.Peek();
        SourceList.Add(item);
        _undoStack.Pop();
    }
    #endregion
}
