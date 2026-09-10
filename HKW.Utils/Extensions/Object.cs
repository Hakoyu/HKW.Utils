using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class ObjectExtensions
{
    /// <param name="obj">对象</param>
    extension(object obj)
    {
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <returns>强制转换类型的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T As<T>()
        {
            return (T)obj;
        }
    }

    /// <summary>
    /// 转换到
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="func">方法</param>
    /// <returns>转换结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTarget ConvertTo<TSource, TTarget>(
        this TSource source,
        Func<TSource, TTarget> func
    )
    {
        return func(source);
    }

    /// <summary>
    /// 尝试获取结果
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="nullableValue">可能为 <see langword="null"/> 的值</param>
    /// <param name="result">结果</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResult<T>(this T? nullableValue, [MaybeNullWhen(false)] out T result)
    {
        result = nullableValue;
        if (result is null)
            return false;
        return true;
    }
}
