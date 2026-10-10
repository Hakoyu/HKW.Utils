using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// 强制转换为目标类型
    /// </summary>
    /// <param name="obj">对象</param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>完成转换的对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CastTo<T>(this T obj)
    {
        return (T)obj;
    }

    /// <summary>
    /// 执行操作后返回原对象
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="action">操作</param>
    /// <returns>原对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Tap<T>(this T obj, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action(obj);
        return obj;
    }
    ///// <summary>
    ///// 转换到
    ///// </summary>
    ///// <typeparam name="TSource">源类型</typeparam>
    ///// <typeparam name="TTarget">目标类型</typeparam>
    ///// <param name="source">源</param>
    ///// <param name="func">方法</param>
    ///// <returns>转换结果</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static TTarget ConvertTo<TSource, TTarget>(
    //    this TSource source,
    //    Func<TSource, TTarget> func
    //)
    //{
    //    return func(source);
    //}

    ///// <summary>
    ///// 尝试获取结果
    ///// </summary>
    ///// <typeparam name="T">类型</typeparam>
    ///// <param name="nullableValue">可能为 <see langword="null"/> 的值</param>
    ///// <param name="result">结果</param>
    ///// <returns>是否成功</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static bool TryGetResult<T>(this T? nullableValue, [MaybeNullWhen(false)] out T result)
    //{
    //    result = nullableValue;
    //    if (result is null)
    //        return false;
    //    return true;
    //}
}
