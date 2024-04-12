using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using HKW.HKWUtils.Utils;

namespace HKW.HKWUtils;

/// <summary>
/// I18n对象信息
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
public sealed class I18nObjectInfo<TKey>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    private I18nObjectInfo(INotifyPropertyChangedX source, Action<string> onPropertyChanged)
    {
        Source = source;
        Source.PropertyChangedX -= Source_PropertyChangedX;
        Source.PropertyChangedX += Source_PropertyChangedX;
        OnPropertyChanged = onPropertyChanged;
    }

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    /// <param name="propertyDatas">属性数据 (键属性名, 键值, 目标属性名)</param>
    public I18nObjectInfo(
        INotifyPropertyChangedX source,
        Action<string> onPropertyChanged,
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            string TargetPropertyName
        )> propertyDatas
    )
        : this(source, onPropertyChanged)
    {
        foreach (var data in propertyDatas)
        {
            if (
                TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                    data.KeyPropertyName,
                    out var targetPropertyNamesWithKeyPropertyName
                )
                is false
            )
            {
                targetPropertyNamesWithKeyPropertyName = TargetPropertyNamesWithKeyPropertyName[
                    data.KeyPropertyName
                ] = new();
            }
            targetPropertyNamesWithKeyPropertyName.Add(data.TargetPropertyName);

            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    data.KeyValue,
                    out var targetPropertyNamesWithKey
                )
                is false
            )
            {
                targetPropertyNamesWithKey = TargetPropertyNamesWithKey[data.KeyValue] = new();
            }
            targetPropertyNamesWithKey.Add(data.TargetPropertyName);
        }
    }

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    /// <param name="propertyDatas">属性数据 (键属性名, 键值, 目标属性名)</param>
    public I18nObjectInfo(
        INotifyPropertyChangedX source,
        Action<string> onPropertyChanged,
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            string TargetPropertyName,
            bool RetentionValueOnKeyChange
        )> propertyDatas
    )
        : this(source, onPropertyChanged)
    {
        foreach (var data in propertyDatas)
        {
            if (data.RetentionValueOnKeyChange)
                RetentionValueOnKeyChangePropertyNames.Add(data.KeyPropertyName);
            if (
                TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                    data.KeyPropertyName,
                    out var targetPropertyNamesWithKeyPropertyName
                )
                is false
            )
            {
                targetPropertyNamesWithKeyPropertyName = TargetPropertyNamesWithKeyPropertyName[
                    data.KeyPropertyName
                ] = new();
            }
            targetPropertyNamesWithKeyPropertyName.Add(data.TargetPropertyName);

            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    data.KeyValue,
                    out var targetPropertyNamesWithKey
                )
                is false
            )
            {
                targetPropertyNamesWithKey = TargetPropertyNamesWithKey[data.KeyValue] = new();
            }
            targetPropertyNamesWithKey.Add(data.TargetPropertyName);
        }
    }

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    /// <param name="propertyDatas">属性数据 (键属性名, 键值, 目标属性名)</param>
    public I18nObjectInfo(
        INotifyPropertyChangedX source,
        Action<string> onPropertyChanged,
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            IEnumerable<string> TargetPropertyNames
        )> propertyDatas
    )
        : this(source, onPropertyChanged)
    {
        foreach (var data in propertyDatas)
        {
            if (
                TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                    data.KeyPropertyName,
                    out var targetPropertyNamesWithKeyPropertyName
                )
                is false
            )
            {
                targetPropertyNamesWithKeyPropertyName = TargetPropertyNamesWithKeyPropertyName[
                    data.KeyPropertyName
                ] = new();
            }
            targetPropertyNamesWithKeyPropertyName.UnionWith(data.TargetPropertyNames);

            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    data.KeyValue,
                    out var targetPropertyNamesWithKey
                )
                is false
            )
            {
                targetPropertyNamesWithKey = TargetPropertyNamesWithKey[data.KeyValue] = new();
            }
            targetPropertyNamesWithKey.UnionWith(data.TargetPropertyNames);
        }
    }

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    /// <param name="propertyDatas">属性数据 (键属性名, 键值, 目标属性名, 在键改变时保留值)</param>
    public I18nObjectInfo(
        INotifyPropertyChangedX source,
        Action<string> onPropertyChanged,
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            IEnumerable<string> TargetPropertyNames,
            bool RetentionValueOnKeyChange
        )> propertyDatas
    )
        : this(source, onPropertyChanged)
    {
        foreach (var data in propertyDatas)
        {
            if (data.RetentionValueOnKeyChange)
                RetentionValueOnKeyChangePropertyNames.Add(data.KeyPropertyName);
            if (
                TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                    data.KeyPropertyName,
                    out var targetPropertyNamesWithKeyPropertyName
                )
                is false
            )
            {
                targetPropertyNamesWithKeyPropertyName = TargetPropertyNamesWithKeyPropertyName[
                    data.KeyPropertyName
                ] = new();
            }
            targetPropertyNamesWithKeyPropertyName.UnionWith(data.TargetPropertyNames);

            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    data.KeyValue,
                    out var targetPropertyNamesWithKey
                )
                is false
            )
            {
                targetPropertyNamesWithKey = TargetPropertyNamesWithKey[data.KeyValue] = new();
            }
            targetPropertyNamesWithKey.UnionWith(data.TargetPropertyNames);
        }
    }

    /// <summary>
    /// 源
    /// </summary>
    public INotifyPropertyChangedX Source { get; }

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public Action<string> OnPropertyChanged { get; }

    /// <summary>
    /// 基于键属性名称的目标属性名称
    /// <para>
    /// (KeyPropertyName, TargetPropertyNames)
    /// </para>
    /// </summary>
    public Dictionary<string, HashSet<string>> TargetPropertyNamesWithKeyPropertyName { get; } =
        new();

    /// <summary>
    /// 基于键值的目标属性名称
    /// <para>
    /// (KeyValue, TargetPropertyNames)
    /// </para>
    /// </summary>
    public Dictionary<TKey, HashSet<string>> TargetPropertyNamesWithKey { get; } = new();

    /// <summary>
    /// 在键改变时保留值的属性名
    /// </summary>
    public HashSet<string> RetentionValueOnKeyChangePropertyNames { get; } = new();

    private void Source_PropertyChangedX(object? sender, PropertyChangedXEventArgs e)
    {
        if (
            TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                e.PropertyName,
                out var targetPropertyNamesWithKeyPropertyName
            )
        )
        {
            // 如果触发事件的是已记录的键属性名
            // 则向属性名对应的I18n资源属性发送修改通知
            NotifyPropertyChangedWithKeyPropertyName(e.PropertyName);
            (var oldValue, var newValue) = e.GetValue<TKey>();
            var oldTargetPropertyNamesWithKey = TargetPropertyNamesWithKey[oldValue];
            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    newValue,
                    out var newTargetPropertyNamesWithKey
                )
            )
            {
                // 将ID对应的属性名添加至新建对应的属性名中
                newTargetPropertyNamesWithKey.UnionWith(targetPropertyNamesWithKeyPropertyName);
            }
            else
            {
                TargetPropertyNamesWithKey[newValue] =
                    targetPropertyNamesWithKeyPropertyName.ToHashSet();
            }
            // 从旧键中去除ID对应的属性名
            oldTargetPropertyNamesWithKey.ExceptWith(targetPropertyNamesWithKeyPropertyName);
            // 如果旧键不存在值则删除
            if (oldTargetPropertyNamesWithKey.HasValue() is false)
                TargetPropertyNamesWithKey.Remove(oldValue);
            if (RetentionValueOnKeyChangePropertyNames.Contains(e.PropertyName))
                KeyChanged?.Invoke(this, (oldValue, newValue));
        }
    }

    /// <summary>
    /// 通知键属性名对应的属性
    /// </summary>
    /// <param name="propertyName">属性名</param>
    public void NotifyPropertyChangedWithKeyPropertyName(string propertyName)
    {
        if (
            TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                propertyName,
                out var targetPropertyNames
            )
            is false
        )
            return;
        foreach (var name in targetPropertyNames)
            OnPropertyChanged(name);
    }

    /// <summary>
    /// 通知键对应的属性
    /// </summary>
    /// <param name="key">键</param>
    public void NotifyPropertyChangedWithKey(TKey key)
    {
        if (TargetPropertyNamesWithKey.TryGetValue(key, out var targetPropertyNames) is false)
            return;
        foreach (var name in targetPropertyNames)
            OnPropertyChanged(name);
    }

    /// <summary>
    /// 通知所有属性改变
    /// </summary>
    public void NotifyAllPropertyChanged()
    {
        foreach (var propertyNames in TargetPropertyNamesWithKeyPropertyName.Values)
        {
            foreach (var name in propertyNames)
                OnPropertyChanged(name);
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        Source.PropertyChangedX -= Source_PropertyChangedX;
    }

    /// <summary>
    /// 键改变
    /// </summary>
    public event KeyChangedEventHandler<TKey>? KeyChanged;
}
