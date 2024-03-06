﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public static bool TryFindLast<T>(
        this List<T> list,
        Predicate<T> match,
        [MaybeNullWhen(false)] out T item
    )
    {
        var index = list.FindLastIndex(match);
        item = list.GetValueOrDefault(index);
        return index == -1 ? false : true;
    }

    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public static bool TryFindLast<T>(
        this List<T> list,
        int startIndex,
        Predicate<T> match,
        out (int, T) item
    )
    {
        var index = list.FindLastIndex(startIndex, match);
        item = (index, list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }

    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">索引</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public static bool TryFindLast<T>(
        this List<T> list,
        int startIndex,
        int count,
        Predicate<T> match,
        out (int, T) item
    )
    {
        var index = list.FindLastIndex(startIndex, count, match);
        item = (index, list.GetValueOrDefault(index)!);
        return index == -1 ? false : true;
    }
}
