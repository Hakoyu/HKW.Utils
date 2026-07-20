using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可撤销列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class UndoableListWrapper<TItem, TList> : IList<TItem>, IListWrapper<TItem, TList>
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
    TList ICollectionWrapper<TItem, TList>.SourceCollection => SourceList;

    private readonly Stack<TItem> _undoStack = new();

    /// <summary>
    /// 撤销栈
    /// </summary>
    public ReadOnlyStack<TItem> UndoStack => field ??= new(_undoStack);

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
    public bool Undo()
    {
        if (SourceList.Count == 0)
            return false;
        var index = SourceList.Count - 1;
        var item = SourceList[index];
        SourceList.RemoveAt(index);
        _undoStack.Push(item);
        return true;
    }

    /// <inheritdoc/>
    public bool Undo(int count)
    {
        if (count <= 0 || count > SourceList.Count || SourceList.Count == 0)
            return false;
        var endIndex = SourceList.Count - count;
        for (var i = SourceList.Count - 1; i >= endIndex; i--)
        {
            var item = SourceList[i];
            SourceList.RemoveAt(i);
            _undoStack.Push(item);
        }
        return true;
    }

    /// <inheritdoc/>
    public bool Undo(TItem item)
    {
        var index = SourceList.LastIndexOf(item);
        if (index < 0)
            return false;
        for (var i = SourceList.Count - 1; i >= index; i--)
        {
            var tempItem = SourceList[i];
            SourceList.RemoveAt(i);
            _undoStack.Push(tempItem);
        }
        return true;
    }
    #endregion
    #region Redo
    /// <inheritdoc/>
    public bool Redo()
    {
        if (_undoStack.Count == 0)
            return false;
        var item = _undoStack.Pop();
        SourceList.Add(item);
        return true;
    }

    /// <inheritdoc/>
    public bool Redo(int count)
    {
        if (count <= 0 || count > _undoStack.Count || _undoStack.Count == 0)
            return false;
        for (var i = 0; i < count; i++)
            SourceList.Add(_undoStack.Pop());
        return true;
    }

    /// <inheritdoc/>
    public bool Redo(TItem item)
    {
        var count = _undoStack.IndexOf(item);
        if (count < 0)
            return false;
        count++;
        for (var i = 0; i < count; i++)
        {
            SourceList.Add(_undoStack.Pop());
        }
        return true;
    }
    #endregion
}
