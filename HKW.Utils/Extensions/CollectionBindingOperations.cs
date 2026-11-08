using System.Collections;
using System.Collections.Specialized;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

internal static class CollectionBindingOperations
{
    private static T[] Snapshot<T>(object source)
    {
        if (source is not IEnumerable enumerable)
            throw new ArgumentException("The binding source must be enumerable.", nameof(source));
        return enumerable.Cast<T>().ToArray();
    }

    private static T[] Items<T>(IList? items) => items is null ? [] : items.Cast<T>().ToArray();

    internal static Action<NotifyListChangeEventArgs<T>> ListX<T>(object source, IList<T> target)
    {
        ResetList(target, Snapshot<T>(source));
        return e =>
        {
            switch (e.Action)
            {
                case ListChangeAction.Add:
                    target.Insert(e.Index, e.NewItem!);
                    break;
                case ListChangeAction.Remove:
                    target.RemoveAt(e.Index);
                    break;
                case ListChangeAction.Replace:
                    target[e.Index] = e.NewItem!;
                    break;
                case ListChangeAction.Clear:
                    ResetList(target, Snapshot<T>(source));
                    break;
                default:
                    throw new NotSupportedException("The list change action is not supported.");
            }
        };
    }

    internal static Action<NotifyCollectionChangedEventArgs> List<T>(object source, IList<T> target)
    {
        ResetList(target, Snapshot<T>(source));
        return e =>
        {
            if (
                e.Action == NotifyCollectionChangedAction.Reset
                || (
                    e.Action
                        is NotifyCollectionChangedAction.Add
                            or NotifyCollectionChangedAction.Replace
                            or NotifyCollectionChangedAction.Move
                    && e.NewStartingIndex < 0
                )
                || (
                    e.Action
                        is NotifyCollectionChangedAction.Remove
                            or NotifyCollectionChangedAction.Replace
                            or NotifyCollectionChangedAction.Move
                    && e.OldStartingIndex < 0
                )
            )
            {
                ResetList(target, Snapshot<T>(source));
                return;
            }
            // 转换整个载荷后再写目标, 避免类型错误留下部分插入.
            var added = Items<T>(e.NewItems);
            var removed = Items<T>(e.OldItems);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Insert(target, e.NewStartingIndex, added);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(target, e.OldStartingIndex, removed.Length);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (removed.Length == added.Length && e.OldStartingIndex == e.NewStartingIndex)
                    {
                        for (var i = 0; i < added.Length; i++)
                            target[e.NewStartingIndex + i] = added[i];
                    }
                    else
                    {
                        Remove(target, e.OldStartingIndex, removed.Length);
                        Insert(target, e.NewStartingIndex, added);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    Remove(target, e.OldStartingIndex, removed.Length);
                    Insert(target, e.NewStartingIndex, added);
                    break;
                default:
                    throw new NotSupportedException(
                        "The collection change action is not supported."
                    );
            }
        };
    }

    private static void ResetList<T>(IList<T> target, T[] items)
    {
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }

    private static void Insert<T>(IList<T> target, int index, T[] items)
    {
        foreach (var item in items)
            target.Insert(index++, item);
    }

    private static void Remove<T>(IList<T> target, int index, int count)
    {
        for (var i = index + count - 1; i >= index; i--)
            target.RemoveAt(i);
    }

    internal static Action<NotifyDictionaryChangeEventArgs<TKey, TValue>> DictionaryX<TKey, TValue>(
        object source,
        IDictionary<TKey, TValue> target
    )
        where TKey : notnull
    {
        ResetDictionary(target, Snapshot<KeyValuePair<TKey, TValue>>(source));
        return e =>
        {
            switch (e.Action)
            {
                case DictionaryChangeAction.Add:
                    var added = e.NewPair!.Value;
                    target.Add(added.Key, added.Value);
                    break;
                case DictionaryChangeAction.Remove:
                    target.Remove(e.OldPair!.Value.Key);
                    break;
                case DictionaryChangeAction.Replace:
                    var replacement = e.NewPair!.Value;
                    target.Remove(e.OldPair!.Value.Key);
                    target[replacement.Key] = replacement.Value;
                    break;
                case DictionaryChangeAction.Clear:
                    ResetDictionary(target, Snapshot<KeyValuePair<TKey, TValue>>(source));
                    break;
                default:
                    throw new NotSupportedException(
                        "The dictionary change action is not supported."
                    );
            }
        };
    }

    internal static Action<NotifyCollectionChangedEventArgs> Dictionary<TKey, TValue>(
        object source,
        IDictionary<TKey, TValue> target
    )
        where TKey : notnull
    {
        ResetDictionary(target, Snapshot<KeyValuePair<TKey, TValue>>(source));
        return e =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetDictionary(target, Snapshot<KeyValuePair<TKey, TValue>>(source));
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;
            var added = Items<KeyValuePair<TKey, TValue>>(e.NewItems);
            var removed = Items<KeyValuePair<TKey, TValue>>(e.OldItems);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var pair in added)
                        target.Add(pair.Key, pair.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var pair in removed)
                        target.Remove(pair.Key);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var pair in removed)
                        target.Remove(pair.Key);
                    foreach (var pair in added)
                        target[pair.Key] = pair.Value;
                    break;
                default:
                    throw new NotSupportedException(
                        "The collection change action is not supported."
                    );
            }
        };
    }

    private static void ResetDictionary<TKey, TValue>(
        IDictionary<TKey, TValue> target,
        KeyValuePair<TKey, TValue>[] items
    )
        where TKey : notnull
    {
        target.Clear();
        foreach (var pair in items)
            target.Add(pair.Key, pair.Value);
    }

    internal static Action<NotifyCollectionChangedEventArgs> Set<T>(object source, ISet<T> target)
    {
        var projection = new SetProjection<T>(source, target);
        projection.Reset();
        return e =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                    return;
                case NotifyCollectionChangedAction.Reset:
                    projection.Reset();
                    return;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    projection.Update(Items<T>(e.OldItems), Items<T>(e.NewItems));
                    return;
                default:
                    throw new NotSupportedException(
                        "The collection change action is not supported."
                    );
            }
        };
    }

    internal static Action<NotifySetChangeEventArgs<T>> SetX<T>(object source, ISet<T> target)
    {
        var projection = new SetProjection<T>(source, target);
        projection.Reset();
        return e =>
        {
            switch (e.Action)
            {
                case SetChangeAction.Clear:
                    projection.Reset();
                    return;
                case SetChangeAction.Add:
                case SetChangeAction.Remove:
                case SetChangeAction.Union:
                case SetChangeAction.Except:
                case SetChangeAction.Intersect:
                case SetChangeAction.SymmetricExcept:
                    projection.Update(e.OldItems?.ToArray() ?? [], e.NewItems?.ToArray() ?? []);
                    return;
                default:
                    throw new NotSupportedException("The set change action is not supported.");
            }
        };
    }

    private sealed class SetProjection<T>(object source, ISet<T> target)
    {
        // 可获取比较器的哈希集合使用引用计数, 包装键允许源包含 null.
        private readonly IEqualityComparer<ItemKey<T>>? _comparer = target is HashSet<T> hash
            ? new ItemKeyComparer<T>(hash.Comparer)
            : null;
        private Dictionary<ItemKey<T>, int>? _counts;

        internal void Reset()
        {
            var items = Snapshot<T>(source);
            if (_comparer is not null)
            {
                var counts = new Dictionary<ItemKey<T>, int>(_comparer);
                foreach (var item in items)
                {
                    var key = new ItemKey<T>(item);
                    counts.TryGetValue(key, out var count);
                    counts[key] = count + 1;
                }
                _counts = counts;
            }
            target.Clear();
            target.UnionWith(items);
        }

        internal void Update(T[] removed, T[] added)
        {
            if (_counts is null)
            {
                // 任意 ISet 的比较规则无法推断, 使用目标自身的集合运算维护投影.
                var items = Snapshot<T>(source);
                target.IntersectWith(items);
                target.UnionWith(items);
                return;
            }
            foreach (var item in removed)
            {
                var key = new ItemKey<T>(item);
                if (_counts.TryGetValue(key, out var count) is false)
                    throw new InvalidOperationException(
                        "The set change does not match the binding snapshot."
                    );
                if (count == 1)
                {
                    _counts.Remove(key);
                    target.Remove(item);
                }
                else
                    _counts[key] = count - 1;
            }
            foreach (var item in added)
            {
                var key = new ItemKey<T>(item);
                _counts.TryGetValue(key, out var count);
                _counts[key] = count + 1;
                if (count == 0)
                    target.Add(item);
            }
        }
    }

    private readonly record struct ItemKey<T>(T Value);

    private sealed class ItemKeyComparer<T>(IEqualityComparer<T> comparer)
        : IEqualityComparer<ItemKey<T>>
    {
        public bool Equals(ItemKey<T> x, ItemKey<T> y) => comparer.Equals(x.Value, y.Value);

        public int GetHashCode(ItemKey<T> obj) =>
            obj.Value is null ? 0 : comparer.GetHashCode(obj.Value);
    }
}
