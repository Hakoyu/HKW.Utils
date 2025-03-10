﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFind<T>(
        this IList<T> list,
        Predicate<T> match,
        [MaybeNullWhen(false)] out T item
    )
    {
        var index = list.FindIndex(match);
        item = list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始索引</param>
    /// <param name="match">条件</param>
    /// <param name="itemInfo">项目和索引</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFind<T>(
        this IList<T> list,
        int startIndex,
        Predicate<T> match,
        out (int Index, T Value) itemInfo
    )
    {
        var index = list.FindIndex(startIndex, match);
        itemInfo = (index, list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始索引</param>
    /// <param name="count">索引</param>
    /// <param name="match">条件</param>
    /// <param name="itemInfo">项目和索引</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFind<T>(
        this IList<T> list,
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) itemInfo
    )
    {
        var index = list.FindIndex(startIndex, count, match);
        itemInfo = (index, list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }
}
