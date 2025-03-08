using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 包含索引值
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="span">内存</param>
    /// <param name="index">索引</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsIndex<T>(this ReadOnlySpan<T> span, int index)
    {
        if (index >= 0 && index < span.Length)
            return true;
        return false;
    }
}
