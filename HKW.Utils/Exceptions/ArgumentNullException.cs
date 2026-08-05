using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
///
/// </summary>
public static class ArgumentNullExceptions
{
    extension(ArgumentNullException)
    {
        /// <summary>
        /// 当不可为 <see langword="null"/> 的值为 <see langword="null"/> 时抛出异常
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="ArgumentException">不可为 <see langword="null"/> 的值为 <see langword="null"/></exception>
        public static void ThrowIfNotNullableIsNull<T>(
            T value,
            [CallerArgumentExpression("value")] string? paramName = null
        )
        {
            if (default(T) is not null)
            {
                ArgumentNullException.ThrowIfNull(value);
            }
        }
    }
}
