using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 改变源自身
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="func">方法</param>
    /// <returns>改变后的源</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTarget To<TSource, TTarget>(this TSource source, Func<TSource, TTarget> func)
    {
        return func(source);
    }

    /// <summary>
    /// 转换到默认
    /// <para>不直接使用,常作为占位符</para>
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns>默认类型</returns>
    public static T ToDefault<T>(this object obj)
    {
        return default!;
    }
}
