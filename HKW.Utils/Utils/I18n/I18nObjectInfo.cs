using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using HKW.HKWUtils.Utils;

namespace HKW.HKWUtils;

/// <summary>
/// I18n对象信息
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public sealed class I18nObjectInfo<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="propertyChanged">属性改变行动</param>
    public I18nObjectInfo(INotifyPropertyChangedX source, PropertyChanged propertyChanged)
    {
        Source = source;
        Source.PropertyChangedX -= Source_PropertyChangedX;
        Source.PropertyChangedX += Source_PropertyChangedX;
        OnPropertyChanged = propertyChanged;
    }

    /// <summary>
    /// 添加属性数据
    /// </summary>
    /// <param name="keyPropertyName">键属性名</param>
    /// <param name="keyValue">键值</param>
    /// <param name="targetPropertyName">目标属性名</param>
    /// <param name="retentionValueOnKeyChange">在键改变时保留值</param>
    /// <returns></returns>
    public I18nObjectInfo<TKey, TValue> AddPropertyInfo(
        string keyPropertyName,
        TKey keyValue,
        string targetPropertyName,
        bool retentionValueOnKeyChange = false
    )
    {
        if (retentionValueOnKeyChange)
            RetentionValueOnKeyChangePropertyNames.Add(keyPropertyName);

        TargetPropertyNamesWithKeyPropertyName
            .GetValueOrCreate(keyPropertyName)
            .Add(targetPropertyName);

        TargetPropertyNamesWithKey.GetValueOrCreate(keyValue).Add(targetPropertyName);
        return this;
    }

    /// <summary>
    /// 添加属性数据
    /// </summary>
    /// <param name="propertyDatas">属性数据 (键属性名, 键值, 目标属性名)</param>
    /// <param name="retentionValueOnKeyChange">在键改变时保留值</param>
    /// <returns></returns>
    public I18nObjectInfo<TKey, TValue> AddPropertyInfo(
        IEnumerable<(
            string KeyPropertyName,
            TKey KeyValue,
            string TargetPropertyName
        )> propertyDatas,
        bool retentionValueOnKeyChange = false
    )
    {
        foreach (var data in propertyDatas)
        {
            if (retentionValueOnKeyChange)
                RetentionValueOnKeyChangePropertyNames.Add(data.KeyPropertyName);
            TargetPropertyNamesWithKeyPropertyName
                .GetValueOrCreate(data.KeyPropertyName)
                .Add(data.TargetPropertyName);

            TargetPropertyNamesWithKey.GetValueOrCreate(data.KeyValue).Add(data.TargetPropertyName);
        }
        return this;
    }

    /// <summary>
    /// 源
    /// </summary>
    public INotifyPropertyChangedX Source { get; }

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
            (var oldValue, var newValue) = e.GetValue<TKey>();
            if (
                TargetPropertyNamesWithKey.TryGetValueOrCreate(
                    newValue,
                    out var newTargetPropertyNamesWithKey,
                    () => targetPropertyNamesWithKeyPropertyName.ToHashSet()
                )
            )
            {
                // 将ID对应的属性名添加至新建对应的属性名中
                newTargetPropertyNamesWithKey.UnionWith(targetPropertyNamesWithKeyPropertyName);
            }
            if (
                TargetPropertyNamesWithKey.TryGetValue(
                    oldValue,
                    out var oldTargetPropertyNamesWithKey
                )
            )
            {
                // 从旧键中去除ID对应的属性名
                oldTargetPropertyNamesWithKey.ExceptWith(targetPropertyNamesWithKeyPropertyName);
                // 如果旧键不存在值则删除
                if (oldTargetPropertyNamesWithKey.HasValue() is false)
                    TargetPropertyNamesWithKey.Remove(oldValue);
            }
            if (RetentionValueOnKeyChangePropertyNames.Contains(e.PropertyName))
                KeyChanged?.Invoke(this, (oldValue, newValue));
        }
    }

    /// <summary>
    /// 通知键对应的属性
    /// </summary>
    /// <param name="key">键</param>
    public void NotifyPropertyChangedByKey(TKey key)
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
        foreach (var pair in TargetPropertyNamesWithKey)
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
        Source.PropertyChangedX -= Source_PropertyChangedX;
    }

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
