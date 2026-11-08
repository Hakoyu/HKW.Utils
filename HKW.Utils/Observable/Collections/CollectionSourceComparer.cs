namespace HKW.HKWUtils.Observable;

internal static class CollectionSourceComparer
{
    public static IEqualityComparer<TKey> Dictionary<TKey, TValue>(
        IDictionary<TKey, TValue> source,
        IEqualityComparer<TKey>? supplied
    )
        where TKey : notnull
    {
        IEqualityComparer<TKey>? actual = source switch
        {
            Dictionary<TKey, TValue> dictionary => dictionary.Comparer,
            OrderedDictionary<TKey, TValue> dictionary => dictionary.Comparer,
            SortedDictionary<TKey, TValue> dictionary => new OrderingEqualityComparer<TKey>(
                dictionary.Comparer
            ),
            SortedList<TKey, TValue> dictionary => new OrderingEqualityComparer<TKey>(
                dictionary.Comparer
            ),
            _ => null,
        };
        return actual ?? supplied ?? EqualityComparer<TKey>.Default;
    }

    public static IEqualityComparer<T> Set<T>(ISet<T> source, IEqualityComparer<T>? supplied)
    {
        IEqualityComparer<T>? actual = source switch
        {
            HashSet<T> set => set.Comparer,
            // 只读取已存在实例的比较器, 空值限制仍由具体后端决定.
#pragma warning disable CS8714
            Collections.OrderedHashSet<T> set => set.Comparer,
#pragma warning restore CS8714
            SortedSet<T> set => new OrderingEqualityComparer<T>(set.Comparer),
            _ => null,
        };
        return actual ?? supplied ?? EqualityComparer<T>.Default;
    }

    // 有序比较器未提供哈希函数; 以比较结果确定相等, 不能套用默认哈希.
    private sealed class OrderingEqualityComparer<T>(IComparer<T> comparer) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => comparer.Compare(x!, y!) == 0;

        public int GetHashCode(T obj) => 0;
    }
}
