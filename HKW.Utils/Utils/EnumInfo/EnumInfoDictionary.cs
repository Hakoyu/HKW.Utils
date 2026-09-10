using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils;

/// <summary>
/// 冻结的枚举信息字典
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public sealed class FrozenEnumInfoDictionary<TEnum>
    : IDictionary<TEnum, EnumInfo<TEnum>>,
        IReadOnlyDictionary<TEnum, EnumInfo<TEnum>>
    where TEnum : struct, Enum
{
    private readonly FrozenDictionary<TEnum, EnumInfo<TEnum>> _dictionary;
    private readonly UntypedDictionary _untypedDictionary;

    /// <summary>
    /// 初始化枚举信息字典
    /// </summary>
    /// <param name="pairs">源字典</param>
    public FrozenEnumInfoDictionary(IEnumerable<KeyValuePair<TEnum, EnumInfo<TEnum>>> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);
        _dictionary = FrozenDictionary.ToFrozenDictionary(pairs);
        _untypedDictionary = new(this);
    }

    /// <summary>
    /// 无类型字典
    /// </summary>
    public IDictionary<Enum, IEnumInfo> Untyped => _untypedDictionary;

    /// <summary>
    /// 无类型只读字典
    /// </summary>
    public IReadOnlyDictionary<Enum, IEnumInfo> UntypedReadOnly => _untypedDictionary;

    #region  IDictionary
    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public EnumInfo<TEnum> this[TEnum key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc cref="Keys"/>
    public ImmutableArray<TEnum> Keys => _dictionary.Keys;

    /// <inheritdoc cref="Values"/>
    public ImmutableArray<EnumInfo<TEnum>> Values => _dictionary.Values;

    /// <inheritdoc/>
    ICollection<TEnum> IDictionary<TEnum, EnumInfo<TEnum>>.Keys => _dictionary.Keys;

    /// <inheritdoc/>
    ICollection<EnumInfo<TEnum>> IDictionary<TEnum, EnumInfo<TEnum>>.Values => _dictionary.Values;

    /// <inheritdoc/>
    public bool ContainsKey(TEnum key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TEnum key, [MaybeNullWhen(false)] out EnumInfo<TEnum> value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    void IDictionary<TEnum, EnumInfo<TEnum>>.Add(TEnum key, EnumInfo<TEnum> value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TEnum, EnumInfo<TEnum>>>.Add(
        KeyValuePair<TEnum, EnumInfo<TEnum>> item
    )
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TEnum, EnumInfo<TEnum>>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TEnum, EnumInfo<TEnum>> item)
    {
        if (_dictionary.TryGetValue(item.Key, out var value) is false)
            return false;
        return EqualityComparer<EnumInfo<TEnum>>.Default.Equals(value, item.Value);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TEnum, EnumInfo<TEnum>>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ValidateCopyTo(array.Length, arrayIndex, Count);

        foreach (var pair in _dictionary)
            array[arrayIndex++] = pair;
    }

    /// <inheritdoc/>
    bool IDictionary<TEnum, EnumInfo<TEnum>>.Remove(TEnum key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TEnum, EnumInfo<TEnum>>>.Remove(
        KeyValuePair<TEnum, EnumInfo<TEnum>> item
    )
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TEnum, EnumInfo<TEnum>>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TEnum, EnumInfo<TEnum>>>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
    #region IReadOnlyDictionary
    /// <inheritdoc/>
    IEnumerable<TEnum> IReadOnlyDictionary<TEnum, EnumInfo<TEnum>>.Keys =>
        ((IDictionary<TEnum, EnumInfo<TEnum>>)this).Keys;

    /// <inheritdoc/>
    IEnumerable<EnumInfo<TEnum>> IReadOnlyDictionary<TEnum, EnumInfo<TEnum>>.Values =>
        ((IDictionary<TEnum, EnumInfo<TEnum>>)this).Values;
    #endregion
    private static bool TryGetTypedKey(Enum key, out TEnum enumKey)
    {
        if (key is TEnum typedKey)
        {
            enumKey = typedKey;
            return true;
        }
        enumKey = default;
        return false;
    }

    private static void ValidateCopyTo(int arrayLength, int arrayIndex, int count)
    {
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (arrayIndex > arrayLength)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (arrayLength - arrayIndex < count)
            throw new ArgumentException("The destination array is too small.", nameof(arrayIndex));
    }

    /// <summary>
    /// 无类型字典
    /// </summary>
    public sealed class UntypedDictionary
        : IDictionary<Enum, IEnumInfo>,
            IReadOnlyDictionary<Enum, IEnumInfo>
    {
        /// <summary>
        /// 泛型源
        /// </summary>
        public FrozenEnumInfoDictionary<TEnum> GenericSource { get; }
        private readonly KeyCollection _keys;
        private readonly ValueCollection _values;

        /// <inheritdoc/>
        public UntypedDictionary(FrozenEnumInfoDictionary<TEnum> source)
        {
            GenericSource = source;
            _keys = new(source._dictionary);
            _values = new(source._dictionary);
        }

        /// <inheritdoc/>
        public int Count => GenericSource.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public IEnumInfo this[Enum key]
        {
            get
            {
                if (
                    TryGetTypedKey(key, out var enumKey)
                    && GenericSource._dictionary.TryGetValue(enumKey, out var value)
                )
                    return value;
                throw new KeyNotFoundException("The given key was not present in the dictionary.");
            }
            set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        /// <inheritdoc/>
        public ICollection<Enum> Keys => _keys;

        /// <inheritdoc/>
        public ICollection<IEnumInfo> Values => _values;

        /// <inheritdoc/>
        IEnumerable<Enum> IReadOnlyDictionary<Enum, IEnumInfo>.Keys => _keys;

        /// <inheritdoc/>
        IEnumerable<IEnumInfo> IReadOnlyDictionary<Enum, IEnumInfo>.Values => _values;

        /// <inheritdoc/>
        public bool ContainsKey(Enum key)
        {
            return TryGetTypedKey(key, out var enumKey)
                && GenericSource._dictionary.ContainsKey(enumKey);
        }

        /// <inheritdoc/>
        public bool TryGetValue(Enum key, [MaybeNullWhen(false)] out IEnumInfo value)
        {
            if (
                TryGetTypedKey(key, out var enumKey)
                && GenericSource._dictionary.TryGetValue(enumKey, out var enumInfo)
            )
            {
                value = enumInfo;
                return true;
            }
            value = default;
            return false;
        }

        void IDictionary<Enum, IEnumInfo>.Add(Enum key, IEnumInfo value)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<KeyValuePair<Enum, IEnumInfo>>.Add(KeyValuePair<Enum, IEnumInfo> item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<KeyValuePair<Enum, IEnumInfo>>.Clear()
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<KeyValuePair<Enum, IEnumInfo>>.Contains(KeyValuePair<Enum, IEnumInfo> item)
        {
            if (TryGetTypedKey(item.Key, out var key) is false)
                return false;
            if (item.Value is not EnumInfo<TEnum> typedValue)
                return false;
            if (GenericSource._dictionary.TryGetValue(key, out var value) is false)
                return false;
            return EqualityComparer<EnumInfo<TEnum>>.Default.Equals(value, typedValue);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<Enum, IEnumInfo>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ValidateCopyTo(array.Length, arrayIndex, Count);

            foreach (var pair in GenericSource._dictionary)
                array[arrayIndex++] = new(pair.Key, pair.Value);
        }

        bool IDictionary<Enum, IEnumInfo>.Remove(Enum key)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<KeyValuePair<Enum, IEnumInfo>>.Remove(KeyValuePair<Enum, IEnumInfo> item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<Enum, IEnumInfo>> GetEnumerator()
        {
            return new UntypedEnumerator(GenericSource._dictionary.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 键集合
    /// </summary>
    public sealed class KeyCollection : ICollection<Enum>, IReadOnlyCollection<Enum>
    {
        private readonly FrozenDictionary<TEnum, EnumInfo<TEnum>> _dictionary;

        /// <inheritdoc/>
        public KeyCollection(FrozenDictionary<TEnum, EnumInfo<TEnum>> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public bool Contains(Enum item)
        {
            return TryGetTypedKey(item, out var key) && _dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public void CopyTo(Enum[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ValidateCopyTo(array.Length, arrayIndex, Count);

            foreach (var pair in _dictionary)
                array[arrayIndex++] = pair.Key;
        }

        /// <inheritdoc/>
        public IEnumerator<Enum> GetEnumerator()
        {
            return new KeyEnumerator(_dictionary.GetEnumerator());
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<Enum>.Add(Enum item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<Enum>.Clear()
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<Enum>.Remove(Enum item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        /// <summary>
        /// 键枚举器
        /// </summary>
        public struct KeyEnumerator : IEnumerator<Enum>
        {
            private FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator _enumerator;

            /// <inheritdoc/>
            public KeyEnumerator(FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator enumerator)
            {
                _enumerator = enumerator;
            }

            /// <inheritdoc/>
            public Enum Current => _enumerator.Current.Key;

            /// <inheritdoc/>
            object IEnumerator.Current => Current;

            /// <inheritdoc/>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <inheritdoc/>
            public void Reset()
            {
                throw new NotSupportedException("Reset is not supported.");
            }

            /// <inheritdoc/>
            public void Dispose() { }
        }
    }

    /// <summary>
    /// 值集合
    /// </summary>
    public sealed class ValueCollection : ICollection<IEnumInfo>, IReadOnlyCollection<IEnumInfo>
    {
        private readonly FrozenDictionary<TEnum, EnumInfo<TEnum>> _dictionary;

        /// <inheritdoc/>
        public ValueCollection(FrozenDictionary<TEnum, EnumInfo<TEnum>> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public bool Contains(IEnumInfo item)
        {
            if (item is not EnumInfo<TEnum> typedItem)
                return false;
            if (_dictionary.TryGetValue(typedItem.Value, out var value) is false)
                return false;
            return EqualityComparer<EnumInfo<TEnum>>.Default.Equals(value, typedItem);
        }

        /// <inheritdoc/>
        public void CopyTo(IEnumInfo[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ValidateCopyTo(array.Length, arrayIndex, Count);

            foreach (var pair in _dictionary)
                array[arrayIndex++] = pair.Value;
        }

        /// <inheritdoc/>
        public IEnumerator<IEnumInfo> GetEnumerator()
        {
            return new ValueEnumerator(_dictionary.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<IEnumInfo>.Add(IEnumInfo item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<IEnumInfo>.Clear()
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<IEnumInfo>.Remove(IEnumInfo item)
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        private struct ValueEnumerator : IEnumerator<IEnumInfo>
        {
            private FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator _enumerator;

            public ValueEnumerator(FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator enumerator)
            {
                _enumerator = enumerator;
            }

            public IEnumInfo Current => _enumerator.Current.Value;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                throw new NotSupportedException("Reset is not supported.");
            }

            public void Dispose() { }
        }
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    public struct UntypedEnumerator : IEnumerator<KeyValuePair<Enum, IEnumInfo>>
    {
        private FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator _enumerator;

        /// <inheritdoc/>
        public UntypedEnumerator(FrozenDictionary<TEnum, EnumInfo<TEnum>>.Enumerator enumerator)
        {
            _enumerator = enumerator;
        }

        /// <inheritdoc/>
        public KeyValuePair<Enum, IEnumInfo> Current =>
            new(_enumerator.Current.Key, _enumerator.Current.Value);

        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            throw new NotSupportedException("Reset is not supported.");
        }

        /// <inheritdoc/>
        public void Dispose() { }
    }
}
