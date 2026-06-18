using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <param name="obj">对象</param>
    extension(object obj)
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="func">方法</param>
        /// <returns>对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Create<T>(Func<T> func)
        {
            return func();
        }
    }

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
