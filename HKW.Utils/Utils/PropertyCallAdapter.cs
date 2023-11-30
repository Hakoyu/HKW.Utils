using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace HKW.HKWUtils;

/// <summary>
/// 属性调用适配器提供者
/// </summary>
/// <typeparam name="TTarget">目标类型</typeparam>
public class PropertyCallAdapterProvider<TTarget>
{
    private static readonly Dictionary<string, IPropertyCallAdapter<TTarget>> _instances = new();

    /// <summary>
    /// 获取实例
    /// <para>示例:
    /// <code><![CDATA[
    /// PropertyCallAdapterProvider<ObjectType>.GetInstance(propertyName).InvokeGet(object)
    /// ]]></code>
    /// </para>
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>属性调用适配器接口</returns>
    public static IPropertyCallAdapter<TTarget> GetInstance(string propertyName)
    {
        if (_instances.TryGetValue(propertyName, out var instance))
            return instance;
        // 获取属性信息
        var property =
            typeof(TTarget).GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            ) ?? throw new Exception("Property not exist");
        // 获取属性 getter 信息
        var getMethod =
            property.GetGetMethod(true) ?? throw new Exception("Property getter not exist");
        // 设置委托类型
        var concreteGetterType = typeof(Func<,>).MakeGenericType(
            typeof(TTarget),
            property.PropertyType
        );
        // 创建委托
        var getterInvocation = Delegate.CreateDelegate(concreteGetterType, null, getMethod);
        var concreteAdapterType = typeof(PropertyCallAdapter<,>).MakeGenericType(
            typeof(TTarget),
            property.PropertyType
        );
        instance =
            Activator.CreateInstance(concreteAdapterType, getterInvocation)
                as IPropertyCallAdapter<TTarget>
            ?? throw new Exception("Instance creation failed");
        // 保存实例信息
        _instances.Add(propertyName, instance);
        return instance;
    }

    /// <summary>
    /// 尝试获取实例
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <param name="propertyCallAdapter">属性调用适配器接口</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetInstance(
        string propertyName,
        [MaybeNullWhen(false)] out IPropertyCallAdapter<TTarget> propertyCallAdapter
    )
    {
        if (_instances.TryGetValue(propertyName, out var instance))
        {
            propertyCallAdapter = instance;
            return true;
        }
        // 获取属性信息
        var property = typeof(TTarget).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        if (property is null)
        {
            propertyCallAdapter = null;
            return false;
        }
        // 获取属性 getter 信息
        var getMethod = property.GetGetMethod(true);
        if (getMethod is null)
        {
            propertyCallAdapter = null;
            return false;
        }
        // 设置委托类型
        var concreteGetterType = typeof(Func<,>).MakeGenericType(
            typeof(TTarget),
            property.PropertyType
        );
        // 创建委托
        var getterInvocation = Delegate.CreateDelegate(concreteGetterType, null, getMethod);
        var concreteAdapterType = typeof(PropertyCallAdapter<,>).MakeGenericType(
            typeof(TTarget),
            property.PropertyType
        );
        instance =
            Activator.CreateInstance(concreteAdapterType, getterInvocation)
            as IPropertyCallAdapter<TTarget>;
        if (instance is null)
        {
            propertyCallAdapter = null;
            return false;
        }
        // 保存实例信息
        _instances.Add(propertyName, instance);
        propertyCallAdapter = instance;
        return true;
    }
}

/// <summary>
/// 属性调用适配器
/// </summary>
/// <typeparam name="TTarget">目标类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public class PropertyCallAdapter<TTarget, TResult> : IPropertyCallAdapter<TTarget>
{
    private readonly Func<TTarget, TResult> _getterInvocation;

    /// <summary>
    ///
    /// </summary>
    /// <param name="getterInvocation"></param>
    public PropertyCallAdapter(Func<TTarget, TResult> getterInvocation)
    {
        _getterInvocation = getterInvocation;
    }

    /// <inheritdoc/>
    public object? InvokeGet(TTarget target)
    {
        return _getterInvocation.Invoke(target);
    }
}

/// <summary>
/// 属性调用适配器接口
/// </summary>
/// <typeparam name="TTarget">目标类型</typeparam>
public interface IPropertyCallAdapter<TTarget>
{
    /// <summary>
    /// 调用获取
    /// </summary>
    /// <param name="target">目标</param>
    /// <returns>返回值</returns>
    public object? InvokeGet(TTarget target);
}
