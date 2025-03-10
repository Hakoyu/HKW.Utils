﻿using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 删除标签
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">枚举</param>
    /// <param name="flag">标签</param>
    /// <returns>删除标签的枚举</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag)
        where TEnum : struct, Enum
    {
        var type = EnumInfo<TEnum>.UnderlyingType;
        return (TEnum)
            NumberUtils.BitwiseOperatorF(
                value,
                NumberUtils.BitwiseComplementF(flag, type),
                type,
                BitwiseOperatorType.And
            );
    }
}
