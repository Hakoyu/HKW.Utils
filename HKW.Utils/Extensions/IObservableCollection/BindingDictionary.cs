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
    /// (IObservableDictionary, IDictionary)
    /// </summary>
    private static Dictionary<object, HashSet<object>> _bindingDictionarys = new();

    /// <summary>
    /// 绑定字典
    /// <para>
    /// 将源字典的修改同步至目标字典
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDictionary">源字典</param>
    /// <param name="targetDictionary">目标字典</param>
    /// <param name="unBinding">解除绑定</param>
    public static void BindingDictionary<TKey, TValue>(
        this INotifyDictionaryChanged<TKey, TValue> sourceDictionary,
        IDictionary<TKey, TValue> targetDictionary,
        bool unBinding = false
    )
        where TKey : notnull
    {
        if (unBinding)
        {
            sourceDictionary.DictionaryChanged -= SourceDictionary_DictionaryChanged;
            if (_bindingDictionarys.TryGetValue(sourceDictionary, out var tlists))
                tlists.Remove(targetDictionary);
            return;
        }
        sourceDictionary.DictionaryChanged -= SourceDictionary_DictionaryChanged;
        sourceDictionary.DictionaryChanged += SourceDictionary_DictionaryChanged;
        _bindingDictionarys.GetValueOrCreate(sourceDictionary).Add(targetDictionary);

        static void SourceDictionary_DictionaryChanged(
            INotifyDictionaryChanged<TKey, TValue> sender,
            NotifyDictionaryChangeEventArgs<TKey, TValue> e
        )
        {
            if (e.Action is DictionaryChangeAction.Add)
            {
                if (e.TryGetNewPair(out var newPair))
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Add(newPair);
                }
            }
            else if (e.Action is DictionaryChangeAction.Remove)
            {
                if (e.TryGetOldPair(out var oldPair))
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary.Add(oldPair);
                }
            }
            else if (e.Action is DictionaryChangeAction.Replace)
            {
                if (e.TryGetNewPair(out var newPair))
                {
                    foreach (
                        var dictionary in _bindingDictionarys[sender]
                            .Cast<IDictionary<TKey, TValue>>()
                    )
                        dictionary[newPair.Key] = newPair.Value;
                }
            }
            else if (e.Action is DictionaryChangeAction.Clear)
            {
                foreach (
                    var dictionary in _bindingDictionarys[sender].Cast<IDictionary<TKey, TValue>>()
                )
                    dictionary.Clear();
            }
        }
    }
}
