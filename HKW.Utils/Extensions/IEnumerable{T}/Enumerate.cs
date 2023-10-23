using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 枚举出带有索引值的枚举值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection">集合</param>
    /// <returns>带有索引的枚举值</returns>
    public static IEnumerable<ItemInfo<T>> Enumerate<T>(this IEnumerable<T> collection)
    {
        var index = 0;
        foreach (var item in collection)
            yield return new(index++, item);
    }
}

/// <summary>
/// 项信息
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("[{Index}, {Value}]")]
public readonly struct ItemInfo<T>
{
    /// <summary>
    /// 索引值
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; }

    /// <inheritdoc/>
    /// <param name="value">值</param>
    /// <param name="index">索引值</param>
    public ItemInfo(int index, T value)
    {
        Index = index;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Index}, {Value}]";
    }
}
