using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// I18n对象信息
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
public sealed class I18nObjectInfo<TKey>
    where TKey : notnull
{
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

    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="onPropertyChanged">属性改变后事件</param>
    /// <param name="propertyDatas">属性数据</param>
    public I18nObjectInfo(
        INotifyPropertyChangedX source,
        Action<string> onPropertyChanged,
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            IEnumerable<string> TargetPropertyNames
        )> propertyDatas
    )
    {
        Source = source;
        Source.PropertyChangedX -= Source_PropertyChangedX;
        Source.PropertyChangedX += Source_PropertyChangedX;
        OnPropertyChanged = onPropertyChanged;
        foreach (var data in propertyDatas)
        {
            if (
                TargetPropertyNamesWithKeyPropertyName.TryGetValue(
                    data.KeyPropertyName,
                    out var targetPropertyNames
                )
                is false
            )
            {
                targetPropertyNames = TargetPropertyNamesWithKeyPropertyName[data.KeyPropertyName] =
                    new();
            }
            targetPropertyNames.UnionWith(data.TargetPropertyNames);

            if (
                TargetPropertyNamesWithKey.TryGetValue(data.KeyValue, out targetPropertyNames)
                is false
            )
            {
                targetPropertyNames = TargetPropertyNamesWithKey[data.KeyValue] = new();
            }
            targetPropertyNames.UnionWith(data.TargetPropertyNames);
        }
    }

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
            if (
                TargetPropertyNamesWithKey.TryGetValue(newValue, out var targetPropertyNamesWithKey)
            )
            {
                // 从旧键中去除ID对应的属性名
                TargetPropertyNamesWithKey[oldValue]
                    .ExceptWith(targetPropertyNamesWithKeyPropertyName);
                // 将ID对应的属性名添加至新建对应的属性名中
                targetPropertyNamesWithKey.UnionWith(targetPropertyNamesWithKeyPropertyName);
            }
            else
            {
                TargetPropertyNamesWithKey[newValue] =
                    targetPropertyNamesWithKeyPropertyName.ToHashSet();
            }
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
        foreach (var p in TargetPropertyNamesWithKey)
        foreach (var name in p.Value)
            OnPropertyChanged(name);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        Source.PropertyChangedX -= Source_PropertyChangedX;
    }
}
