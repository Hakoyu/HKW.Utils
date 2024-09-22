using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 包含索引值
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="span">内存</param>
    /// <param name="index">索引</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsIndex<T>(this Span<T> span, int index)
    {
        if (index >= 0 && index < span.Length)
            return true;
        return false;
    }
}
