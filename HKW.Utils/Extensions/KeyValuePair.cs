using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class KeyValuePairExtensions
{
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="pair">键值对</param>
    extension<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        where TKey : notnull
    {
        /// <summary>
        /// 是空的
        /// </summary>
        public bool IsEmpty => EqualityComparer<TKey>.Default.Equals(pair.Key, default);

        /// <inheritdoc/>
        public static bool operator ==(KeyValuePair<TKey, TValue> a, KeyValuePair<TKey, TValue> b)
        {
            return EqualityComparer<TKey>.Default.Equals(a.Key, b.Key)
                && EqualityComparer<TValue>.Default.Equals(a.Value, b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=(KeyValuePair<TKey, TValue> a, KeyValuePair<TKey, TValue> b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public static bool operator ==(KeyValuePair<TKey, TValue> a, (TKey, TValue) b)
        {
            return EqualityComparer<TKey>.Default.Equals(a.Key, b.Item1)
                && EqualityComparer<TValue>.Default.Equals(a.Value, b.Item2);
        }

        /// <inheritdoc/>
        public static bool operator !=(KeyValuePair<TKey, TValue> a, (TKey, TValue) b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 内容相同
        /// </summary>
        /// <param name="pair2">键值对2</param>
        /// <returns>是否内容相同</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals((TKey Key, TValue Value) pair2)
        {
            return pair == pair2;
        }

        /// <summary>
        /// 内容相同
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否内容相同</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TKey key, TValue value)
        {
            return EqualityComparer<TKey>.Default.Equals(pair.Key, key)
                && EqualityComparer<TValue>.Default.Equals(pair.Value, value);
        }

        /// <summary>
        /// 作为元组
        /// </summary>
        /// <returns>元组</returns>
        public (TKey Key, TValue Value) AsTuple()
        {
            return (pair.Key, pair.Value);
        }
    }

    extension(KeyValuePair)
    {
        /// <summary>
        /// 从元组创建键值对
        /// </summary>
        /// <param name="tuple">元组</param>
        /// <returns>键值对</returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>((TKey, TValue) tuple)
            where TKey : notnull
        {
            return new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2);
        }
    }
}
