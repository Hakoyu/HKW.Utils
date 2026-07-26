using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils;

//public class I18nResource<TKey, TValue> : II18nResource, INotifyPropertyChanged
//    where TKey : notnull
//{
//    private readonly ObservableDictionary<TKey, ObservableDictionary<CultureInfo, TValue>> _dictionary =
//        new();

//    private readonly ObservableSet<CultureInfo> _cultures = new();

//    public I18nResource(
//        string resourceName,
//        GetDefaultCultureData<TKey, TValue> getDefault,
//        CultureInfo? cultureInfo
//    )
//    {
//        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
//        ResourceName = resourceName;
//        GetDefault = getDefault;
//        CurrentCulture = cultureInfo ?? CultureInfo.CurrentCulture;
//        _cultures.Add(CurrentCulture);
//        Cultures = new(_cultures);
//        GetCurrentCultureData = new(this);
//        GetCurrentCultureDataOrDefault = new(this);
//    }

//    private void DictionaryChanged(
//        IObservableDictionary<CultureInfo, TValue> sender,
//        NotifyDictionaryChangeEventArgs<CultureInfo, TValue> e
//    )
//    { }

//    /// <inheritdoc/>
//    public string ResourceName { get; }

//    /// <inheritdoc/>
//    public CultureInfo CurrentCulture
//    {
//        get => field;
//        set
//        {
//            if (field == value)
//                return;
//            field = value;
//            CurrentCultureChanged?.Invoke(this, value);
//            PropertyChanged?.Invoke(this, new(nameof(CurrentCulture)));
//        }
//    }

//    public ReadOnlyObservableSet<CultureInfo> Cultures { get; }

//    ///// <summary>
//    ///// 当添加新键或新文化时自动为
//    ///// </summary>
//    //public bool CreateDefaultValue { get; set; }

//    public GetDefaultCultureData<TKey, TValue> GetDefault { get; set; }

//    public GetDataCore GetCurrentCultureData { get; }
//    public GetDataOrDefaultCore GetCurrentCultureDataOrDefault { get; }

//    public TValue GetData(TKey key, CultureInfo? cultureInfo = null)
//    {
//        cultureInfo ??= CurrentCulture;
//        return _dictionary[key][cultureInfo];
//    }

//    public TValue GetDataOrDefault(
//        TKey key,
//        CultureInfo? cultureInfo = null,
//        GetDefaultCultureData<TKey, TValue>? getDefault = null
//    )
//    {
//        cultureInfo ??= CurrentCulture;
//        getDefault ??= GetDefault;
//        if (_dictionary.TryGetValue(key, out var dic) is false)
//            return getDefault(key, cultureInfo);
//        if (dic.TryGetValue(cultureInfo, out var value) is false)
//            return getDefault(key, cultureInfo);
//        return value;
//    }

//    public bool SetData(TKey key, TValue value, CultureInfo? cultureInfo = null)
//    {
//        ArgumentNullException.ThrowIfNull(key);
//        cultureInfo ??= CurrentCulture;
//        if (_cultures.Contains(cultureInfo) is false)
//            return false;
//        if (_dictionary.TryGetValue(key, out var dic) is false)
//        {
//            dic = _dictionary[key] = new();
//            foreach (var culture in _cultures)
//                dic.Add(culture, default!);
//            dic.DictionaryChanged += DictionaryChanged;
//        }
//        var oldValue = dic.GetValueOrDefault(cultureInfo);
//        dic[cultureInfo] = value;
//        CultureDataChanged?.Invoke(this, new(key, oldValue, value, cultureInfo));
//        return true;
//    }

//    public bool RemoveData(TKey key)
//    {
//        ArgumentNullException.ThrowIfNull(key);
//        var result = _dictionary.Remove(key, out var dic);
//        if (result)
//        {
//            dic!.DictionaryChanged -= DictionaryChanged;
//            CultureDataChanged?.Invoke(this, new(key, default, default, null));
//        }
//        return result;
//    }

//    public void ClearData()
//    {
//        foreach (var pair in _dictionary)
//        {
//            pair.Value.DictionaryChanged -= DictionaryChanged;
//            CultureDataChanged?.Invoke(this, new(pair.Key, default, default, null));
//        }
//        _dictionary.Clear();
//        _cultures.Clear();
//    }

//    public bool AddCulture(CultureInfo cultureInfo)
//    {
//        ArgumentNullException.ThrowIfNull(cultureInfo);
//        var result = _cultures.Add(cultureInfo);
//        if (result)
//        {
//            foreach (var pair in _dictionary)
//                pair.Value.Add(cultureInfo, default!);
//        }
//        return result;
//    }

//    public bool RemoveCulture(CultureInfo cultureInfo)
//    {
//        ArgumentNullException.ThrowIfNull(cultureInfo);
//        if (CurrentCulture == cultureInfo)
//            throw new ArgumentException(
//                "The deleted cultureInfo cannot be the same as CurrentCulture",
//                nameof(cultureInfo)
//            );
//        var result = _cultures.Remove(cultureInfo);
//        if (result)
//        {
//            foreach (var pair in _dictionary)
//                pair.Value.Remove(cultureInfo);
//        }
//        return result;
//    }

//    public void ClearCulture()
//    {
//        foreach (var pair in _dictionary)
//            _dictionary.Clear();
//        _cultures.Clear();
//    }

//    public bool RenameKey(TKey oldKey, TKey newKey)
//    {
//        ArgumentNullException.ThrowIfNull(oldKey);
//        ArgumentNullException.ThrowIfNull(newKey);
//        if (_dictionary.TryGetValue(oldKey, out var dic) is false)
//            return false;
//        return _dictionary.TryAdd(newKey, dic);
//    }

//    /// <summary>
//    /// 文化改变后事件
//    /// </summary>
//    public event EventHandler<CultureInfo>? CurrentCultureChanged;

//    /// <summary>
//    /// 属性改变后事件
//    /// </summary>
//    public event PropertyChangedEventHandler? PropertyChanged;

//    public event CultureDataChangedEventHander<TKey, TValue>? CultureDataChanged;

//    public class GetDataCore(I18nResource<TKey, TValue> source) : INotifyPropertyChanged
//    {
//        public TValue this[TKey key] => source.GetData(key);

//        public void Refresh()
//        {
//            PropertyChanged?.Invoke(this, new(""));
//        }

//        /// <inheritdoc/>
//        public event PropertyChangedEventHandler? PropertyChanged;
//    }

//    public class GetDataOrDefaultCore(I18nResource<TKey, TValue> source) : INotifyPropertyChanged
//    {
//        /// <summary>
//        /// 使用 this[] 获取数据或默认
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public TValue this[TKey key] => source.GetDataOrDefault(key);

//        /// <summary>
//        /// 刷新 this
//        /// </summary>
//        public void Refresh()
//        {
//            PropertyChanged?.Invoke(this, new(""));
//        }

//        /// <inheritdoc/>
//        public event PropertyChangedEventHandler? PropertyChanged;
//    }
//}
