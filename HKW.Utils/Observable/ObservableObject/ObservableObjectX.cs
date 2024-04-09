using System.ComponentModel;
using System.Runtime.CompilerServices;

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
}
