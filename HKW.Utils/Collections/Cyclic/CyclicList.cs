using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环列表
/// <para>任何修改列表的行为会导致循环重置</para>
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
public class CyclicList<T> : IListX<T>
{
    private readonly List<T> _list;

    #region ctor
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

    #region Cyclic
    /// <summary>
    /// 当前项目
    /// </summary>
    public T Current { get; private set; } = default!;

    /// <summary>
    /// 当前索引
    /// </summary>
    public int CurrntIndex { get; private set; } = 0;

    /// <summary>
    /// 自动重置
    /// </summary>
    [DefaultValue(false)]
    public bool AutoReset { get; set; } = false;

    /// <summary>
    /// 移动到下一个
    /// </summary>
    /// <returns>移动成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool MoveNext()
    {
        if (CurrntIndex >= _list.Count)
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

    /// <summary>
    /// 重置循环
    /// </summary>
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
        set => ((IList<T>)_list)[index] = value;
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_list).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

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
}
