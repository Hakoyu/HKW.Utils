using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
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
        /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContentEquals(KeyValuePair<TKey, TValue> pair2)
        {
            return EqualityComparer<TKey>.Default.Equals(pair1.Key, pair2.Key)
                && EqualityComparer<TValue>.Default.Equals(pair1.Value, pair2.Value);
        }

        /// <summary>
        /// 内容相同
        /// </summary>
        /// <param name="pair2">键值对2</param>
        /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContentEquals((TKey Key, TValue Value) pair2)
        {
            return EqualityComparer<TKey>.Default.Equals(pair1.Key, pair2.Key)
                && EqualityComparer<TValue>.Default.Equals(pair1.Value, pair2.Value);
        }

        /// <summary>
        /// 内容相同
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContentEquals(TKey key, TValue value)
        {
            return EqualityComparer<TKey>.Default.Equals(pair1.Key, key)
                && EqualityComparer<TValue>.Default.Equals(pair1.Value, value);
        }
    }
}
