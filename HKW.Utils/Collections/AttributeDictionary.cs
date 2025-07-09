using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 特性字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class AttributeDictionary
    : IDictionary<Type, Attribute>,
        IReadOnlyDictionary<Type, Attribute>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="inherit">包括继承特性</param>
    public AttributeDictionary(MemberInfo memberInfo, bool inherit)
    {
        _dictionary = Attribute
            .GetCustomAttributes(memberInfo, inherit)
            .ToImmutableDictionary(attr => attr.GetType(), attr => attr);
    }
    #endregion
    private readonly ImmutableDictionary<Type, Attribute> _dictionary;

    #region Attribute
    /// <summary>
    /// 包含指定类型的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>包含为 <see langword="true"/>, 否则为 <see langword="false"/></returns>
    public bool Contains<TAttribute>()
        where TAttribute : Attribute
    {
        return _dictionary.ContainsKey(typeof(TAttribute));
    }

    /// <summary>
    /// 获取指定类型的特性,若存在多个特性则返回第一个找到的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>指定类型的特性,若存在多个特性则返回第一个找到的特性</returns>
    public TAttribute? GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        if (_dictionary.TryGetValue(typeof(TAttribute), out var attribute))
            return (TAttribute)attribute;
        else
            return null;
    }

    /// <summary>
    /// 获取指定类型的所有特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>指定类型的所有特性</returns>
    public TAttribute[] GetAttributes<TAttribute>()
        where TAttribute : Attribute
    {
        return _dictionary
            .Where(kv => kv.Key == typeof(TAttribute))
            .Select(kv => kv.Value)
            .Cast<TAttribute>()
            .ToArray();
    }

    /// <summary>
    /// 尝试获取指定类型的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="attribute">指定类型的特性,若存在多个特性则返回第一个找到的特性</param>
    /// <returns>获取成功为 <see langword="true"/>, 否则为 <see langword="false"/></returns>
    public bool TryGetAttribute<TAttribute>([MaybeNullWhen(false)] out TAttribute attribute)
        where TAttribute : Attribute
    {
        if (_dictionary.TryGetValue(typeof(TAttribute), out var attr))
        {
            attribute = (TAttribute)attr;
            return true;
        }
        else
        {
            attribute = null;
            return false;
        }
    }

    /// <summary>
    /// 尝试获取指定类型的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="attributes">指定类型的所有特性</param>
    /// <returns>获取成功为 <see langword="true"/>, 否则为 <see langword="false"/></returns>
    public bool TryGetAttributes<TAttribute>([MaybeNullWhen(false)] out TAttribute[] attributes)
        where TAttribute : Attribute
    {
        if (Contains<TAttribute>())
        {
            attributes = GetAttributes<TAttribute>();
            return true;
        }
        else
        {
            attributes = null;
            return false;
        }
    }
    #endregion

    #region IDictionary
    /// <inheritdoc/>
    public Attribute this[Type key]
    {
        get => ((IDictionary<Type, Attribute>)_dictionary)[key];
        set => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ICollection<Type> Keys => ((IDictionary<Type, Attribute>)_dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<Attribute> Values => ((IDictionary<Type, Attribute>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<Type, Attribute>>)_dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<Type, Attribute>>)_dictionary).IsReadOnly;

    IEnumerable<Type> IReadOnlyDictionary<Type, Attribute>.Keys =>
        ((IReadOnlyDictionary<Type, Attribute>)_dictionary).Keys;

    IEnumerable<Attribute> IReadOnlyDictionary<Type, Attribute>.Values =>
        ((IReadOnlyDictionary<Type, Attribute>)_dictionary).Values;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IDictionary)_dictionary).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

    /// <inheritdoc/>
    void IDictionary<Type, Attribute>.Add(Type key, Attribute value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<Type, Attribute>>.Add(KeyValuePair<Type, Attribute> item)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<Type, Attribute> item)
    {
        return ((ICollection<KeyValuePair<Type, Attribute>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(Type key)
    {
        return ((IDictionary<Type, Attribute>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Type, Attribute>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Type, Attribute>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Type, Attribute>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Type, Attribute>>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(Type key)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<Type, Attribute> item)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out Attribute value)
    {
        return ((IDictionary<Type, Attribute>)_dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    #endregion
}
