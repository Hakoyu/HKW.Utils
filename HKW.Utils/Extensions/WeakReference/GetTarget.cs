namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取目标
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="weakReference">弱引用</param>
    /// <returns>获取成功返回目标值, 获取失败则返回 <see langword="null"/></returns>
    public static T? GetTarget<T>(this WeakReference<T> weakReference)
        where T : class?
    {
        return weakReference.TryGetTarget(out var t) ? t : null;
    }
}
