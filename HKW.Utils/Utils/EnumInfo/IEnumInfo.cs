using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

/// <summary>
/// 枚举信息接口
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public interface IEnumInfo<TEnum> : IEnumInfo, IEquatable<IEnumInfo<TEnum>>, IEquatable<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public new TEnum Value { get; }
}

/// <summary>
/// 枚举信息接口
/// </summary>
public interface IEnumInfo
{
    /// <summary>
    /// 枚举值
    /// </summary>
    public Enum Value { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 短名称
    /// </summary>
    public string ShortName { get; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 显示
    /// </summary>
    public DisplayAttribute? Display { get; }

    ///// <summary>
    ///// 枚举类型
    ///// </summary>
    //public Type EnumType { get; }

    ///// <summary>
    ///// 是可标记的
    ///// </summary>
    //public bool IsFlagable { get; }
}
