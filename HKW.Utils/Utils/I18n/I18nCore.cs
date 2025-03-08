using System.ComponentModel;
using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 可观测本地化
/// <para>用例:
/// <c>I18nCore.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN")</c>
/// </para>
/// </summary>
public class I18nCore
{
    /// <summary>
    /// 改变 <see cref="CurrentCulture"/> 同时修改 <see cref="Thread.CurrentThread"/> 的 <see cref="Thread.CurrentCulture"/>
    /// </summary>
    [DefaultValue(false)]
    public bool ChangeThreadCulture { get; set; } = false;

    /// <summary>
    /// 改变 <see cref="CurrentCulture"/> 同时修改 <see cref="Thread.CurrentThread"/> 的 <see cref="Thread.CurrentUICulture"/>
    /// </summary>
    [DefaultValue(false)]
    public bool ChangeThreadUICulture { get; set; } = false;

    /// <summary>
    /// 本地化资源实例集合
    /// <para>(ResourceName, I18nResourceInfo)</para>
    /// </summary>
    public HashSet<II18nResource> I18nResources { get; } = new();

    private CultureInfo _currentCulture = CultureInfo.CurrentCulture;

    /// <summary>
    /// 当前文化
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            var isEqual = _currentCulture == value;
            _currentCulture = value;

            if (ChangeThreadCulture)
                Thread.CurrentThread.CurrentCulture = value;
            if (ChangeThreadUICulture)
                Thread.CurrentThread.CurrentUICulture = value;
            if (isEqual is false)
                RefreshAllI18nResource();
        }
    }

    /// <summary>
    /// 创建I18n资源
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    /// <param name="addCurrentCulture">为资源添加当前文化</param>
    /// <returns>I18n资源</returns>
    public I18nResource<TKey, TValue> CreateResource<TKey, TValue>(bool addCurrentCulture = false)
        where TKey : notnull
    {
        return new I18nResource<TKey, TValue>(this, addCurrentCulture);
    }

    /// <summary>
    /// 刷新所有I18n资源
    /// </summary>
    public void RefreshAllI18nResource()
    {
        CurrentCultureChanged?.Invoke(this, new(CurrentCulture));
        foreach (var resource in I18nResources)
            resource.RefreshAllI18nObject();
    }

    #region BindingValue
    /// <summary>
    /// 绑定两个值, 在触发 <see cref="CurrentCultureChanged"/> 时会对目标重新赋值
    /// <para>示例:
    /// <code>target = source</code>
    /// 等同于:
    /// <code><![CDATA[
    /// target = ObservableI18nCore.BindingValue((value) => target = value, () => source);
    /// ]]></code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="setTargetValue">设置目标值</param>
    /// <param name="getSourceValue">获取源值</param>
    /// <returns>源的返回值</returns>
    public T BindingValue<T>(Action<T> setTargetValue, Func<T> getSourceValue)
    {
        CurrentCultureChanged -= I18nCore_CurrentCultureChanged;
        CurrentCultureChanged += I18nCore_CurrentCultureChanged;
        return getSourceValue();
        void I18nCore_CurrentCultureChanged(I18nCore sender, CultureChangedEventArgs e)
        {
            setTargetValue(getSourceValue());
        }
    }

    /// <summary>
    /// 绑定两个值, 在触发 <see cref="CurrentCultureChanged"/> 时会对目标重新赋值
    /// <para>示例:
    /// <code>target.Value = source</code>
    /// 等同于:
    /// <code><![CDATA[
    /// target.Value = ObservableI18nCore.BindingValue(target, (value, target) => target.Value = value, () => source);
    /// ]]></code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="target">目标</param>
    /// <param name="setTargetValue">设置目标值</param>
    /// <param name="getSourceValue">获取源值</param>
    /// <returns>源的返回值</returns>
    public T BindingValue<T, TTarget>(
        TTarget target,
        Action<T, TTarget> setTargetValue,
        Func<T> getSourceValue
    )
        where TTarget : class
    {
        CurrentCultureChanged -= I18nCore_CurrentCultureChanged;
        CurrentCultureChanged += I18nCore_CurrentCultureChanged;
        return getSourceValue();
        void I18nCore_CurrentCultureChanged(I18nCore sender, CultureChangedEventArgs e)
        {
            setTargetValue(getSourceValue(), target);
        }
    }

    /// <summary>
    /// 使用弱引用绑定目标, 在触发 <see cref="CurrentCultureChanged"/> 时会对目标重新赋值
    /// <para>示例:
    /// <code>target.Value = source</code>
    /// 等同于:
    /// <code><![CDATA[
    /// target.Value = ObservableI18nCore.BindingValueWithWeakReference(target, (value, target) => target.Value = value, () => source);
    /// ]]></code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="target">目标</param>
    /// <param name="setTargetValue">设置目标值</param>
    /// <param name="getSourceValue">获取源值</param>
    /// <returns>源的返回值</returns>
    public T BindingValueWithWeakReference<T, TTarget>(
        TTarget target,
        Action<T, TTarget> setTargetValue,
        Func<T> getSourceValue
    )
        where TTarget : class
    {
        var weakReference = new WeakReference<TTarget>(target);
        CurrentCultureChanged += CultureChangedEvent;
        return getSourceValue();

        void CultureChangedEvent(object? sender, CultureChangedEventArgs e)
        {
            if (weakReference.TryGetTarget(out var target))
                setTargetValue(getSourceValue(), target);
            else
                CurrentCultureChanged -= CultureChangedEvent;
        }
    }
    #endregion

    /// <summary>
    /// 清除所有I18n资源
    /// </summary>
    public void ClearI18nResources()
    {
        foreach (var resource in I18nResources)
            resource.I18nCore = null;
    }

    /// <summary>
    /// 文化改变后事件
    /// </summary>
    public event CultureChangedEventHander? CurrentCultureChanged;
}
