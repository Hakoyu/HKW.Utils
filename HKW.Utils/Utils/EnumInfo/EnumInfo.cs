using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
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

    #region DefaultToString

    private static Func<IEnumInfo, string>? _defaultToString;

    /// <summary>
    /// 默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultToString
    {
        get => _defaultToString ?? GlobalDefaultToString;
        set => _defaultToString = value;
    }
    #endregion

    #region DefaultGetName
    private static Func<IEnumInfo, string>? _defaultGetDisplayName;

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayName
    {
        get => _defaultGetDisplayName ?? GlobalDefaultGetDisplayName;
        set => _defaultGetDisplayName = value;
    }
    #endregion

    #region DefaultGetShortName
    private static Func<IEnumInfo, string>? _defaultGetDisplayShortName;

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayShortName
    {
        get => _defaultGetDisplayShortName ?? GlobalDefaultGetDisplayShortName;
        set => _defaultGetDisplayShortName = value;
    }
    #endregion

    #region DefaultGetDescription
    private static Func<IEnumInfo, string>? _defaultGetDisplayDescription;

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayDescription
    {
        get => _defaultGetDisplayDescription ?? GlobalDefaultGetDisplayDescription;
        set => _defaultGetDisplayDescription = value;
    }
    #endregion

    #endregion

    #region GlobalDefault


    /// <summary>
    /// 全局默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultToString { get; } =
        static v => v.Value.ToString();

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayName { get; } =
        static v =>
        {
            if (v.IsFlaggable is false || v.IsNone)
                return v.Display?.Name ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.Name ?? i.Value.ToString())
            );
        };

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayShortName { get; } =
        static v =>
        {
            if (v.IsFlaggable is false || v.IsNone)
                return v.Display?.ShortName ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.ShortName ?? i.Value.ToString())
            );
        };

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayDescription { get; } =
        static v =>
        {
            if (v.IsFlaggable is false || v.IsNone)
                return v.Display?.Description ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.Description ?? i.Value.ToString())
            );
        };

    #endregion

    /// <summary>
    /// 构建Lambda表达式, 在无枚举准确类型的情况下创建枚举信息
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <returns>构造方法</returns>
    internal static Func<Enum, IEnumInfo> CreateEnumInfoExpression(Type enumType)
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
