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
