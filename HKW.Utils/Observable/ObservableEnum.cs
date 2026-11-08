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
    /// <param name="addFlag">添加枚举值</param>
    /// <param name="removeFlag">删除枚举值</param>
    /// <remarks>
    /// <![CDATA[
    /// 添加枚举值: (value, flag) => value | flag
    /// 删除枚举值: (value, flag) => value & ~flag
    /// ]]>
    /// </remarks>
    public ObservableEnum(TEnum value, AddFlag<TEnum> addFlag, RemoveFlag<TEnum> removeFlag)
        : this(value)
    {
        AddFlagFunc = addFlag;
        RemoveFlagFunc = removeFlag;
    }

    private bool _isChanging;
    private TEnum _value;

    /// <summary>
    /// 枚举值
    /// </summary>
    public TEnum Value
    {
        get => _value;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            SetValueCore(value, mutation);
        }
    }

    private void SetValueCore(TEnum value, in ObservablePropertyMutation mutation)
    {
        if (EqualityComparer<TEnum>.Default.Equals(_value, value))
            return;
        var changing = mutation.Changing;
        var changed = mutation.Changed;
        if (changing is not null)
        {
            changing.Invoke(this, PropertyChangingArgsCache.Value);
            changing.Invoke(this, PropertyChangingArgsCache.Info);
        }
        _value = value;
        if (changed is not null)
        {
            changed.Invoke(this, PropertyChangedArgsCache.Value);
            changed.Invoke(this, PropertyChangedArgsCache.Info);
        }
    }

    Enum IObservableEnum.Value
    {
        get => Value;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            SetValueCore((TEnum)value, mutation);
        }
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
        using var mutation = new ObservablePropertyMutation(
            ref _isChanging,
            PropertyChanging,
            PropertyChanged
        );
        SetValueCore(AddFlagFunc(Value, flag), mutation);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    public void RemoveFlag(TEnum flag)
    {
        using var mutation = new ObservablePropertyMutation(
            ref _isChanging,
            PropertyChanging,
            PropertyChanged
        );
        SetValueCore(RemoveFlagFunc(Value, flag), mutation);
    }

    /// <summary>
    /// 浅复制当前值及实例位操作策略, 不复制事件订阅和命令实例.
    /// </summary>
    /// <remarks>
    /// 显式策略委托及其闭包按引用共享. 未指定实例策略时, 克隆仍跟随类型默认策略.
    /// </remarks>
    public object Clone()
    {
        return new ObservableEnum<TEnum>(Value)
        {
            _addFlagFunc = _addFlagFunc,
            _removeFlagFunc = _removeFlagFunc,
        };
    }

    /// <summary>
    /// 添加标志命令
    /// </summary>
    public SimpleCommand<TEnum> AddFlagCommand => field ??= new(p => AddFlag(p));

    /// <summary>
    /// 删除标志命令
    /// </summary>
    public SimpleCommand<TEnum> RemoveFlagCommand => field ??= new(p => RemoveFlag(p));

    private AddFlag<TEnum>? _addFlagFunc;
    private RemoveFlag<TEnum>? _removeFlagFunc;

    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlag<TEnum> AddFlagFunc
    {
        get => _addFlagFunc ?? DefaultAddFlagFunc;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            _addFlagFunc = value;
        }
    }

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlag<TEnum> RemoveFlagFunc
    {
        get => _removeFlagFunc ?? DefaultRemoveFlagFunc;
        set
        {
            using var mutation = new ObservablePropertyMutation(
                ref _isChanging,
                PropertyChanging,
                PropertyChanged
            );
            _removeFlagFunc = value;
        }
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
/// <remarks>
/// 并发首次访问每个枚举类型只编译一次工厂. 不保证外部枚举元数据缓存和静态策略写入线程安全.
/// 使用反射及动态代码编译, 不承诺 NativeAOT 或裁剪兼容性.
/// </remarks>
public static class ObservableEnum
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<
        Type,
        Lazy<Func<Enum, IObservableEnum>>
    > _expressionByType = new();

    /// <summary>
    /// 根据枚举值创建可观测枚举
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>可观测枚举</returns>
    public static IObservableEnum Create(Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var factory = _expressionByType.GetOrAdd(
            value.GetType(),
            static type => new Lazy<Func<Enum, IObservableEnum>>(() =>
                CreateObservableEnumExpression(type)
            )
        );
        return factory.Value(value);
    }

    /// <summary>
    /// 构建 Lambda 表达式, 在无枚举准确类型的情况下创建可观测枚举
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
