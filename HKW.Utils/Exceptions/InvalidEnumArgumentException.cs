using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
/// 提供 <see cref="InvalidEnumArgumentException"/> 相关的扩展辅助方法
/// </summary>
public static class InvalidEnumArgumentExceptions
{
    extension(InvalidEnumArgumentException)
    {
        /// <summary>
        /// 当指定枚举类型不支持按位标志用法时抛出异常
        /// </summary>
        /// <param name="enumInfo">要检查的枚举值信息</param>
        /// <exception cref="InvalidEnumArgumentException">
        /// 当 <paramref name="enumInfo.IsFlagable"/> 为 <see langword="false"/> 时抛出
        /// </exception>
        public static void ThrowIfNotFlaggable(IEnumInfo enumInfo)
        {
            if (enumInfo.IsFlaggable is false)
                throw new InvalidEnumArgumentException(
                    $"Enum type '{enumInfo.EnumType.FullName}' must be decorated with '{nameof(FlagsAttribute)}' to support flag operations."
                );
        }
    }
}
