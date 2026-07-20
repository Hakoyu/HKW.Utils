using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 特性字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class AttributeDictionary
    : IDictionary<Type, ImmutableArray<Attribute>>,
        IReadOnlyDictionary<Type, ImmutableArray<Attribute>>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="inherit">包括继承特性</param>
    public AttributeDictionary(MemberInfo memberInfo, bool inherit)
    {
        var dic = new Dictionary<Type, List<Attribute>>();
        foreach (Attribute attribute in memberInfo.GetCustomAttributes(inherit))
        {
            var type = attribute.GetType();
            if (dic.TryGetValue(type, out var list) is false)
                list = dic[type] = new List<Attribute>();
            list.Add(attribute);
        }

        _dictionary = dic.ToImmutableDictionary(
            key => key.Key,
            attr => attr.Value.ToImmutableArray()
        );
    }
    #endregion
    private readonly ImmutableDictionary<Type, ImmutableArray<Attribute>> _dictionary;

    #region Attribute
    /// <summary>
    /// 包含指定类型的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>是否包含</returns>
    public bool Contains<TAttribute>()
        where TAttribute : Attribute
    {
        return _dictionary.ContainsKey(typeof(TAttribute));
    }

    /// <summary>
    /// 包含指定类型的特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <returns>是否包含</returns>
    public bool Contains(Type attributeType)
    {
        return _dictionary.ContainsKey(attributeType);
    }

    /// <summary>
    /// 包含指定类型的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>是否包含</returns>
    public bool IsDefined<TAttribute>()
        where TAttribute : Attribute
    {
        return _dictionary.ContainsKey(typeof(TAttribute));
    }

    /// <summary>
    /// 包含指定类型的特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <returns>是否包含</returns>
    public bool IsDefined(Type attributeType)
    {
        return _dictionary.ContainsKey(attributeType);
    }

    /// <summary>
    /// 获取指定类型的特性,若存在多个特性则返回第一个找到的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>指定类型的特性,若存在多个特性则返回第一个找到的特性</returns>
    public TAttribute? GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        if (_dictionary.TryGetValue(typeof(TAttribute), out var attributes))
            return (TAttribute)attributes[0];
        else
            return null;
    }

    /// <summary>
    /// 获取指定类型的特性,若存在多个特性则返回第一个找到的特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <returns>指定类型的特性,若存在多个特性则返回第一个找到的特性</returns>
    public Attribute? GetAttribute(Type attributeType)
    {
        if (_dictionary.TryGetValue(attributeType, out var attributes))
            return attributes[0];
        else
            return null;
    }

    /// <summary>
    /// 获取指定类型的所有特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>指定类型的所有特性</returns>
    public ImmutableArray<TAttribute> GetAttributes<TAttribute>()
        where TAttribute : Attribute
    {
        return _dictionary[typeof(TAttribute)].CastArray<TAttribute>();
    }

    /// <summary>
    /// 获取指定类型的所有特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <returns>指定类型的所有特性</returns>
    public ImmutableArray<Attribute> GetAttributes(Type attributeType)
    {
        return _dictionary[attributeType];
    }

    /// <summary>
    /// 尝试获取指定类型的第一个特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="attribute">指定类型的特性,若存在多个特性则返回第一个找到的特性</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetAttribute<TAttribute>([MaybeNullWhen(false)] out TAttribute attribute)
        where TAttribute : Attribute
    {
        if (_dictionary.TryGetValue(typeof(TAttribute), out var attributes))
        {
            attribute = (TAttribute)attributes[0];
            return true;
        }
        else
        {
            attribute = null;
            return false;
        }
    }

    /// <summary>
    /// 尝试获取指定类型的第一个特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <param name="attribute">指定类型的特性,若存在多个特性则返回第一个找到的特性</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetAttribute(Type attributeType, [MaybeNullWhen(false)] out Attribute attribute)
    {
        if (_dictionary.TryGetValue(attributeType, out var attributes))
        {
            attribute = attributes[0];
            return true;
        }
        else
        {
            attribute = null;
            return false;
        }
    }

    /// <summary>
    /// 尝试获取指定类型的所有特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="attributes">指定类型的所有特性</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetAttributes<TAttribute>(
        [MaybeNullWhen(false)] out ImmutableArray<Attribute> attributes
    )
        where TAttribute : Attribute
    {
        if (_dictionary.TryGetValue(typeof(TAttribute), out attributes))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 尝试获取指定类型的所有特性
    /// </summary>
    /// <param name="attributeType">特性类型</param>
    /// <param name="attributes">指定类型的所有特性</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetAttributes(
        Type attributeType,
        [MaybeNullWhen(false)] out ImmutableArray<Attribute> attributes
    )
    {
        if (_dictionary.TryGetValue(attributeType, out attributes))
            return true;
        else
            return false;
    }
    #endregion

    #region IDictionary
    /// <inheritdoc/>
    public ImmutableArray<Attribute> this[Type key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public ICollection<Type> Keys =>
        ((IDictionary<Type, ImmutableArray<Attribute>>)_dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<ImmutableArray<Attribute>> Values =>
        ((IDictionary<Type, ImmutableArray<Attribute>>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    IEnumerable<Type> IReadOnlyDictionary<Type, ImmutableArray<Attribute>>.Keys => _dictionary.Keys;

    IEnumerable<ImmutableArray<Attribute>> IReadOnlyDictionary<
        Type,
        ImmutableArray<Attribute>
    >.Values => _dictionary.Values;

    /// <inheritdoc/>
    void IDictionary<Type, ImmutableArray<Attribute>>.Add(Type key, ImmutableArray<Attribute> value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<Type, ImmutableArray<Attribute>>>.Add(
        KeyValuePair<Type, ImmutableArray<Attribute>> item
    )
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<Type, ImmutableArray<Attribute>>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<Type, ImmutableArray<Attribute>> item)
    {
        return _dictionary.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(Type key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Type, ImmutableArray<Attribute>>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Type, ImmutableArray<Attribute>>>)_dictionary).CopyTo(
            array,
            arrayIndex
        );
    }

    /// <inheritdoc/>
    bool IDictionary<Type, ImmutableArray<Attribute>>.Remove(Type key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<Type, ImmutableArray<Attribute>>>.Remove(
        KeyValuePair<Type, ImmutableArray<Attribute>> item
    )
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IDictionary<Type, ImmutableArray<Attribute>>.TryGetValue(
        Type key,
        [MaybeNullWhen(false)] out ImmutableArray<Attribute> value
    )
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    bool IReadOnlyDictionary<Type, ImmutableArray<Attribute>>.TryGetValue(
        Type key,
        [MaybeNullWhen(false)] out ImmutableArray<Attribute> value
    )
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Type, ImmutableArray<Attribute>>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    #endregion
}
