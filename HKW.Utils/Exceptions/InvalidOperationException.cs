using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
/// 不支持的异常
/// </summary>
public static partial class InvalidOperationExceptions
{
    extension(InvalidOperationException)
    {
        /// <summary>
        /// 当 <paramref name="collection"/> 为空集合时, 抛出异常.
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="paramName">集合参数名</param>
        /// <remarks>
        /// 已知集合优先读取数量; 其他序列仅尝试读取首项并释放枚举器, 不保留该项.
        /// </remarks>
        /// <exception cref="InvalidOperationException">当 <paramref name="collection"/> 为空时抛出. </exception>
        public static void ThrowIfEmptyCollection<T>(
            IEnumerable<T> collection,
            [CallerArgumentExpression("collection")] string? paramName = null
        )
        {
            ArgumentNullException.ThrowIfNull(collection, paramName);
            if (
                collection is IReadOnlyCollection<T> readOnly
                    ? readOnly.Count > 0
                    : collection.Any()
            )
                return;
            throw new InvalidOperationException($"Collection \"{paramName}\" is empty");
        }

        /// <summary>
        /// 当 <paramref name="collection"/> 为空集合时, 抛出异常.
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="paramName">集合参数名</param>
        /// <exception cref="InvalidOperationException">当 <paramref name="collection"/> 为空时抛出. </exception>
        public static void ThrowIfEmptyCollection<T>(
            IReadOnlyCollection<T> collection,
            [CallerArgumentExpression("collection")] string? paramName = null
        )
        {
            ArgumentNullException.ThrowIfNull(collection, paramName);
            if (collection.Count > 0)
                return;
            throw new InvalidOperationException($"Collection \"{paramName}\" is empty");
        }
    }
}
