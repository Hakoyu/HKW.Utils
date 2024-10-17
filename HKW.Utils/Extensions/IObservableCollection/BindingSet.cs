using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// (IObservableSet, ISet)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingSets = [];

    /// <summary>
    /// 绑定集合
    /// <para>
    /// 将源集合的修改同步至目标集合
    /// </para>
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingSetX<T>(
        this INotifySetChanged<T> sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        if (unBinding)
        {
            sourceSet.SetChanged -= SourceSet_SetChanged;
            if (_bindingSets.TryGetValue(sourceSet, out var tsets))
                tsets.Remove(targetSet);
            return;
        }
        sourceSet.SetChanged -= SourceSet_SetChanged;
        sourceSet.SetChanged += SourceSet_SetChanged;
        _bindingSets.GetOrCreate(sourceSet).Add(targetSet);

        static void SourceSet_SetChanged(INotifySetChanged<T> sender, NotifySetChangeEventArgs<T> e)
        {
            if (e.Action is SetChangeAction.Add)
            {
                ArgumentNullException.ThrowIfNull(e.NewItems, nameof(e.NewItems));
                foreach (var item in e.NewItems)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.AddRange(e.NewItems);
                }
            }
            else if (e.Action is SetChangeAction.Remove)
            {
                ArgumentNullException.ThrowIfNull(e.OldItems, nameof(e.OldItems));
                foreach (var item in e.OldItems)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.Remove(item);
                }
            }
            else if (e.Action is SetChangeAction.Clear)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.Clear();
            }
            else if (e.Action is SetChangeAction.Union)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems, nameof(e.OtherItems));
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                {
                    if (set is HashSet<T> hashSet)
                        hashSet.TrimExcess();
                    set.UnionWith(e.OtherItems);
                }
            }
            else if (e.Action is SetChangeAction.Except)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems, nameof(e.OtherItems));
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.ExceptWith(e.OtherItems);
            }
            else if (e.Action is SetChangeAction.Intersect)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems, nameof(e.OtherItems));
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.IntersectWith(e.OtherItems);
            }
            else if (e.Action is SetChangeAction.SymmetricExcept)
            {
                ArgumentNullException.ThrowIfNull(e.OtherItems, nameof(e.OtherItems));
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.SymmetricExceptWith(e.OtherItems);
            }
        }
    }

    /// <summary>
    /// 绑定集合
    /// <para>
    /// 将源集合的修改同步至目标集合
    /// </para>
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源集合</param>
    /// <param name="targetSet">目标集合</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingSet<T>(
        this INotifyCollectionChanged sourceSet,
        ISet<T> targetSet,
        bool unBinding = false
    )
    {
        if (unBinding)
        {
            sourceSet.CollectionChanged -= SourceSet_CollectionChanged;
            if (_bindingSets.TryGetValue(sourceSet, out var tsets))
                tsets.Remove(targetSet);
            return;
        }
        sourceSet.CollectionChanged -= SourceSet_CollectionChanged;
        sourceSet.CollectionChanged += SourceSet_CollectionChanged;
        _bindingSets.GetOrCreate(sourceSet).Add(targetSet);

        static void SourceSet_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                {
                    if (set is HashSet<T> hashSet)
                        hashSet.TrimExcess();
                    foreach (var item in e.NewItems!.Cast<T>())
                        set.Add(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems!.Cast<T>())
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.Remove(item);
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.Clear();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
