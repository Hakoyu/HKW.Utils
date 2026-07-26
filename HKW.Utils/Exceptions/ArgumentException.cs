using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
/// 参数异常
/// </summary>
public static partial class ArgumentExceptions
{
    extension(ArgumentException)
    {
        /// <summary>
        /// 当 <paramref name="argument"/> 与 <paramref name="expected"/> 相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与 <paramref name="expected"/> 不相等时抛出。</exception>
        public static void ThrowIfEquals<T>(
            T argument,
            T expected,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (EqualityComparer<T>.Default.Equals(argument, expected) is false)
                return;
            throw new ArgumentException(
                $"Argument value does match the expected value. Actual: [{argument?.ToString() ?? "null"}], Expected: [{expected?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 与 <paramref name="expected"/> 不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与 <paramref name="expected"/> 不相等时抛出。</exception>
        public static void ThrowIfNotEquals<T>(
            T argument,
            T expected,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (EqualityComparer<T>.Default.Equals(argument, expected))
                return;
            throw new ArgumentException(
                $"Argument value does not match the expected value. Actual: [{argument?.ToString() ?? "null"}], Expected: [{expected?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        #region ThrowIfAllNotEquals
        /// <summary>
        /// 当 <paramref name="argument"/> 与 <paramref name="expected1"/>全部 expected 参数不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected1">期望匹配的目标值。</param>
        /// <param name="expected2">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与任意 expected 参数不相等时抛出。</exception>
        public static void ThrowIfAllNotEquals<T>(
            T argument,
            T expected1,
            T expected2,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (
                EqualityComparer<T>.Default.Equals(argument, expected1)
                || EqualityComparer<T>.Default.Equals(argument, expected2)
            )
                return;
            throw new ArgumentException(
                $"Argument value does not match the all expected values. Actual: [{argument?.ToString() ?? "null"}], Expecteds: [{expected1?.ToString() ?? "null"}, {expected2?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 与全部 expected 参数不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected1">期望匹配的目标值。</param>
        /// <param name="expected2">期望匹配的目标值。</param>
        /// <param name="expected3">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与全部 expected 参数不相等时抛出。</exception>
        public static void ThrowIfAllNotEquals<T>(
            T argument,
            T expected1,
            T expected2,
            T expected3,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (
                EqualityComparer<T>.Default.Equals(argument, expected1)
                || EqualityComparer<T>.Default.Equals(argument, expected2)
                || EqualityComparer<T>.Default.Equals(argument, expected3)
            )
                return;
            throw new ArgumentException(
                $"Argument value does not match the all expected values. Actual: [{argument?.ToString() ?? "null"}], Expecteds: [{expected1?.ToString() ?? "null"}, {expected2?.ToString() ?? "null"}, {expected3?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 与全部 expected 参数不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected1">期望匹配的目标值。</param>
        /// <param name="expected2">期望匹配的目标值。</param>
        /// <param name="expected3">期望匹配的目标值。</param>
        /// <param name="expected4">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与全部 expected 参数不相等时抛出。</exception>
        public static void ThrowIfAllNotEquals<T>(
            T argument,
            T expected1,
            T expected2,
            T expected3,
            T expected4,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (
                EqualityComparer<T>.Default.Equals(argument, expected1)
                || EqualityComparer<T>.Default.Equals(argument, expected2)
                || EqualityComparer<T>.Default.Equals(argument, expected3)
                || EqualityComparer<T>.Default.Equals(argument, expected4)
            )
                return;
            throw new ArgumentException(
                $"Argument value does not match the all expected values. Actual: [{argument?.ToString() ?? "null"}], Expecteds: [{expected1?.ToString() ?? "null"}, {expected2?.ToString() ?? "null"}, {expected3?.ToString() ?? "null"}, {expected4?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 与全部 expected 参数不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expected1">期望匹配的目标值。</param>
        /// <param name="expected2">期望匹配的目标值。</param>
        /// <param name="expected3">期望匹配的目标值。</param>
        /// <param name="expected4">期望匹配的目标值。</param>
        /// <param name="expected5">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与全部 expected 参数不相等时抛出。</exception>
        public static void ThrowIfAllNotEquals<T>(
            T argument,
            T expected1,
            T expected2,
            T expected3,
            T expected4,
            T expected5,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (
                EqualityComparer<T>.Default.Equals(argument, expected1)
                || EqualityComparer<T>.Default.Equals(argument, expected2)
                || EqualityComparer<T>.Default.Equals(argument, expected3)
                || EqualityComparer<T>.Default.Equals(argument, expected4)
                || EqualityComparer<T>.Default.Equals(argument, expected5)
            )
                return;
            throw new ArgumentException(
                $"Argument value does not match the all expected values. Actual: [{argument?.ToString() ?? "null"}], Expecteds: [{expected1?.ToString() ?? "null"}, {expected2?.ToString() ?? "null"}, {expected3?.ToString() ?? "null"}, {expected4?.ToString() ?? "null"}, {expected5?.ToString() ?? "null"}], ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 与 <paramref name="expecteds"/> 集合内所有不相等时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="expecteds">期望匹配的目标值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 与 <paramref name="expecteds"/> 不相等时抛出。</exception>
        public static void ThrowIfAllNotEquals<T>(
            T argument,
            IEnumerable<T> expecteds,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (expecteds is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(argument, list[i]))
                        return;
                }
            }
            else
            {
#pragma warning disable S3267
                foreach (var item in expecteds)
                {
                    if (EqualityComparer<T>.Default.Equals(argument, item))
                        return;
                }
#pragma warning restore S3267
            }

            throw new ArgumentException(
                $"Argument value does not match the all expected values. Actual: [{argument?.ToString() ?? "null"}], Expected: {string.Join(", ", expecteds)}, ArgumentType: [{argument?.GetType().ToString() ?? "null"}].",
                paramName
            );
        }
        #endregion

        /// <summary>
        /// 当 <paramref name="argument"/> 为只读集合时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 只读集合时抛出。</exception>
        public static void ThrowIfReadOnlyCollection<T>(
            ICollection<T> argument,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (argument.IsReadOnly)
                throw new ArgumentException(ExceptionMessage.IsReadOnlyCollection, paramName);
        }

        /// <summary>
        /// 当 <paramref name="argument"/> 为空集合时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <param name="argument">需要校验的参数值。</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="System.ArgumentException">当 <paramref name="argument"/> 为空集合时抛出。</exception>
        public static void ThrowIfEmptyCollection<T>(
            ICollection<T> argument,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (argument.Count == 0)
                throw new ArgumentException("Collection is empty.", paramName);
        }

        /// <summary>
        /// 当 <paramref name="collection"/> 集合不包含 <paramref name="argument"/>时，抛出 <see cref="System.ArgumentException"/>。
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="argument">项</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="ArgumentException">当 <paramref name="collection"/> 集合不包含 <paramref name="argument"/>时抛出。</exception>
        public static void ThrowIfNotContains<T>(
            ICollection<T> collection,
            T argument,
            [CallerArgumentExpression("argument")] string? paramName = null
        )
        {
            if (collection.Contains(argument) is false)
                throw new ArgumentException($"Item not contains, Item: [{argument}].", paramName);
        }
    }
}
