using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
///
/// </summary>
public static class KeyNotFoundExceptionExtensions
{
    extension(KeyNotFoundException)
    {
        /// <summary>
        /// 如果键不存在则异常
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="KeyNotFoundException">键不存在</exception>
        public static void ThrowIfKeyNotFound<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            out TValue value
        )
        {
            if (dictionary.TryGetValue(key, out value!) is false)
                throw new KeyNotFoundException(
                    $"The given key '{key}' was not present in the dictionary."
                );
        }
    }
}
