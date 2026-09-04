//using System.Collections.Concurrent;
//using Splat;

//namespace HKW;

///// <summary>
///// 启用记录仪
///// <para>
///// 配合 <see cref="LogHostHelper.LogX{T}(T)"/> 可获取 <typeparamref name="T"/> 注册的日志管理器
///// </para>
///// </summary>
///// <typeparam name="T"></typeparam>
//#pragma warning disable S2326 // Unused type parameters should be removed
//public interface IEnableLogger<T> : IEnableLogger
//#pragma warning restore S2326 // Unused type parameters should be removed
//    where T : IEnableLogger { }

///// <summary>
/////
///// </summary>
//public static class LogHostHelper
//{
//    /// <summary>
//    /// 类型和记录器, (Type, LogManager)
//    /// </summary>
//    private static readonly ConcurrentDictionary<Type, ILogManager> _logManagerByType = [];

//    /// <summary>
//    /// 获取父类指定的日志记录器, 或者 <see cref="IEnableLogger{T}"/> 指定的类型日志记录器
//    /// <para>
//    /// 如果有多个 <see cref="IEnableLogger{T}"/>, 则以 <c>Type.GetInterfaces()</c> 最末尾的第一个为准
//    /// </para>
//    /// <para>
//    /// 需要在 <see cref="RegisterLoggerManager"/> 先设置日志记录器
//    /// </para>
//    /// </summary>
//    /// <typeparam name="T">类型</typeparam>
//    /// <param name="logClassInstance">源</param>
//    /// <returns>记录器</returns>
//    /// <exception cref="InvalidOperationException"></exception>
//#pragma warning disable IDE0060 // 删除未使用的参数
//    public static IFullLogger LogX<T>(this T logClassInstance)
//#pragma warning restore IDE0060 // 删除未使用的参数
//        where T : IEnableLogger
//    {
//        var type = typeof(T);
//        if (_logManagerByType.TryGetValue(type, out var service) is false)
//        {
//            var t = typeof(T);
//            do
//            {
//                t = t.BaseType;
//            } while (t is not null && _logManagerByType.TryGetValue(t, out service) is false);
//        }
//        if (service is null)
//        {
//            var its = type.GetInterfaces();
//            for (var i = its.Length - 1; i >= 0; i--)
//            {
//                if (its[i].Name != typeof(IEnableLogger<>).Name)
//                    continue;

//                _logManagerByType.TryGetValue(its[i].GenericTypeArguments[0], out service);
//                break;
//            }
//        }

//        if (service is not null)
//            return service.GetLogger<T>();
//        throw new InvalidOperationException(
//            "ILogManager is null. This should never happen, your dependency resolver is broken"
//        );
//    }

//    /// <summary>
//    /// 注册日志管理器
//    /// <para>
//    /// 之后所有 <paramref name="targetType"/> 的子类使用 <see cref="LogX{T}(T)"/> 时都会返回注册的日志管理器
//    /// </para>
//    /// </summary>
//    /// <param name="targetType">目标类型, 通常指定 ViewModelBase</param>
//    /// <param name="logManager">日志管理器</param>
//    public static void RegisterLoggerManager(Type targetType, ILogManager logManager)
//    {
//        _logManagerByType.TryAdd(targetType, logManager);
//    }

//    /// <summary>
//    /// 注销日志管理器
//    /// </summary>
//    /// <param name="targetType">目标类型, 通常指定 VieModelBase</param>
//    public static void UnregisterLoggerManager(Type targetType)
//    {
//        _logManagerByType.TryRemove(targetType, out _);
//    }
//}
