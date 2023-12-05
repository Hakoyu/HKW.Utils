using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察对象
/// <para>示例:<code><![CDATA[
/// public class ObservableClassExample : ObservableClass<ObservableClassExample>
/// {
///     int _value = 0;
///     public int Value
///     {
///         get => _value;
///         set => SetProperty(nameof(Value), ref _value, value);
///     }
/// }]]></code></para>
/// </summary>
public abstract class ObservableClass<T>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        INotifyPropertyChangingX<T>,
        INotifyPropertyChangedX<T>
    where T : ObservableClass<T>
{
    #region NotifySender
    /// <summary>
    /// 通知属性更改 (PropertyName, Responder)
    /// </summary>
    protected readonly Dictionary<
        string,
        PropertyChangedResponder<T>
    > _allPropertyChangedResponder = new();

    /// <summary>
    /// 获取通知响应器
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>接收器</returns>
    public PropertyChangedResponder<T> GetPropertyChangedResponder(string propertyName)
    {
        if (_allPropertyChangedResponder.TryGetValue(propertyName, out var value) is false)
            value = new(propertyName, (T)this);
        return value;
    }

    /// <summary>
    /// 删除通知响应器
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemovePropertyChangedResponder(string propertyName)
    {
        if (_allPropertyChangedResponder.TryGetValue(propertyName, out var value) is false)
            return false;
        value.Close();
        return _allPropertyChangedResponder.Remove(propertyName);
    }
    #endregion

    #region OnPropertyChange
    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="value">值</param>
    /// <param name="newValue">新值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    protected virtual bool SetProperty<TValue>(string name, ref TValue value, TValue newValue)
    {
        if (value?.Equals(newValue) is true)
            return false;
        var oldValue = value;
        if (OnPropertyChanging(name, oldValue, newValue))
            return false;
        value = newValue;
        OnPropertyChanged(name, oldValue, newValue);
        return true;
    }

    /// <summary>
    /// 属性改变前
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    /// <returns>取消为 <see langword="true"/> 否则为 <see langword="false"/></returns>
    protected virtual bool OnPropertyChanging(string name, object? oldValue, object? newValue)
    {
        PropertyChanging?.Invoke(this, new(name));
        if (PropertyChangingX is null)
            return false;
        var e = new PropertyChangingXEventArgs(name, oldValue, newValue);
        PropertyChangingX?.Invoke((T)this, e);
        if (e.Cancel)
            PropertyChanged?.Invoke(this, new(name));
        return e.Cancel;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    protected virtual void OnPropertyChanged(string name, object? oldValue, object? newValue)
    {
        PropertyChanged?.Invoke(this, new(name));
        PropertyChangedX?.Invoke((T)this, new(name, oldValue, newValue));
    }
    #endregion

    #region Event
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event PropertyChangingXEventHandler<T>? PropertyChangingX;

    /// <inheritdoc/>
    public event PropertyChangedXEventHandler<T>? PropertyChangedX;
    #endregion
}
