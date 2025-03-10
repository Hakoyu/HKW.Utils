﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试使用索引获取列表的值
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">索引</param>
    /// <param name="value">项目</param>
    /// <returns>成功获取值为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(
        this IList<T> list,
        int index,
        [MaybeNullWhen(false)] out T value
    )
    {
        if (list.ContainsIndex(index))
        {
            value = list[index];
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}
