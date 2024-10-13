using System;
using System.Collections.Generic;
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
    private static Dictionary<object, HashSet<object>> _bindingSets = new();

    /// <summary>
    /// 绑定列表
    /// <para>
    /// 将源列表的修改同步至目标列表
    /// </para>
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="sourceSet">源列表</param>
    /// <param name="targetSet">目标列表</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingSet<T>(
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
                if (e.NewItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.AddRange(e.NewItems);
                }
            }
            else if (e.Action is SetChangeAction.Remove)
            {
                if (e.OldItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    {
                        foreach (var item in e.OldItems)
                            set.Remove(item);
                    }
                }
            }
            else if (e.Action is SetChangeAction.Clear)
            {
                foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                    set.Clear();
            }
            else if (e.Action is SetChangeAction.Union)
            {
                if (e.OtherItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.UnionWith(e.OtherItems);
                }
            }
            else if (e.Action is SetChangeAction.Except)
            {
                if (e.OtherItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.ExceptWith(e.OtherItems);
                }
            }
            else if (e.Action is SetChangeAction.Intersect)
            {
                if (e.OtherItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.IntersectWith(e.OtherItems);
                }
            }
            else if (e.Action is SetChangeAction.SymmetricExcept)
            {
                if (e.OtherItems is not null)
                {
                    foreach (var set in _bindingSets[sender].Cast<ISet<T>>())
                        set.SymmetricExceptWith(e.OtherItems);
                }
            }
        }
    }
}
