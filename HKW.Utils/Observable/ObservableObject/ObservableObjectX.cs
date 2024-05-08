using System.ComponentModel;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察对象
/// <para>示例:<code><![CDATA[
/// public class ViewModelExample : ObservableObjectX
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
        INotifyPropertyChangedX,
        INotifyMemberPropertyChangedX
{
    /// <inheritdoc/>
    protected ObservableObjectX()
    {
        PropertyChanged -= ObservableObjectX_PropertyChanged;
        PropertyChanged += ObservableObjectX_PropertyChanged;
    }

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
        if (
            NotifyMemberProperties.TryGetValue(propertyName, out var oldMember)
            && newValue is INotifyPropertyChangedX newMember
        )
        {
            oldMember.PropertyChangedX -= Member_PropertyChangedX;

            NotifyMemberProperties[propertyName] = newMember;
            newMember.PropertyChangedX -= Member_PropertyChangedX;
            newMember.PropertyChangedX += Member_PropertyChangedX;
        }
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

    #region NotifyPropertyChanged
    /// <summary>
    /// 通知映射
    /// <para>(SourcePropertyName, TargetPropertyNames)</para>
    /// </summary>
    protected Dictionary<string, HashSet<string>> NotifyProperties { get; set; } = null!;

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyName">源属性名</param>
    /// <param name="targetPropertyName">目标属性名</param>
    protected void NotifyPropertyChanged(string sourcePropertyName, string targetPropertyName)
    {
        var map = NotifyProperties.GetOrCreateValue(sourcePropertyName);
        map.Add(targetPropertyName);
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyName">源属性名</param>
    /// <param name="targetPropertyNames">目标属性名</param>
    protected void NotifyPropertyChanged(
        string sourcePropertyName,
        IEnumerable<string> targetPropertyNames
    )
    {
        var map = NotifyProperties.GetOrCreateValue(sourcePropertyName);
        map.UnionWith(targetPropertyNames);
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyNames">源属性名</param>
    /// <param name="targetPropertyName">目标属性名</param>
    protected void NotifyPropertyChanged(
        IEnumerable<string> sourcePropertyNames,
        string targetPropertyName
    )
    {
        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            var map = NotifyProperties.GetOrCreateValue(sourcePropertyName);
            map.Add(targetPropertyName);
        }
    }

    /// <summary>
    /// 在源属性已改变后通知目标属性改变
    /// </summary>
    /// <param name="sourcePropertyNames">源属性名</param>
    /// <param name="targetPropertyNames">目标属性名</param>
    protected void NotifyPropertyChanged(
        IEnumerable<string> sourcePropertyNames,
        IEnumerable<string> targetPropertyNames
    )
    {
        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            var map = NotifyProperties.GetOrCreateValue(sourcePropertyName);
            map.UnionWith(targetPropertyNames);
        }
    }

    private void ObservableObjectX_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            return;
        if (NotifyProperties.TryGetValue(e.PropertyName, out var targetPropertyNames))
        {
            foreach (var name in targetPropertyNames)
                OnPropertyChanged(name);
        }
    }
    #endregion

    #region  MemberPropertyChanged
    /// <summary>
    /// 通知成员属性改变
    /// <para>(PropertyName, PropertyValue)</para>
    /// </summary>
    protected BidirectionalDictionary<
        string,
        INotifyPropertyChangedX
    > NotifyMemberProperties { get; set; } = null!;

    /// <summary>
    /// 通知成员属性改变
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <param name="member">成员</param>
    protected void NotifyMemberPropertyChanged(string memberName, INotifyPropertyChangedX member)
    {
        NotifyMemberProperties ??= new();
        NotifyMemberProperties.Add(memberName, member);
        member.PropertyChangedX -= Member_PropertyChangedX;
        member.PropertyChangedX += Member_PropertyChangedX;
    }

    /// <summary>
    /// 删除通知成员属性改变
    /// </summary>
    /// <param name="memberName">成员名称</param>
    protected bool RemoveNotifyMemberPropertyChanged(string memberName)
    {
        var result = NotifyMemberProperties.Remove(memberName, out var member);
        if (result)
            member!.PropertyChangedX -= Member_PropertyChangedX;
        return result;
    }

    /// <summary>
    /// 删除通知成员属性改变
    /// </summary>
    /// <param name="member">成员</param>
    protected bool RemoveNotifyMemberPropertyChanged(INotifyPropertyChangedX member)
    {
        var result = NotifyMemberProperties.Remove(member);
        if (result)
            member.PropertyChangedX -= Member_PropertyChangedX;
        return result;
    }

    private void Member_PropertyChangedX(object? sender, PropertyChangedXEventArgs e)
    {
        if (sender is not INotifyPropertyChangedX notify)
            return;
        MemberPropertyChangedX?.Invoke(
            sender,
            new(NotifyMemberProperties[notify], e.PropertyName, e.OldValue, e.NewValue)
        );
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

    /// <inheritdoc/>
    public event MemberPropertyChangedXEventHandler? MemberPropertyChangedX;
    #endregion
}
