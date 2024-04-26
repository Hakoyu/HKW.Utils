using System.ComponentModel;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察对象
/// <para>示例:<code><![CDATA[
/// public class ViewModelExample : ViewModelBase<ViewModelExample>
/// {
///     int _value = 0;
///     public int Value
///     {
///         get => _value;
///         set => SetProperty(ref _value, value);
///     }
/// }]]></code></para>
/// </summary>
public abstract class ObservableObjectX
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        INotifyPropertyChangingX,
        INotifyPropertyChangedX
{
    #region OnPropertyChange
    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="newValue">新值</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual bool SetProperty<TValue>(
        ref TValue value,
        TValue newValue,
        [CallerMemberName] string propertyName = null!
    )
    {
        if (value?.Equals(newValue) is true)
            return false;
        var oldValue = value;
        if (OnPropertyChanging(oldValue, newValue, propertyName) is false)
            return false;
        value = newValue;
        OnPropertyChanged(oldValue, newValue, propertyName);
        return true;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="newValue">新值</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual bool SetProperty(
        object? value,
        object? newValue,
        [CallerMemberName] string propertyName = null!
    )
    {
        if (value?.Equals(newValue) is true)
            return false;
        var oldValue = value;
        if (OnPropertyChanging(oldValue, newValue, propertyName) is false)
            return false;
        value = newValue;
        OnPropertyChanged(oldValue, newValue, propertyName);
        return true;
    }

    /// <summary>
    /// 属性改变前
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>取消为 <see langword="false"/> 否则为 <see langword="true"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual bool OnPropertyChanging(
        object? oldValue,
        object? newValue,
        [CallerMemberName] string propertyName = null!
    )
    {
        PropertyChanging?.Invoke(this, new(propertyName));
        if (PropertyChangingX is null)
            return true;
        var e = new PropertyChangingXEventArgs(propertyName, oldValue, newValue);
        PropertyChangingX.Invoke(this, e);
        // 如果取消, 则通知View恢复原值
        if (e.Cancel)
            PropertyChanged?.Invoke(this, new(propertyName));
        return e.Cancel is false;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    /// <param name="propertyName">属性名称</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnPropertyChanged(
        object? oldValue,
        object? newValue,
        [CallerMemberName] string propertyName = null!
    )
    {
        PropertyChanged?.Invoke(this, new(propertyName));
        PropertyChangedX?.Invoke(this, new(propertyName, oldValue, newValue));
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
        PropertyChangedX?.Invoke(this, new(propertyName, null, null));
    }
    #endregion

    #region NotifyPropertyOnPropertyChanged
    /// <summary>
    /// 通知映射
    /// <para>(TourcePropertyName, TargetPropertyNames)</para>
    /// </summary>
    protected Dictionary<string, HashSet<string>> NotifyMap { get; set; } = new();

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyName">源属性名</param>
    /// <param name="targetPropertyName">目标属性名</param>
    public void NotifyPropertyOnPropertyChanged(
        string sourcePropertyName,
        string targetPropertyName
    )
    {
        PropertyChanged -= ObservableObjectX_PropertyChanged;
        PropertyChanged += ObservableObjectX_PropertyChanged;
        var map = NotifyMap.GetOrCreateValue(sourcePropertyName);
        map.Add(targetPropertyName);
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyName">源属性名</param>
    /// <param name="targetPropertyNames">目标属性名</param>
    public void NotifyPropertyOnPropertyChanged(
        string sourcePropertyName,
        IEnumerable<string> targetPropertyNames
    )
    {
        PropertyChanged -= ObservableObjectX_PropertyChanged;
        PropertyChanged += ObservableObjectX_PropertyChanged;
        var map = NotifyMap.GetOrCreateValue(sourcePropertyName);
        map.UnionWith(targetPropertyNames);
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyNames">源属性名</param>
    /// <param name="targetPropertyName">目标属性名</param>
    public void NotifyPropertyOnPropertyChanged(
        IEnumerable<string> sourcePropertyNames,
        string targetPropertyName
    )
    {
        PropertyChanged -= ObservableObjectX_PropertyChanged;
        PropertyChanged += ObservableObjectX_PropertyChanged;
        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            var map = NotifyMap.GetOrCreateValue(sourcePropertyName);
            map.Add(targetPropertyName);
        }
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyNames">源属性名</param>
    /// <param name="targetPropertyNames">目标属性名</param>
    public void NotifyPropertyOnPropertyChanged(
        IEnumerable<string> sourcePropertyNames,
        IEnumerable<string> targetPropertyNames
    )
    {
        PropertyChanged -= ObservableObjectX_PropertyChanged;
        PropertyChanged += ObservableObjectX_PropertyChanged;
        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            var map = NotifyMap.GetOrCreateValue(sourcePropertyName);
            map.UnionWith(targetPropertyNames);
        }
    }

    private void ObservableObjectX_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            return;
        if (NotifyMap.TryGetValue(e.PropertyName, out var targetPropertyNames))
        {
            foreach (var name in targetPropertyNames)
                OnPropertyChanged(name);
        }
    }
    #endregion
    #region Event
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event PropertyChangingXEventHandler? PropertyChangingX;

    /// <inheritdoc/>
    public event PropertyChangedXEventHandler? PropertyChangedX;
    #endregion
    /// <summary>
    /// 值缓存
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    protected class ValueBuffer(object? oldValue, object? newValue)
    {
        /// <summary>
        /// 旧值
        /// </summary>
        public WeakReference OldValue { get; } = new(oldValue);

        /// <summary>
        /// 新值
        /// </summary>
        public WeakReference NewValue { get; } = new(newValue);
    }
}
