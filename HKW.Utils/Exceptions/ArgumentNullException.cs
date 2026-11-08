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
        /// <typeparam name="T">目标类型; 仅非可空值类型拒绝空值, 引用类型的可空标注不能在运行时区分</typeparam>
        /// <param name="value">待赋给目标类型的对象值; 本方法仅检查空值, 不执行类型转换</param>
        /// <param name="paramName">参数名</param>
        /// <exception cref="ArgumentNullException">不可为 <see langword="null"/> 的值为 <see langword="null"/></exception>
        public static void ThrowIfNotNullableIsNull<T>(
            object? value,
            [CallerArgumentExpression("value")] string? paramName = null
        )
        {
            if (default(T) is not null)
            {
                ArgumentNullException.ThrowIfNull(value, paramName);
            }
        }
    }
}
