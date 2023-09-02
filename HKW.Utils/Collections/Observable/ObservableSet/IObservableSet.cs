using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察集合接口
/// </summary>
public interface IObservableSet<T>
    : ISet<T>,
        INotifySetChanging<T>,
        INotifySetChanged<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
{
    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<T>? Comparer { get; }

    /// <summary>
    /// 启用通知集合修改
    /// <para>
    /// 启用后 执行方法:
    /// <see cref="ObservableSet{T}.IntersectWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.ExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.SymmetricExceptWith(IEnumerable{T})"/>,
    /// <see cref="ObservableSet{T}.UnionWith(IEnumerable{T})"/>
    /// 将会触发
    /// <see cref="ObservableSet{T}.CollectionChanged"/>
    /// </para>
    /// <para> 并启用属性:
    /// <see cref="NotifySetChangingEventArgs{T}.NewItems"/>
    /// <see cref="NotifySetChangingEventArgs{T}.OldItems"/>
    /// <see cref="NotifySetChangedEventArgs{T}.NewItems"/>
    /// <see cref="NotifySetChangedEventArgs{T}.OldItems"/>
    /// </para>
    /// </summary>
    public bool NotifySetModifies { get; set; }
}
