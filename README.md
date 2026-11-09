# HKW.Utils

HKW 的 .NET 10 工具库. 

## 能力入口

| 领域 | 入口与边界 |
| --- | --- |
| 集合 | [Collections](HKW.Utils/Collections): 双向字典, `OrderedHashSet<T>`, 过滤包装器, 尾项撤销/重做, 只读容器 |
| 可观察对象 | [Observable](HKW.Utils/Observable): 列表/集合/字典及选择状态, 状态组, 值, 枚举, 命令, 几何对象 |
| 单向绑定 | [ObservableCollectionExtensions](HKW.Utils/Extensions/IObservableCollection.cs): 创建时同步, 持有 `IDisposable` 令牌 |
| 几何 | [Drawing](HKW.Utils/Drawing): 泛型 `Point<T>`, `Size<T>`, `Range<T>`, `Rectangle<T>` 及接口 |
| 国际化 | [I18n](HKW.Utils/Utils/I18n): 文化/键矩阵, 观察器, `I18nCore`, 可检查的部分失败状态 |
| 数值与元数据 | [NumberUtils](HKW.Utils/Utils/NumberUtils), [EnumInfo](HKW.Utils/Utils/EnumInfo), [AttributeDictionary](HKW.Utils/Collections/AttributeDictionary.cs) |
| 通用工具 | [Extensions](HKW.Utils/Extensions), [Exceptions](HKW.Utils/Exceptions), [SplitUtils](HKW.Utils/Utils/SplitUtils), [FileUtils](HKW.Utils/Utils/FileUtils.cs) |

可观察集合直接实现 .NET 通知接口及本库的 Changing/Changed 接口. 包装器操作完整源集合, 过滤结果在 `Filtered*` 属性中. 后端必须遵守稳定比较器, 独占写入等各类型契约, 不能将任意外部集合视为事务后端.

## 许可

包许可声明为 MIT.
