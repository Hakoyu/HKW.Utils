# HKW.Utils

Utils created by HKW

## Observable

Provide a variety of observable tools

The base class for all Observable types is [ReactiveObject](https://www.reactiveui.net/).

### ObservableCollectionWrapper

You can wrap various types of collections into the ObservableCollection type.

**ObservableList**

The wrapper can support any list that implements the `IList<T>` interface.

You can use `IListWrapper.BaseList` to access the original list.

`ObservableListWrapper` in addition to providing the `INotifyCollectionChanged` and `INotifyPropertyChanged `interfaces, it also supports `INotifyListChanging` and `INotifyListChanged`.

```csharp
var observableList = new ObservableListWrapper<int, List<int>>(new List<int>());
```

**ObservableSet**

The wrapper can support any set that implements the `ISet<T>` interface.

The recommended set type for `ObservableSetWrapper` is `OrderedSet`, as it ensures the order of items during set operations. Using the native `HashSet` may lead to incorrect item order.

You can use `ISetWrapper.BaseSet` to access the original set.

`ObservableSetWrapper` to providing the `INotifyCollectionChanged` and `INotifyPropertyChanged` interfaces, it also supports `INotifySetChanging` and `INotifySetChanged`.

```csharp
var observableSet = new ObservableSetWrapper<int, HashSet<int>>(new HashSet<int>());
```

**ObservableDictionary**

The wrapper can support any dictionary that implements the `IDictionary<TKey, TValue>` interface.

You can use `IDictionaryWrapper.BaseDictionary` to access the original dictionary.

`ObservableDictionaryWrapper` in addition to providing the `INotifyCollectionChanged` and `INotifyPropertyChanged` interfaces, it also supports `INotifyDictionaryChanging` and `INotifyDictionaryChanged`.

```csharp
var observableDictionary = new ObservableDictionaryWrapper<int, string, Dictionary<int, string>>(new Dictionary<int, string>());
```

### Drawing

Some `Drawing` types that support `INotifyPropertyChanged` are primarily intended for display purposes, as they may not offer sufficient performance for more intensive applications. These types are useful for scenarios where you need to notify the UI of property changes but do not require high performance, such as in data-binding contexts or simple visualizations.

```csharp
var point = new ObservablePoint<int>();
var range = new ObservableRange<int>();
var rect = new ObservableRectangle<int>();
var size = new ObservableSize<int>();
```

## OtherUtils