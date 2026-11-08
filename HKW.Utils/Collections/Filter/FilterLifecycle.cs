using System.Runtime.ExceptionServices;

namespace HKW.HKWUtils.Collections;

internal interface IFilterStorage
{
    object SourceStorage { get; }
}

/// <summary>
/// 三类过滤包装器共享操作边界, 不共享集合算法.
/// </summary>
internal sealed class FilterLifecycle
{
    public bool IsActive { get; private set; }
    public bool IsUpdatingSource { get; private set; }
    public bool SourceInvalidated { get; private set; }
    public bool IsValid { get; set; } = true;
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 启用
    /// </summary>
    /// <param name="recovery">恢复</param>
    /// <returns>过滤租期</returns>
    /// <exception cref="InvalidOperationException">当 <see cref="IsDisposed"/> 是 <see langword="true"/></exception>
    public Lease Enter(bool recovery = false)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsActive)
            throw new InvalidOperationException(
                "Reentrant writes to the same filtered collection are not allowed."
            );
        if (recovery is false)
            EnsureReadable();
        IsActive = true;
        SourceInvalidated = false;
        return new(this);
    }

    public void InvalidateSource()
    {
        SourceInvalidated = true;
        IsValid = false;
    }

    public void EnsureSourceUnchanged()
    {
        if (SourceInvalidated)
            throw new InvalidOperationException(
                "The source changed during filtered view publication. Call Refresh to recover."
            );
    }

    public void EnsureReadable()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsValid is false)
            throw new InvalidOperationException(
                "The filtered view is invalid. Call Refresh successfully before reading or modifying it."
            );
    }

    public void Update(Action update, Action? refresh)
    {
        ExceptionDispatchInfo? failure = null;
        IsUpdatingSource = true;
        try
        {
            update();
        }
        catch (Exception exception)
        {
            failure = ExceptionDispatchInfo.Capture(exception);
        }
        finally
        {
            IsUpdatingSource = false;
        }
        try
        {
            refresh?.Invoke();
        }
        catch (Exception exception) when (failure is not null)
        {
            throw new AggregateException(
                "Both the source update and filtered view recovery failed.",
                failure.SourceException,
                exception
            );
        }
        failure?.Throw();
    }

    public void Dispose(Action unsubscribe)
    {
        if (IsDisposed)
            return;
        using var operation = Enter(recovery: true);
        unsubscribe();
        IsDisposed = true;
    }

    public static void Validate<T>(
        ICollection<T> source,
        ICollection<T> target,
        object? owner = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        if (ReferenceEquals(source, owner) || ReferenceEquals(target, owner))
            throw new ArgumentException("A filtered collection cannot use itself as storage.");
        if (source.IsReadOnly || target.IsReadOnly)
            throw new ArgumentException("Source and filtered collections must be writable.");
        if (ReferenceEquals(Storage(source, owner), Storage(target, owner)))
            throw new ArgumentException("Source and filtered collections must not share storage.");
    }

    private static object Storage(object collection, object? owner)
    {
        while (collection is IFilterStorage wrapper)
        {
            collection = wrapper.SourceStorage;
            if (ReferenceEquals(collection, owner))
                throw new ArgumentException(
                    "Filtered collection storage must not contain a cycle."
                );
        }
        return collection;
    }

    public static bool SameItem<T>(T first, T second) =>
        typeof(T).IsValueType
            ? EqualityComparer<T>.Default.Equals(first, second)
            : ReferenceEquals(first, second);

    public static bool Matches<T>(ICollection<T> target, IReadOnlyList<T> expected)
    {
        if (target.Count != expected.Count)
            return false;
        using var items = target.GetEnumerator();
        for (var index = 0; index < expected.Count; index++)
        {
            if (items.MoveNext() is false || SameItem(items.Current, expected[index]) is false)
                return false;
        }
        return items.MoveNext() is false;
    }

    internal sealed class ItemComparer<T> : IEqualityComparer<T>
    {
        public static ItemComparer<T> Instance { get; } = new();

        public bool Equals(T? first, T? second) => SameItem(first, second);

        public int GetHashCode(T item)
        {
            if (item is null)
            {
                return 0;
            }
            else
            {
                return typeof(T).IsValueType
                    ? EqualityComparer<T>.Default.GetHashCode(item)
                    : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(item);
            }
        }
    }

    internal sealed class PairComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public static PairComparer<TKey, TValue> Instance { get; } = new();

        public bool Equals(KeyValuePair<TKey, TValue> first, KeyValuePair<TKey, TValue> second) =>
            SameItem(first.Key, second.Key) && SameItem(first.Value, second.Value);

        public int GetHashCode(KeyValuePair<TKey, TValue> item) =>
            HashCode.Combine(
                ItemComparer<TKey>.Instance.GetHashCode(item.Key!),
                ItemComparer<TValue>.Instance.GetHashCode(item.Value!)
            );
    }

    public readonly struct Lease(FilterLifecycle owner) : IDisposable
    {
        public void Dispose() => owner.IsActive = false;
    }
}
