using System.Collections;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 为仅实现泛型接口的后端提供非泛型参数, 数组和枚举适配.
/// </summary>
internal static class CollectionInterfaceBridge
{
    internal static bool IsCompatible<T>(object? value) =>
        value is T || value is null && default(T) is null;

    internal static T GetValue<T>(object? value, string parameterName)
    {
        if (value is null && default(T) is not null)
            throw new ArgumentNullException(parameterName);
        if (IsCompatible<T>(value) is false)
            throw new ArgumentException(
                $"Value must be compatible with {typeof(T)}.",
                parameterName
            );
        return (T)value!;
    }

    internal static T GetKey<T>(object key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return GetValue<T>(key, nameof(key));
    }

    internal static bool IsCompatibleKey<T>(object key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return key is T;
    }

    internal static object SyncRoot<TSource>(TSource source, object fallback)
    {
        if (source is ICollection collection)
        {
            return collection.SyncRoot;
        }
        else
        {
            return typeof(TSource).IsValueType ? fallback : source!;
        }
    }

    internal static bool IsSynchronized(object source) =>
        source is ICollection { IsSynchronized: true };

    internal static bool IsFixedSize<T>(IList<T> source) =>
        source is IList list ? list.IsFixedSize : source.IsReadOnly;

    internal static void ValidateArray(Array array, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Rank != 1)
            throw new ArgumentException("Array must be one-dimensional.", nameof(array));
        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException("Array must have a zero lower bound.", nameof(array));
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        if (index > array.Length || count > array.Length - index)
            throw new ArgumentException(
                "Destination array has insufficient capacity.",
                nameof(array)
            );
    }

    internal static void CopyTo<T>(ICollection<T> source, Array array, int index)
    {
        ValidateArray(array, index, source.Count);
        try
        {
            if (array is T[] typed)
            {
                source.CopyTo(typed, index);
                return;
            }
            var elementType = array.GetType().GetElementType()!;
            if (
                array is not object[] objects
                || (
                    elementType.IsAssignableFrom(typeof(T))
                    || typeof(T).IsAssignableFrom(elementType)
                )
                    is false
            )
                throw new ArgumentException(
                    "Destination array type is incompatible.",
                    nameof(array)
                );
            foreach (var item in source)
                objects[index++] = item!;
        }
        catch (ArrayTypeMismatchException)
        {
            throw new ArgumentException("Destination array type is incompatible.", nameof(array));
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException("Destination array type is incompatible.", nameof(array));
        }
    }

    internal static void CopyDictionaryTo<TKey, TValue>(
        IDictionary<TKey, TValue> source,
        Array array,
        int index
    )
    {
        ValidateArray(array, index, source.Count);
        if (array is DictionaryEntry[] entries)
        {
            foreach (var pair in source)
                entries[index++] = new DictionaryEntry(pair.Key!, pair.Value);
            return;
        }
        CopyTo(source, array, index);
    }

    internal static ICollection AsCollection<T>(ICollection<T> source) =>
        source as ICollection ?? new CollectionView<T>(source);

    private sealed class CollectionView<T>(ICollection<T> source) : ICollection
    {
        public int Count => source.Count;
        public bool IsSynchronized => CollectionInterfaceBridge.IsSynchronized(source);
        public object SyncRoot => CollectionInterfaceBridge.SyncRoot(source, this);

        public void CopyTo(Array array, int index) =>
            CollectionInterfaceBridge.CopyTo(source, array, index);

        public IEnumerator GetEnumerator() => source.GetEnumerator();
    }

    internal sealed class DictionaryEnumerator<TKey, TValue>(
        IEnumerator<KeyValuePair<TKey, TValue>> enumerator
    ) : IDictionaryEnumerator, IDisposable
    {
        private bool _active;

        public DictionaryEntry Entry
        {
            get
            {
                if (_active is false)
                    throw new InvalidOperationException(
                        "Enumeration has not started or has already finished."
                    );
                var pair = enumerator.Current;
                return new DictionaryEntry(pair.Key!, pair.Value);
            }
        }

        public object Key => Entry.Key;
        public object? Value => Entry.Value;
        public object Current => Entry;

        public bool MoveNext() => _active = enumerator.MoveNext();

        public void Reset()
        {
            enumerator.Reset();
            _active = false;
        }

        public void Dispose()
        {
            enumerator.Dispose();
            _active = false;
        }
    }
}
