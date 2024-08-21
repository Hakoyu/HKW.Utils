using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试获取结果
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="nullableValue">可能为 <see langword="null"/> 的值</param>
    /// <param name="result">结果</param>
    /// <returns>值不为 <see langword="null"/> 返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResult<T>(this T? nullableValue, [MaybeNullWhen(false)] out T result)
    {
        result = nullableValue;
        if (result is null)
            return false;
        return true;
    }
}
