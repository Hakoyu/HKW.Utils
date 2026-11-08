using System.Collections.Specialized;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// 可观察集合的单向绑定扩展
/// </summary>
public static partial class ObservableCollectionExtensions
{
    #region BindingList
    /// <summary>
    /// (INotifyListChanged, ILists)
    /// 绑定列表, 将源列表的修改同步至目标列表
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceList">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingListX<T>(
        this INotifyListChanged<T> sourceList,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        return CollectionBindingRegistry.Bind<NotifyListChangeEventArgs<T>>(
            sourceList,
            targetList,
            unBinding,
            () => CollectionBindingOperations.ListX(sourceList, targetList),
            channel => sourceList.ListChanged += channel.Handle,
            channel => sourceList.ListChanged -= channel.Handle
        );
    }

    /// <summary>
    /// (INotifyCollectionChanged, ILists)
    /// 绑定列表, 将源列表的修改同步至目标列表
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceCollection">源列表</param>
    /// <param name="targetList">目标列表</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingList<T>(
        this INotifyCollectionChanged sourceCollection,
        IList<T> targetList,
        bool unBinding = false
    )
    {
        return CollectionBindingRegistry.Bind<NotifyCollectionChangedEventArgs>(
            sourceCollection,
            targetList,
            unBinding,
            () => CollectionBindingOperations.List(sourceCollection, targetList),
            channel => sourceCollection.CollectionChanged += channel.Handle,
            channel => sourceCollection.CollectionChanged -= channel.Handle
        );
    }
    #endregion

    #region BindingDictionary
    /// <summary>
    /// (INotifyDictionaryChanged, IDictionarys)
    /// 绑定字典, 将源字典的修改同步至目标字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDictionary">源字典</param>
    /// <param name="targetDictionary">目标字典</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingDictionaryX<TKey, TValue>(
        this INotifyDictionaryChanged<TKey, TValue> sourceDictionary,
        IDictionary<TKey, TValue> targetDictionary,
        bool unBinding = false
    )
        where TKey : notnull
    {
        return CollectionBindingRegistry.Bind<NotifyDictionaryChangeEventArgs<TKey, TValue>>(
            sourceDictionary,
            targetDictionary,
            unBinding,
            () => CollectionBindingOperations.DictionaryX(sourceDictionary, targetDictionary),
            channel => sourceDictionary.DictionaryChanged += channel.Handle,
            channel => sourceDictionary.DictionaryChanged -= channel.Handle
        );
    }

    /// <summary>
    /// (INotifyCollectionChanged, IDictionarys)
    /// 绑定字典, 将源字典的修改同步至目标字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDictionary">源字典</param>
    /// <param name="targetDictionary">目标字典</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingDictionary<TKey, TValue>(
        this INotifyCollectionChanged sourceDictionary,
        IDictionary<TKey, TValue> targetDictionary,
        bool unBinding = false
    )
        where TKey : notnull
    {
        return CollectionBindingRegistry.Bind<NotifyCollectionChangedEventArgs>(
            sourceDictionary,
            targetDictionary,
            unBinding,
            () => CollectionBindingOperations.Dictionary(sourceDictionary, targetDictionary),
            channel => sourceDictionary.CollectionChanged += channel.Handle,
            channel => sourceDictionary.CollectionChanged -= channel.Handle
        );
    }
    #endregion

    #region BindingSet
    /// <summary>
    /// (INotifySetChanged, ISets)
    /// 绑定集合, 将源集合的修改同步至目标集合
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingSetX<T>(
        this INotifySetChanged<T> sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        return CollectionBindingRegistry.Bind<NotifySetChangeEventArgs<T>>(
            sourceSet,
            targetSet,
            unBinding,
            () => CollectionBindingOperations.SetX(sourceSet, targetSet),
            channel => sourceSet.SetChanged += channel.Handle,
            channel => sourceSet.SetChanged -= channel.Handle
        );
    }

    /// <summary>
    /// (INotifyCollectionChanged, ISets)
    /// 绑定集合, 将源集合的修改同步至目标集合
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定, 未绑定时不执行操作</param>
    /// <returns>可重复释放的绑定令牌, 解绑调用返回空令牌</returns>
    /// <remarks>
    /// 初始化及异常约定见 <see cref="ObservableCollectionExtensions"/>. 源必须可枚举.
    /// </remarks>
    public static IDisposable BindingSet<T>(
        this INotifyCollectionChanged sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        return CollectionBindingRegistry.Bind<NotifyCollectionChangedEventArgs>(
            sourceSet,
            targetSet,
            unBinding,
            () => CollectionBindingOperations.Set(sourceSet, targetSet),
            channel => sourceSet.CollectionChanged += channel.Handle,
            channel => sourceSet.CollectionChanged -= channel.Handle
        );
    }

    #endregion
}
