using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环列表
/// <para>任何修改列表数量的行为会导致循环重置</para>
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class CyclicList<T>
    : IListRange<T>,
        IListFind<T>,
        ICyclicCollection<T>,
        IReadOnlyList<T>,
        IList
{
    private readonly List<T> _list;

    #region Ctor
    /// <inheritdoc/>
    public CyclicList()
        : this(null, null) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public CyclicList(int capacity)
        : this(capacity, null) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public CyclicList(IEnumerable<T> collection)
        : this(null, collection) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="collection">集合</param>
    private CyclicList(int? capacity, IEnumerable<T>? collection)
    {
        if (capacity is not null)
            _list = new(capacity.Value);
        else if (collection is not null)
            _list = new(collection);
        else
            _list = new();
    }
    #endregion

    /// <summary>
    /// 当前索引
    /// </summary>
    public int CurrntIndex { get; private set; } = 0;

    #region ICyclicCollection
    /// <inheritdoc/>
    public T Current { get; private set; } = default!;

    /// <inheritdoc/>
    public bool AutoReset { get; set; }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (CurrntIndex >= _list.Count - 1)
        {
            if (AutoReset)
            {
                CurrntIndex = 0;
                Current = _list[CurrntIndex];
                return true;
            }
            return false;
        }
        CurrntIndex++;
        Current = _list[CurrntIndex];
        return true;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        if (Count == 0)
        {
            CurrntIndex = -1;
            Current = default!;
        }
        else
        {
            CurrntIndex = 0;
            Current = _list[CurrntIndex];
        }
    }
    #endregion

    #region IList
    /// <inheritdoc/>
    public T this[int index]
    {
        get => ((IList<T>)_list)[index];
        set
        {
            ((IList<T>)_list)[index] = value;
            if (index == CurrntIndex)
                Current = value;
        }
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_list).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)_list).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_list).SyncRoot;

    object? IList.this[int index]
    {
        get => ((IList)_list)[index];
        set => ((IList)_list)[index] = value;
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        ((ICollection<T>)_list).Add(item);
        Reset();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)_list).Clear();
        Reset();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_list).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_list).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_list).GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return ((IList<T>)_list).IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        ((IList<T>)_list).Insert(index, item);
        Reset();
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var result = ((ICollection<T>)_list).Remove(item);
        Reset();
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        ((IList<T>)_list).RemoveAt(index);
        Reset();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
    #endregion

    #region IListX
    /// <inheritdoc/>
    public void AddRange(IEnumerable<T> collection)
    {
        _list.AddRange(collection);
        Reset();
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        _list.InsertRange(index, collection);
        Reset();
    }

    /// <inheritdoc/>
    public void RemoveAll(Predicate<T> match)
    {
        _list.RemoveAll(match);
        Reset();
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        _list.RemoveRange(index, count);
        Reset();
    }

    /// <inheritdoc/>
    public void Reverse()
    {
        _list.Reverse();
        Reset();
    }

    /// <inheritdoc/>
    public void Reverse(int index, int count)
    {
        _list.Reverse(index, count);
        Reset();
    }
    #endregion

    #region IListFind
    /// <inheritdoc/>
    public T? Find(Predicate<T> match)
    {
        return _list.Find(match);
    }

    /// <inheritdoc/>
    public (int Index, T? Value) Find(int startIndex, Predicate<T> match)
    {
        var index = _list.FindIndex(startIndex, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public (int Index, T? Value) Find(int startIndex, int count, Predicate<T> match)
    {
        var index = _list.FindIndex(startIndex, count, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public bool TryFind(Predicate<T> match, [MaybeNullWhen(false)] out T item)
    {
        var index = _list.FindIndex(match);
        item = _list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFind(int startIndex, Predicate<T> match, out (int Index, T Value) item)
    {
        var index = _list.FindIndex(startIndex, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFind(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    )
    {
        var index = _list.FindIndex(startIndex, count, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public int FindIndex(Predicate<T> match)
    {
        return _list.FindIndex(match);
    }

    /// <inheritdoc/>
    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, match);
    }

    /// <inheritdoc/>
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, count, match);
    }

    /// <inheritdoc/>
    public T? FindLast(Predicate<T> match)
    {
        return _list.FindLast(match);
    }

    /// <inheritdoc/>
    public (int Index, T? Value) FindLast(int startIndex, Predicate<T> match)
    {
        var index = _list.FindLastIndex(startIndex, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public (int Index, T? Value) FindLast(int startIndex, int count, Predicate<T> match)
    {
        var index = _list.FindLastIndex(startIndex, count, match);
        return (index, _list.GetValueOrDefault(index));
    }

    /// <inheritdoc/>
    public bool TryFindLast(Predicate<T> match, [MaybeNullWhen(false)] out T item)
    {
        var index = _list.FindLastIndex(match);
        item = _list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFindLast(int startIndex, Predicate<T> match, out (int Index, T Value) item)
    {
        var index = _list.FindLastIndex(startIndex, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public bool TryFindLast(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    )
    {
        var index = _list.FindLastIndex(startIndex, count, match);
        item = (index, _list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <inheritdoc/>
    public int FindLastIndex(Predicate<T> match)
    {
        return _list.FindLastIndex(match);
    }

    /// <inheritdoc/>
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, match);
    }

    /// <inheritdoc/>
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, count, match);
    }

    /// <inheritdoc/>
    public int Add(object? value)
    {
        var result = ((IList)_list).Add(value);
        Reset();
        return result;
    }

    /// <inheritdoc/>
    public bool Contains(object? value)
    {
        return ((IList)_list).Contains(value);
    }

    /// <inheritdoc/>
    public int IndexOf(object? value)
    {
        return ((IList)_list).IndexOf(value);
    }

    /// <inheritdoc/>
    public void Insert(int index, object? value)
    {
        ((IList)_list).Insert(index, value);
        Reset();
    }

    /// <inheritdoc/>
    public void Remove(object? value)
    {
        ((IList)_list).Remove(value);
        Reset();
    }

    /// <inheritdoc/>
    public void CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }
    #endregion
}
