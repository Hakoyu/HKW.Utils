using System;
using System.Collections.Generic;
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
