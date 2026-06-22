using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils;

/// <summary>
/// 可撤销列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class UndoableListWrapper<TItem, TList>
    : IList<TItem>,
        IUndoableCollection<TItem>,
        IRedoableCollection<TItem>,
        IListWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    public UndoableListWrapper(TList list)
    {
        SourceList = list;
    }

    /// <summary>
    /// 基础列表
    /// </summary>
    public TList SourceList { get; }

    /// <summary>
    /// 撤销栈
    /// </summary>
    public Stack<TItem> UndoStack { get; } = new();

    #region IList
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => SourceList[index];
        set => SourceList[index] = value;
    }

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    /// <summary>
    /// 添加项
    /// <para>
    /// 此操作会清空 <see cref="UndoStack"/>
    /// </para>
    /// </summary>
    public void Add(TItem item)
    {
        SourceList.Add(item);
        UndoStack.Clear();
    }

    /// <summary>
    /// 清空列表
    /// <para>
    /// 此操作会清空 <see cref="UndoStack"/>
    /// </para>
    /// </summary>
    public void Clear()
    {
        SourceList.Clear();
        UndoStack.Clear();
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
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        SourceList.Insert(index, item);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        return SourceList.Remove(item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        SourceList.RemoveAt(index);
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
    public TItem Undo()
    {
        var i = SourceList.Count - 1;
        var item = SourceList[i];
        SourceList.RemoveAt(i);
        UndoStack.Push(item);
        return item;
    }

    /// <inheritdoc/>
    public bool Undo(int count)
    {
        if (Count - count < 0 || count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        if (Count == 0)
            return false;
        var temp = Count - count;
        for (var i = Count - 1; i >= temp; i--)
        {
            var item = SourceList[i];
            SourceList.RemoveAt(i);
            UndoStack.Push(item);
        }
        return true;
    }

    object IUndoableCollection.Undo()
    {
        return Undo()!;
    }
    #endregion
    #region Redo
    /// <inheritdoc/>
    public TItem Redo()
    {
        var item = UndoStack.Pop();
        SourceList.Add(item);
        return item;
    }

    /// <inheritdoc/>
    public bool Redo(int count)
    {
        if (count > UndoStack.Count || count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        if (count == 0)
            return false;
        for (var i = 0; i < count; i++)
        {
            SourceList.Add(UndoStack.Pop());
        }
        return true;
    }

    object IRedoableCollection.Redo()
    {
        return Redo()!;
    }
    #endregion
}
