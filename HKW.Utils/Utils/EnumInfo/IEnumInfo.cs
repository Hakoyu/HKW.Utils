using System.ComponentModel.DataAnnotations;

namespace HKW.HKWUtils;

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
