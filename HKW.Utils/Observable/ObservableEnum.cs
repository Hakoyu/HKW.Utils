using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Input;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测枚举接口
/// </summary>
public interface IObservableEnum
{
    /// <summary>
    /// 枚举值
    /// </summary>
    public Enum Value { get; set; }

    /// <summary>
    /// 枚举信息
    /// </summary>
    public IEnumInfo Info { get; }
}

/// <summary>
/// 可观测枚举
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class ObservableEnum<TEnum>
    : IObservableEnum,
        INotifyPropertyChanging,
        INotifyPropertyChanged,
        ICloneable
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    public ObservableEnum(TEnum value = default)
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(Info);
        Value = value;
    }

    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    /// <param name="addFlag">添加枚举值 <c>(value, flag) => value | flag</c></param>
    /// <param name="removeFlag">删除枚举值 <c>(value, flag) => value &amp; ~flag</c></param>
    public ObservableEnum(TEnum value, AddFlag<TEnum> addFlag, RemoveFlag<TEnum> removeFlag)
        : this(value)
    {
        AddFlagFunc = addFlag;
        RemoveFlagFunc = removeFlag;
    }

    /// <summary>
    /// 枚举值
    /// </summary>
    public TEnum Value
    {
        get => field;
        set
        {
            if (field == value)
                return;
            PropertyChanging?.Invoke(this, PropertyChangingArgsCache.Value);
            PropertyChanging?.Invoke(this, PropertyChangingArgsCache.Info);
            field = value;
            PropertyChanged?.Invoke(this, PropertyChangedArgsCache.Value);
            PropertyChanged?.Invoke(this, PropertyChangedArgsCache.Info);
        }
    }

    Enum IObservableEnum.Value
    {
        get => Value;
        set => Value = (TEnum)value;
    }

    /// <summary>
    /// 枚举信息
    /// </summary>
    public EnumInfo<TEnum> Info => Value.GetInfo();
    IEnumInfo IObservableEnum.Info => Info;

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    public void AddFlag(TEnum flag)
    {
        Value = AddFlagFunc(Value, flag);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    public void RemoveFlag(TEnum flag)
    {
        Value = RemoveFlagFunc(Value, flag);
    }

    /// <inheritdoc/>
    public object Clone()
    {
        return new ObservableEnum<TEnum>(Value);
    }

    /// <summary>
    /// 添加标志命令
    /// </summary>
    public SimpleCommand<TEnum> AddFlagCommand => field ??= new(p => AddFlag(p));

    /// <summary>
    /// 删除标志命令
    /// </summary>
    public SimpleCommand<TEnum> RemoveFlagCommand => field ??= new(p => RemoveFlag(p));

    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlag<TEnum> AddFlagFunc
    {
        get => field ?? DefaultAddFlagFunc;
        set => field = value;
    }

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlag<TEnum> RemoveFlagFunc
    {
        get => field ?? DefaultRemoveFlagFunc;
        set => field = value;
    }

    /// <summary>
    /// 默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> DefaultAddFlagFunc
    {
        get => field ?? GlobalDefaultAddFlagFunc;
        set => field = value;
    }

    /// <summary>
    /// 默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> DefaultRemoveFlagFunc
    {
        get => field ?? GlobalDefaultRemoveFlagFunc;
        set => field = value;
    }

    /// <summary>
    /// 全局默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> GlobalDefaultAddFlagFunc { get; } = (v, f) => v.AddFlag(f);

    /// <summary>
    /// 全局默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> GlobalDefaultRemoveFlagFunc { get; } =
        (v, f) => v.RemoveFlag(f);

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

/// <summary>
/// 添加标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>添加标志的值</returns>
public delegate TEnum AddFlag<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;

/// <summary>
/// 删除标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>删除标志的值</returns>
public delegate TEnum RemoveFlag<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;

/// <summary>
/// 可观测枚举工厂
/// </summary>
public static class ObservableEnum
{
    private static readonly Dictionary<Type, Func<Enum, IObservableEnum>> _expressionByType = [];

    /// <summary>
    /// 根据枚举值创建可观测枚举
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>可观测枚举</returns>
    public static IObservableEnum Create(Enum value)
    {
        var type = value.GetType();
        if (_expressionByType.TryGetValue(type, out var func) is false)
            func = _expressionByType[type] = CreateObservableEnumExpression(type);

        return func.Invoke(value);
    }

    /// <summary>
    /// 构建 Lambda 表达式，在无枚举准确类型的情况下创建可观测枚举
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <returns>构造方法</returns>
    internal static Func<Enum, IObservableEnum> CreateObservableEnumExpression(Type enumType)
    {
        var enumParam = Expression.Parameter(typeof(Enum), "enumValue");
        var observableEnumType = typeof(ObservableEnum<>).MakeGenericType(enumType);
        var constructor = observableEnumType.GetConstructor([enumType])!;
        var createCall = Expression.New(constructor, Expression.Convert(enumParam, enumType));
        var lambda = Expression.Lambda<Func<Enum, IObservableEnum>>(createCall, enumParam);
        return lambda.Compile();
    }
}
