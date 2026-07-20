using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测值
/// </summary>
/// <typeparam name="TObject">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
[DebuggerDisplay("Source = {Source}, Value = {Value}")]
public sealed partial class ObservableWrapper<TObject, TProperty> : DisposableReactiveObject
    where TObject : INotifyPropertyChanged
{
    #region Ctor

    /// <inheritdoc/>
    /// <param name="source">视图模型</param>
    /// <param name="expression">获取器表达式</param>
    /// <param name="setter">设置器</param>
    public ObservableWrapper(
        TObject source,
        Expression<Func<TObject, TProperty>> expression,
        Action<TObject, TProperty> setter
    )
        : this(source, GetName(expression), expression.Compile(), setter) { }

    /// <inheritdoc/>
    /// <param name="source">视图模型</param>
    /// <param name="propertyName">属性名</param>
    /// <param name="getter">获取器</param>
    /// <param name="setter">设置器</param>
    public ObservableWrapper(
        TObject source,
        string propertyName,
        Func<TObject, TProperty> getter,
        Action<TObject, TProperty> setter
    )
    {
        if (source is INotifyPropertyChanging changing)
            changing.PropertyChanging += ViewModel_PropertyChanging;
        source.PropertyChanged += ViewModel_PropertyChanged;

        Source = source;
        PropertyName = propertyName;
        _getter = getter;
        _setter = setter;
    }

    private void ViewModel_PropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == PropertyName)
            this.RaisePropertyChanging(nameof(Value));
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PropertyName)
            this.RaisePropertyChanged(nameof(Value));
    }

    #endregion
    /// <summary>
    /// 源
    /// </summary>
    public TObject Source { get; private set; }

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; }

    private readonly Func<TObject, TProperty> _getter;
    private readonly Action<TObject, TProperty> _setter;

    private TProperty _value = default!;

    /// <summary>
    /// 值
    /// </summary>
    public TProperty Value
    {
        get => _getter(Source);
        set => RaiseAndSetValue(ref _value, value);
    }

    private void RaiseAndSetValue(ref TProperty backingField, TProperty newValue, bool check = true)
    {
        if (!check || !EqualityComparer<TProperty>.Default.Equals(backingField, newValue))
        {
            this.RaisePropertyChanging(nameof(Value));
            backingField = newValue;
            _setter(Source, newValue);
            this.RaisePropertyChanged(nameof(Value));
        }
    }

    private static string GetName(Expression<Func<TObject, TProperty>> expression)
    {
        if (expression.Body is MemberExpression member)
        {
            return member.Member.Name;
        }
        else if (
            expression.Body is UnaryExpression unary
            && unary.Operand is MemberExpression unaryMember
        )
        {
            return unaryMember.Member.Name;
        }
        else
            throw new ArgumentException("Expression error", nameof(expression));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Source is INotifyPropertyChanging changing)
                changing.PropertyChanging -= ViewModel_PropertyChanging;
            Source.PropertyChanged -= ViewModel_PropertyChanged;
            Source = default!;
        }
    }
}
