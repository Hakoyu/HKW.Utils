using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests;

internal static class CollectionInterfaceContract
{
    public static void Queries<T>(IList list, T present)
    {
        Assert.IsTrue(list.Contains(present));
        Assert.AreEqual(0, list.IndexOf(present));
        Assert.IsFalse(list.Contains(new object()));
        Assert.AreEqual(-1, list.IndexOf(new object()));
        Assert.AreEqual(default(T) is null, list.Contains(null));
        Assert.AreEqual(default(T) is null ? 0 : -1, list.IndexOf(null));
    }

    public static void Copy(ICollection collection, object?[] expected, Type itemType)
    {
        var objects = new object?[collection.Count + 2];
        collection.CopyTo(objects, 1);
        CollectionAssert.AreEqual(expected, objects.Skip(1).Take(collection.Count).ToArray());
        Assert.IsNull(objects[0]);
        Assert.IsNull(objects[^1]);
        var typed = Array.CreateInstance(itemType, collection.Count + 1);
        collection.CopyTo(typed, 1);
        CollectionAssert.AreEqual(expected, typed.Cast<object?>().Skip(1).ToArray());
        Assert.ThrowsExactly<ArgumentNullException>(() => collection.CopyTo(null!, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => collection.CopyTo(objects, -1));
        Assert.Throws<ArgumentException>(() => collection.CopyTo(objects, objects.Length + 1));
        Assert.ThrowsExactly<ArgumentException>(() =>
            collection.CopyTo(new object[collection.Count], 1)
        );
        Assert.ThrowsExactly<ArgumentException>(() => collection.CopyTo(new object[2, 2], 0));
        Assert.ThrowsExactly<ArgumentException>(() =>
            collection.CopyTo(Array.CreateInstance(typeof(object), [collection.Count + 1], [1]), 0)
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            collection.CopyTo(Array.CreateInstance(typeof(object), [collection.Count + 1], [-1]), 0)
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            collection.CopyTo(new DateTime[collection.Count], 0)
        );
        Assert.IsFalse(collection.IsSynchronized);
        var syncRoot = collection.SyncRoot;
        Assert.AreSame(syncRoot, collection.SyncRoot);
    }

    public static void ReadOnly<T>(ICollection<T> collection, T present)
    {
        var before = collection.ToArray();
        Assert.IsTrue(collection.IsReadOnly);
        Assert.IsTrue(collection.Contains(present));
        var copy = new T[collection.Count + 1];
        collection.CopyTo(copy, 1);
        CollectionAssert.AreEqual(before, copy.Skip(1).ToArray());
        Assert.ThrowsExactly<ArgumentNullException>(() => collection.CopyTo(null!, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => collection.CopyTo(copy, -1));
        Assert.ThrowsExactly<ArgumentException>(() => collection.CopyTo([], 0));
        Assert.ThrowsExactly<NotSupportedException>(() => collection.Add(present));
        Assert.ThrowsExactly<NotSupportedException>(() => collection.Remove(present));
        Assert.ThrowsExactly<NotSupportedException>(collection.Clear);
        CollectionAssert.AreEqual(before, collection.ToArray());
    }
}

internal class GenericOnlyList<T>(IList<T> source) : IList<T>
{
    public T this[int index]
    {
        get => source[index];
        set => source[index] = value;
    }
    public int Count => source.Count;
    public bool IsReadOnly => source.IsReadOnly;

    public void Add(T item) => source.Add(item);

    public void Clear() => source.Clear();

    public bool Contains(T item) => source.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

    public int IndexOf(T item) => source.IndexOf(item);

    public void Insert(int index, T item) => source.Insert(index, item);

    public bool Remove(T item) => source.Remove(item);

    public void RemoveAt(int index) => source.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal readonly struct GenericValueList<T>(IList<T> source) : IList<T>
{
    public T this[int index]
    {
        get => source[index];
        set => source[index] = value;
    }
    public int Count => source.Count;
    public bool IsReadOnly => source.IsReadOnly;

    public void Add(T item) => source.Add(item);

    public void Clear() => source.Clear();

    public bool Contains(T item) => source.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

    public int IndexOf(T item) => source.IndexOf(item);

    public void Insert(int index, T item) => source.Insert(index, item);

    public bool Remove(T item) => source.Remove(item);

    public void RemoveAt(int index) => source.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

#pragma warning disable CS0067
internal sealed class GenericOnlyObservableList<T>(IList<T> source)
    : GenericOnlyList<T>(source),
        IObservableList<T>
{
    public event ObservableListChangingEventHandler<T>? ListChanging;
    public event ObservableListChangedEventHandler<T>? ListChanged;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
}
#pragma warning restore CS0067

internal sealed class GenericOnlyDictionary<TKey, TValue>(IDictionary<TKey, TValue> source)
    : IDictionary<TKey, TValue>
    where TKey : notnull
{
    public TValue this[TKey key]
    {
        get => source[key];
        set => source[key] = value;
    }
    public ICollection<TKey> Keys => new GenericOnlyCollection<TKey>(source.Keys);
    public ICollection<TValue> Values => new GenericOnlyCollection<TValue>(source.Values);
    public int Count => source.Count;
    public bool IsReadOnly => source.IsReadOnly;

    public void Add(TKey key, TValue value) => source.Add(key, value);

    public void Add(KeyValuePair<TKey, TValue> item) => source.Add(item);

    public void Clear() => source.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item) => source.Contains(item);

    public bool ContainsKey(TKey key) => source.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        source.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => source.GetEnumerator();

    public bool Remove(TKey key) => source.Remove(key);

    public bool Remove(KeyValuePair<TKey, TValue> item) => source.Remove(item);

    public bool TryGetValue(TKey key, out TValue value) => source.TryGetValue(key, out value!);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class GenericOnlyCollection<T>(ICollection<T> source) : ICollection<T>
{
    public int Count => source.Count;
    public bool IsReadOnly => source.IsReadOnly;

    public void Add(T item) => source.Add(item);

    public void Clear() => source.Clear();

    public bool Contains(T item) => source.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

    public bool Remove(T item) => source.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
