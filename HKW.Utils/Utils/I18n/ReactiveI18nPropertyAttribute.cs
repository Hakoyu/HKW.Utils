﻿using HKW.HKWReactiveUI;

namespace HKW.HKWUtils;

/// <summary>
/// I18n属性
/// <para>
/// 示例:
/// <code><![CDATA[
/// partial class MyViewModel : ReactiveObject
/// {
///     [ReactiveProperty]
///     public string ID { get; set; } = string.Empty;
///
///     [I18nProperty("Program.I18nResource", nameof(ID), true)]
///     public string Name
///     {
///         get => Program.I18nResource.GetCurrentCultureDataOrDefault(ID);
///         set => Program.I18nResource.SetCurrentCultureData(ID, value);
///     }
/// }
/// ]]></code>
/// </para>
/// 这样就会生成代码
/// <code><![CDATA[
/// partial class MyViewModel : ReactiveObject
/// {
///     public string ID
///     {
///         get => $ID;
///         set => this.RaiseAndSetIfChanged(ref $ID, value);
///     }
///
///     [I18nProperty("Program.I18nResource", nameof(ID), true)]
///     public string Name
///     {
///         get => Program.I18nResource.GetCurrentCultureDataOrDefault(ID);
///         set => Program.I18nResource.SetCurrentCultureData(ID, value);
///     }
///
///     protected void InitializeReactiveObject()
///     {
///         Program.I18nResource.I18nObjects.Add(new(this));
///         var i18nObject = Program.I18nResource.I18nObjects.Last();
///         i18nObject.AddProperty(nameof(ID), x => ((TestModel) x).ID, nameof(Name), true);
///     }
/// }
/// ]]></code></summary>
/// <remarks>
/// 如果继承了 <see cref="ReactiveObjectX"/> 则会重写 <see cref="ReactiveObjectX.InitializeReactiveObject"/> 方法,不需要手动运行
/// <para>
/// 否则需要手动运行生成的 <see langword="InitializeReactiveObject"/> 方法
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ReactiveI18nPropertyAttribute : Attribute
{
    ///<inheritdoc/>
    /// <param name="ResourceName">资源名称</param>
    /// <param name="KeyPropertyName">键属性值</param>
    /// <param name="RetentionValueOnKeyChange">当键改变时保留值</param>
    public ReactiveI18nPropertyAttribute(
        string ResourceName,
        string KeyPropertyName,
        bool RetentionValueOnKeyChange = false
    )
    {
        this.ResourceName = ResourceName;
        this.KeyPropertyName = KeyPropertyName;
        this.RetentionValueOnKeyChange = RetentionValueOnKeyChange;
    }

    ///<inheritdoc/>
    /// <param name="ResourceName">资源名称</param>
    /// <param name="ObjectName">对象名称</param>
    /// <param name="KeyPropertyName">键属性值</param>
    /// <param name="RetentionValueOnKeyChange">当键改变时保留值</param>
    public ReactiveI18nPropertyAttribute(
        string ResourceName,
        string ObjectName,
        string KeyPropertyName,
        bool RetentionValueOnKeyChange = false
    )
    {
        this.ResourceName = ResourceName;
        this.ObjectName = ObjectName;
        this.KeyPropertyName = KeyPropertyName;
        this.RetentionValueOnKeyChange = RetentionValueOnKeyChange;
    }

    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// 对象名称
    /// </summary>
    public string ObjectName { get; } = string.Empty;

    /// <summary>
    /// 键属性值
    /// </summary>
    public string KeyPropertyName { get; }

    /// <summary>
    /// 当键改变时保留值
    /// </summary>
    public bool RetentionValueOnKeyChange { get; }
}
