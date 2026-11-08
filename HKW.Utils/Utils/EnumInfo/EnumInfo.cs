using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

/// <summary>
/// 枚举信息
/// </summary>
public static class EnumInfo
{
    /// <summary>
    /// 按类型区分的枚举信息字典, 你在操作它之前最好知道自己在做什么
    /// <para>
    /// (EnumType, (EnumValue, EnumInfo))
    /// </para>
    /// </summary>
    public static ConcurrentDictionary<Type, IDictionary<Enum, IEnumInfo>> InfosByType { get; } =
    [];

    #region Default

    /// <summary>
    /// 默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultToString
    {
        get => field ?? GlobalDefaultToString;
        set => field = value;
    }

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayName
    {
        get => field ?? GlobalDefaultGetDisplayName;
        set => field = value;
    }

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayShortName
    {
        get => field ?? GlobalDefaultGetDisplayShortName;
        set => field = value;
    }

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayDescription
    {
        get => field ?? GlobalDefaultGetDisplayDescription;
        set => field = value;
    }
    #endregion

    #region GlobalDefault


    /// <summary>
    /// 全局默认到字符串方法
    /// </summary>
    [DefaultValue("EnumValue")]
    public static Func<IEnumInfo, string> GlobalDefaultToString { get; } =
        static v => FormatFlags(v, static i => i.Value.ToString());

    /// <summary>
    /// 全局默认到字符串方法1
    /// </summary>
    [DefaultValue("EnumType_EnumValue")]
    public static Func<IEnumInfo, string> GlobalDefaultToString1 { get; } =
        static v => FormatFlags(v, static i => $"{i.EnumType.Name}_{i.Value}");

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayName { get; } =
        static v => FormatFlags(v, static i => i.Display?.GetName() ?? i.Value.ToString());

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayShortName { get; } =
        static v => FormatFlags(v, static i => i.Display?.GetShortName() ?? i.Value.ToString());

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayDescription { get; } =
        static v => FormatFlags(v, static i => i.Display?.GetDescription() ?? i.Value.ToString());

    private static string FormatFlags(IEnumInfo info, Func<IEnumInfo, string> formatter)
    {
        if (info.IsFlaggable is false || info.IsNone)
            return formatter(info);
        var native = info.Value.ToString();
        // 原生枚举无法完整分解为已命名标志时返回数值, 保留该值以免丢失未知位.
        if (char.IsAsciiDigit(native[0]) || native[0] == '-')
            return formatter(info);
        return string.Join(", ", info.GetFlagInfos().Select(formatter));
    }

    #endregion

    internal static Func<Enum, IEnumInfo> BuildEnumInfoExpression(Type enumType)
    {
        var enumParam = Expression.Parameter(typeof(Enum), "enumValue");
        var createMethod = typeof(EnumInfo<>)
            .MakeGenericType(enumType)
            .GetMethod(nameof(EnumInfo<StringComparison>.Create), new[] { typeof(Enum) });
        var createCall = Expression.Call(createMethod!, enumParam);
        var lambda = Expression.Lambda<Func<Enum, IEnumInfo>>(createCall, enumParam);
        return lambda.Compile();
    }
}
