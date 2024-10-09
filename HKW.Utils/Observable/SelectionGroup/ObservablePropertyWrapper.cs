using System.ComponentModel;
using HKW.HKWReactiveUI;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测属性包装器
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public partial class ObservablePropertyWrapper<TSource, TValue>
    : ReactiveObjectX,
        ICloneable<ObservablePropertyWrapper<TSource, TValue>>,
        IDisposable
    where TSource : INotifyPropertyChanged
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="propertyName">属性名</param>
    /// <param name="getAction">获取行动</param>
    /// <param name="setAction">设置行动</param>
    public ObservablePropertyWrapper(
        TSource source,
        string propertyName,
        Func<TSource, TValue> getAction,
        Action<TSource, TValue> setAction
    )
    {
        Source = source;
        PropertyName = propertyName;
        GetAction = getAction;
        SetAction = setAction;
        if (Source is null)
            return;
        Source.PropertyChanged -= Source_PropertyChanged;
        Source.PropertyChanged += Source_PropertyChanged;
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PropertyName)
            this.RaisePropertyChanged(nameof(Value));
    }

    /// <summary>
    /// 源
    /// </summary>
    public TSource Source { get; } = default!;

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue Value
    {
        get => GetAction.Invoke(Source);
        set => SetAction.Invoke(Source, value);
    }

    /// <summary>
    /// 获取行动
    /// </summary>
    public Func<TSource, TValue> GetAction { get; set; }

    /// <summary>
    /// 设置行动
    /// </summary>
    public Action<TSource, TValue> SetAction { get; set; }

    #region ICloneable
    /// <inheritdoc/>
    public ObservablePropertyWrapper<TSource, TValue> Clone()
    {
        return Clone(Source);
    }

    /// <summary>
    /// 克隆一个新对象, 但重新设置源
    /// </summary>
    /// <param name="source">新源</param>
    /// <returns>克隆的对象</returns>
    public ObservablePropertyWrapper<TSource, TValue> Clone(TSource source)
    {
        return new(source, PropertyName, GetAction, SetAction);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
    #endregion

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservablePropertyWrapper()
    {
        //必须为false
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        //必须为true
        Dispose(true);
        //通知垃圾回收器不再调用终结器
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (Source is not null)
                Source.PropertyChanged -= Source_PropertyChanged;
            GetAction = null!;
            SetAction = null!;

            _disposed = true;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Close()
    {
        Dispose();
    }
    #endregion
}
