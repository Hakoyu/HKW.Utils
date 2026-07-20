using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class KeyValuePairExtensions
{
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="pair1">键值对1</param>
    extension<TKey, TValue>(KeyValuePair<TKey, TValue> pair1)
        where TKey : notnull
    {
        /// <summary>
        /// 内容相同
        /// </summary>
        /// <param name="pair2">键值对2</param>
        /// <returns>是否内容相同</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals((TKey Key, TValue Value) pair2)
        {
            return EqualityComparer<TKey>.Default.Equals(pair1.Key, pair2.Key)
                && EqualityComparer<TValue>.Default.Equals(pair1.Value, pair2.Value);
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
            return EqualityComparer<TKey>.Default.Equals(pair1.Key, key)
                && EqualityComparer<TValue>.Default.Equals(pair1.Value, value);
        }
    }

    extension(KeyValuePair)
    {
        /// <summary>
        /// 从元组创建键值对
        /// </summary>
        /// <param name="tuple">元组</param>
        /// <returns>键值对</returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(
            (TKey Key, TValue Value) tuple
        )
            where TKey : notnull
        {
            return new KeyValuePair<TKey, TValue>(tuple.Key, tuple.Value);
        }
    }
}
