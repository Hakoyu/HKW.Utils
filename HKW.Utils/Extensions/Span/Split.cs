﻿using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 分割Span
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="span">源</param>
    /// <param name="separator">分割符</param>
    /// <param name="removeEmptyEntries">删除空白项</param>
    /// <returns>枚举器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanSplitEnumerator<T> Split<T>(
        this Span<T> span,
        T separator,
        bool removeEmptyEntries = false
    )
        where T : IEquatable<T>
    {
        return new SpanSplitEnumerator<T>(span, separator, removeEmptyEntries);
    }

    /// <summary>
    /// 分割Span&lt;char&gt;
    /// </summary>
    /// <param name="span">源</param>
    /// <param name="stringSplitOptions">字符串分割设置</param>
    /// <param name="separator">分割符</param>
    /// <returns>枚举器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CharSpanSplitEnumerator Split(
        this Span<char> span,
        StringSplitOptions stringSplitOptions,
        params char[] separator
    )
    {
        return new CharSpanSplitEnumerator(span, separator, stringSplitOptions);
    }
}
