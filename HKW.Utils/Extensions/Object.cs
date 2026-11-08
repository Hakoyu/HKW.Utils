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
    /// <returns>完成转换的对象; 引用类型或可空值类型接收空引用时返回空值</returns>
    /// <exception cref="InvalidCastException">对象不能直接转换为目标类型</exception>
    /// <exception cref="NullReferenceException">将空引用拆箱为非可空值类型</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CastTo<T>(this object? obj)
    {
        return (T)obj!;
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
}
