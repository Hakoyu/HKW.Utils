using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData.Binding;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils;

/// <summary>
/// I18n对象信息
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public sealed class I18nObject<TKey, TValue> : IEquatable<I18nObject<TKey, TValue>>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    public I18nObject(IReactiveObject source)
        : this(source, source.RaisePropertyChanged) { }

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="propertyChanged">属性改变行动</param>
    public I18nObject(IReactiveObject source, PropertyChanged propertyChanged)
    {
        Source = source;
        Source.PropertyChanging -= Source_PropertyChanging;
        Source.PropertyChanged -= Source_PropertyChanged;

        Source.PropertyChanging += Source_PropertyChanging;
        Source.PropertyChanged += Source_PropertyChanged;

        OnPropertyChanged = propertyChanged;
    }

    /// <summary>
    /// 源
    /// </summary>
    public IReactiveObject Source { get; }

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public PropertyChanged OnPropertyChanged { get; }

    /// <summary>
    /// 基于键属性名称的目标属性名称
    /// <para>
    /// (KeyPropertyName, TargetPropertyNames)
    /// </para>
    /// </summary>
    public Dictionary<string, HashSet<string>> KeyNameToTargetNames { get; } = new();

    /// <summary>
    /// 基于键值的目标属性名称
    /// <para>
    /// (KeyValue, TargetPropertyNames)
    /// </para>
    /// </summary>
    public Dictionary<TKey, HashSet<string>> KeyToTargetNames { get; } = new();

    /// <summary>
    /// 在键改变时保留值的属性名
    /// </summary>
    public HashSet<string> RetentionValueOnKeyChangePropertyNames { get; } = new();

    /// <summary>
    /// 获取键的方法
    /// <para>
    /// (KeyPropertyName, GetKey)
    /// </para>
    /// </summary>
    public Dictionary<string, Func<IReactiveObject, TKey>> KeyNameToGetKey { get; } = new();

    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="keyPropertyName">键属性名</param>
    /// <param name="getKey">获取键</param>
    /// <param name="targetPropertyName">目标属性名</param>
    /// <param name="retentionValueOnKeyChange">键更改时保留值</param>
    public void AddProperty(
        string keyPropertyName,
        Func<IReactiveObject, TKey> getKey,
        string targetPropertyName,
        bool retentionValueOnKeyChange = false
    )
    {
        var result = KeyNameToTargetNames.GetOrCreate(keyPropertyName).Add(targetPropertyName);
        if (result is false)
            return;

        if (retentionValueOnKeyChange)
            RetentionValueOnKeyChangePropertyNames.Add(keyPropertyName);

        KeyNameToGetKey[keyPropertyName] = getKey;

        KeyToTargetNames.GetOrCreate(getKey(Source)).Add(targetPropertyName);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private TKey _oldKeyValue = default!;

    private void Source_PropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;
        if (KeyNameToGetKey.TryGetValue(e.PropertyName, out var getKey))
            _oldKeyValue = getKey(Source);
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName) || _oldKeyValue is null)
            return;
        if (
            KeyNameToTargetNames.TryGetValue(
                e.PropertyName,
                out var targetPropertyNamesWithKeyPropertyName
            )
        )
        {
            var newValue = KeyNameToGetKey[e.PropertyName](Source);
            // 如果触发事件的是已记录的键属性名
            if (
                KeyToTargetNames.TryGetValueOrCreate(
                    newValue,
                    out var newTargetPropertyNamesWithKey,
                    () => targetPropertyNamesWithKeyPropertyName.ToHashSet()
                )
            )
            {
                // 将ID对应的属性名添加至新建对应的属性名中
                newTargetPropertyNamesWithKey.UnionWith(targetPropertyNamesWithKeyPropertyName);
            }
            if (KeyToTargetNames.TryGetValue(_oldKeyValue, out var oldTargetPropertyNamesWithKey))
            {
                // 从旧键中去除ID对应的属性名
                oldTargetPropertyNamesWithKey.ExceptWith(targetPropertyNamesWithKeyPropertyName);
                // 如果旧键不存在值则删除
                if (oldTargetPropertyNamesWithKey.HasValue() is false)
                    KeyToTargetNames.Remove(_oldKeyValue);
            }
            if (RetentionValueOnKeyChangePropertyNames.Contains(e.PropertyName))
                KeyChanged?.Invoke(this, (_oldKeyValue, newValue));
        }
        _oldKeyValue = default!;
    }

    /// <summary>
    /// 通知键对应的属性
    /// </summary>
    /// <param name="key">键</param>
    public void NotifyPropertyChangedByKey(TKey key)
    {
        if (KeyToTargetNames.TryGetValue(key, out var targetPropertyNames) is false)
            return;
        foreach (var name in targetPropertyNames)
            OnPropertyChanged(name);
    }

    /// <summary>
    /// 通知所有属性改变
    /// </summary>
    public void NotifyAllPropertyChanged()
    {
        foreach (var pair in KeyToTargetNames)
        {
            foreach (var name in pair.Value)
                OnPropertyChanged(name);
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        Source.PropertyChanging -= Source_PropertyChanging;
        Source.PropertyChanged -= Source_PropertyChanged;
    }

    #region IEquatable
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as I18nObject<TKey, TValue>);
    }

    /// <inheritdoc/>
    public bool Equals(I18nObject<TKey, TValue>? other)
    {
        return Source.Equals(other?.Source);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Source.GetHashCode();
    }
    #endregion
    /// <summary>
    /// 键改变
    /// </summary>
    public event KeyChangedEventHandler<TKey, TValue>? KeyChanged;

    /// <summary>
    /// 属性改变
    /// </summary>
    /// <param name="propertyName">属性名</param>
    public delegate void PropertyChanged(string propertyName);
}
