using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环集合
/// </summary>
public class CyclicSet<T> : ISet<T>, IReadOnlySet<T>, ICyclicCollection<T>
{
    private readonly HashSet<T> _set = new();

    #region Ctor
    /// <inheritdoc/>
    public CyclicSet()
        : this(null, null, null) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public CyclicSet(int capacity)
        : this(capacity, null, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public CyclicSet(IEqualityComparer<T> comparer)
        : this(null, null, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public CyclicSet(IEnumerable<T> collection)
        : this(null, collection, null) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public CyclicSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
        : this(null, collection, comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public CyclicSet(int capacity, IEqualityComparer<T>? comparer)
        : this(capacity, null, comparer) { }

    private CyclicSet(int? capacity, IEnumerable<T>? collection, IEqualityComparer<T>? comparer)
    {
        if (capacity is not null)
            _set = new(capacity.Value, comparer);
        if (collection is not null)
            _set = new(collection, comparer);
        else
            _set = new(comparer);
    }
    #endregion

    private readonly List<T> _list = new();
    private int _currentIndex = 0;

    #region ICyclicCollection
    /// <inheritdoc/>
    public T Current { get; private set; } = default!;

    /// <inheritdoc/>
    public bool AutoReset { get; set; } = false;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (_currentIndex >= _list.Count - 1)
        {
            if (AutoReset)
            {
                _currentIndex = 0;
                Current = _list[_currentIndex];
                return true;
            }
            return false;
        }
        _currentIndex++;
        Current = _list[_currentIndex];
        return true;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        if (Count == 0)
        {
            _currentIndex = -1;
            Current = default!;
        }
        else
        {
            _currentIndex = 0;
            Current = _list[_currentIndex];
        }
    }

    private void Refresh()
    {
        _list.Clear();
        _list.AddRange(_list);
        Reset();
    }
    #endregion

    #region ISet
    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_set).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

    /// <inheritdoc/>
    public bool Add(T item)
    {
        var result = ((ISet<T>)_set).Add(item);
        if (result)
        {
            _list.Add(item);
            Reset();
        }
        return result;
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)_set).ExceptWith(other);
        Refresh();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        ((ISet<T>)_set).IntersectWith(other);
        Refresh();
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)_set).SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)_set).SymmetricExceptWith(other);
        Refresh();
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        ((ISet<T>)_set).UnionWith(other);
        Refresh();
    }

    void ICollection<T>.Add(T item)
    {
        var count = Count;
        ((ICollection<T>)_set).Add(item);
        if (Count != count)
            _list.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)_set).Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_set).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_set).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return ((ICollection<T>)_set).Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_set).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion
}
