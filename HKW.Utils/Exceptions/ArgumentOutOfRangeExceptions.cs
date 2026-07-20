using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
/// 参数超出范围异常
/// </summary>
public static class ArgumentOutOfRangeExceptions
{
    extension(ArgumentOutOfRangeException exception)
    {
        /// <summary>
        /// 当数值不在范围内时触发异常
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="value">数值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="paramName">参数名</param>
        public static void ThrowIfOutOfRangeInclusive<T>(
            T value,
            T min,
            T max,
            [CallerArgumentExpression("value")] string? paramName = null
        )
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, min, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, max, paramName);
        }

        /// <summary>
        /// 当数值不在或等于范围内时触发异常
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="value">数值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="paramName">参数名</param>
        public static void ThrowIfOutOfRangeExclusive<T>(
            T value,
            T min,
            T max,
            [CallerArgumentExpression("value")] string? paramName = null
        )
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, min, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, max, paramName);
        }

        /// <summary>
        /// 当数值不在范围内或等于最小值时触发异常
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="value">数值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="paramName">参数名</param>
        public static void ThrowIfOutOfRangeMinExclusive<T>(
            T value,
            T min,
            T max,
            [CallerArgumentExpression("value")] string? paramName = null
        )
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, min, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, max, paramName);
        }

        /// <summary>
        /// 当数值不在范围内或等于最大值时触发异常
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="value">数值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="paramName">参数名</param>
        public static void ThrowIfOutOfRangeMaxExclusive<T>(
            T value,
            T min,
            T max,
            [CallerArgumentExpression("value")] string? paramName = null
        )
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, min, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, max, paramName);
        }
    }
}
