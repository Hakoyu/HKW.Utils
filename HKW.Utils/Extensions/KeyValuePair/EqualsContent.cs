using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 内容相同
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="pair1">键值对1</param>
    /// <param name="pair2">键值对2</param>
    /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsContent<TKey, TValue>(
        this KeyValuePair<TKey, TValue> pair1,
        KeyValuePair<TKey, TValue>? pair2
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pair1);
        ArgumentNullException.ThrowIfNull(pair2);
        return pair1.Key?.Equals(pair2.Value.Key) is true
            && pair1.Value?.Equals(pair2.Value.Value) is true;
    }

    /// <summary>
    /// 内容相同
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="pair1">键值对1</param>
    /// <param name="pair2">键值对2</param>
    /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsContent<TKey, TValue>(
        this KeyValuePair<TKey, TValue> pair1,
        (TKey Key, TValue Value)? pair2
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pair1);
        ArgumentNullException.ThrowIfNull(pair2);
        return pair1.Key?.Equals(pair2.Value.Key) is true
            && pair1.Value?.Equals(pair2.Value.Value) is true;
    }

    /// <summary>
    /// 内容相同
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="pair1">键值对</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>内容相同为 <see langword="true"/> 不相同为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsContent<TKey, TValue>(
        this KeyValuePair<TKey, TValue> pair1,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pair1);
        return pair1.Key?.Equals(key) is true && pair1.Value?.Equals(value) is true;
    }
}
