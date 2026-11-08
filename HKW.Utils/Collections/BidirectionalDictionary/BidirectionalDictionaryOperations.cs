namespace HKW.HKWUtils.Collections;

/// <summary>
/// 双表操作; 后端必须独占写入, 比较器稳定且不重入, 失败操作须在写入前抛错.
/// 回滚失败会同时报告原始异常与回滚异常, 不递归重试失效的后端.
/// </summary>
internal static class BidirectionalDictionaryOperations
{
    internal static void ValidateWritable<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse
    )
        where TKey : notnull
        where TValue : notnull
    {
        if (forward.IsReadOnly || reverse.IsReadOnly)
            throw new NotSupportedException("Both backing dictionaries must be writable.");
    }

    internal static bool TryAdd<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        TKey key,
        TValue value
    )
        where TKey : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        ValidateWritable(forward, reverse);
        if (forward.ContainsKey(key) || reverse.ContainsKey(value))
            return false;
        return AddValidated(forward, reverse, key, value);
    }

    private static bool AddValidated<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        TKey key,
        TValue value
    )
        where TKey : notnull
        where TValue : notnull
    {
        forward.Add(key, value);
        try
        {
            reverse.Add(value, key);
        }
        catch (Exception error)
        {
            Rollback(error, static state => state.forward.Remove(state.key), (forward, key));
            throw;
        }
        return true;
    }

    internal static bool TrySet<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        IEqualityComparer<TValue> comparer,
        TKey key,
        TValue value
    )
        where TKey : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        ValidateWritable(forward, reverse);
        if (forward.TryGetValue(key, out var oldValue) is false)
            return reverse.ContainsKey(value) is false
                && AddValidated(forward, reverse, key, value);
        if (comparer.Equals(oldValue, value))
            return true;
        if (reverse.ContainsKey(value))
            return false;
        var storedKey = reverse[oldValue];
        reverse.Add(value, storedKey);
        try
        {
            forward[key] = value;
        }
        catch (Exception error)
        {
            Rollback(error, static state => state.reverse.Remove(state.value), (reverse, value));
            throw;
        }
        try
        {
            if (reverse.Remove(oldValue) is false)
                throw new InvalidOperationException("The backing dictionaries are inconsistent.");
        }
        catch (Exception error)
        {
            Rollback(
                error,
                static state =>
                {
                    state.forward[state.key] = state.oldValue;
                    state.reverse.Remove(state.value);
                },
                (forward, reverse, key, oldValue, value)
            );
            throw;
        }
        return true;
    }

    internal static bool Remove<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        TKey key
    )
        where TKey : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(key);
        ValidateWritable(forward, reverse);
        if (forward.TryGetValue(key, out var value) is false)
            return false;
        var storedKey = reverse[value];
        if (reverse.Remove(value) is false)
            throw new InvalidOperationException("The backing dictionaries are inconsistent.");
        try
        {
            if (forward.Remove(key) is false)
                throw new InvalidOperationException("The backing dictionaries are inconsistent.");
        }
        catch (Exception error)
        {
            Rollback(
                error,
                static state => state.reverse.Add(state.value, state.storedKey),
                (reverse, value, storedKey)
            );
            throw;
        }
        return true;
    }

    internal static bool Remove<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        IEqualityComparer<TValue> comparer,
        KeyValuePair<TKey, TValue> item
    )
        where TKey : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(item.Key);
        ArgumentNullException.ThrowIfNull(item.Value);
        ValidateWritable(forward, reverse);
        return forward.TryGetValue(item.Key, out var value)
            && comparer.Equals(item.Value, value)
            && Remove(forward, reverse, item.Key);
    }

    internal static void Clear<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse
    )
        where TKey : notnull
        where TValue : notnull
    {
        ValidateWritable(forward, reverse);
        var snapshot = forward.ToArray();
        forward.Clear();
        try
        {
            reverse.Clear();
        }
        catch (Exception error)
        {
            Rollback(
                error,
                static state =>
                {
                    foreach (var pair in state.snapshot)
                        state.forward.Add(pair.Key, pair.Value);
                },
                (forward, snapshot)
            );
            throw;
        }
    }

    private static void Rollback<TState>(Exception error, Action<TState> undo, TState state)
    {
        try
        {
            undo(state);
        }
        catch (Exception rollbackError)
        {
            throw new AggregateException(
                "The operation and its rollback both failed. The backing dictionaries may be inconsistent.",
                error,
                rollbackError
            );
        }
    }

    internal static IEqualityComparer<TKey> GetComparer<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        IEqualityComparer<TKey>? comparer
    )
        where TKey : notnull
    {
        if (dictionary is Dictionary<TKey, TValue> known)
        {
            if (comparer is not null && Equals(comparer, known.Comparer) is false)
                throw new ArgumentException(
                    "The comparer must match the backing dictionary comparer.",
                    nameof(comparer)
                );
            return known.Comparer;
        }
        return comparer
            ?? throw new ArgumentException(
                "An explicit matching comparer is required for custom backing dictionaries.",
                nameof(comparer)
            );
    }

    internal static void ValidateInverse<TKey, TValue>(
        IDictionary<TKey, TValue> forward,
        IDictionary<TValue, TKey> reverse,
        IEqualityComparer<TKey> keyComparer,
        IEqualityComparer<TValue> valueComparer
    )
        where TKey : notnull
        where TValue : notnull
    {
        var keys = new HashSet<TKey>(keyComparer);
        var values = new HashSet<TValue>(valueComparer);
        if (forward.Count != reverse.Count)
            throw new ArgumentException("The backing dictionaries must be mutual inverses.");
        foreach (var pair in forward)
        {
            ArgumentNullException.ThrowIfNull(pair.Key);
            ArgumentNullException.ThrowIfNull(pair.Value);
            if (
                keys.Add(pair.Key) is false
                || values.Add(pair.Value) is false
                || reverse.TryGetValue(pair.Value, out var key) is false
                || keyComparer.Equals(key, pair.Key) is false
            )
                throw new ArgumentException(
                    "The backing dictionaries must be mutual inverses under the supplied comparers."
                );
        }
        foreach (var pair in reverse)
        {
            ArgumentNullException.ThrowIfNull(pair.Key);
            ArgumentNullException.ThrowIfNull(pair.Value);
            if (
                forward.TryGetValue(pair.Value, out var value) is false
                || valueComparer.Equals(value, pair.Key) is false
            )
                throw new ArgumentException(
                    "The backing dictionaries must be mutual inverses under the supplied comparers."
                );
        }
    }
}
