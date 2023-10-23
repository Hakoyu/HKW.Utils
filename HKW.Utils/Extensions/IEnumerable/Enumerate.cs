using System;
using System.Collections;
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
    /// <param name="collection">集合</param>
    /// <returns>带有索引的枚举值</returns>
    public static IEnumerable<ItemInfo> Enumerate(this IEnumerable collection)
    {
        var index = 0;
        foreach (var item in collection)
            yield return new(index++, item);
    }
}

/// <summary>
/// 项信息
/// </summary>
[DebuggerDisplay("[{Index}, {Value}]")]
public readonly struct ItemInfo
{
    /// <summary>
    /// 索引值
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 值
    /// </summary>
    public object Value { get; }

    /// <inheritdoc/>
    /// <param name="value">值</param>
    /// <param name="index">索引值</param>
    public ItemInfo(int index, object value)
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
