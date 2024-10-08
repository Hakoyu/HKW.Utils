﻿using System.Runtime.CompilerServices;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 将普通集合转换为只读集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="set">集合</param>
    /// <returns>只读集合</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(set, nameof(set));
        return new ReadOnlySet<T>(set);
    }
}
