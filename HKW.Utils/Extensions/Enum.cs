using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    extension<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="flag">标签</param>
        /// <returns>添加标签的枚举</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEnum AddFlag(TEnum flag)
        {
            return (TEnum)
                NumberUtils.BitwiseOperatorF(
                    value,
                    flag,
                    EnumInfo<TEnum>.UnderlyingType,
                    BitwiseOperatorType.Or
                );
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="flag">标签</param>
        /// <returns>删除标签的枚举</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEnum RemoveFlag(TEnum flag)
        {
            var type = EnumInfo<TEnum>.UnderlyingType;
            return (TEnum)
                NumberUtils.BitwiseOperatorF(
                    value,
                    NumberUtils.BitwiseComplementF(flag, type),
                    type,
                    BitwiseOperatorType.And
                );
        }

        /// <summary>
        /// 获取枚举信息
        /// </summary>
        /// <returns>枚举信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EnumInfo<TEnum> GetInfo()
        {
            return EnumInfo<TEnum>.GetInfo(value);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>目标信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetDisplayInfo(EnumInfoDisplayTarget target)
        {
            var info = value.GetInfo();
            return target switch
            {
                EnumInfoDisplayTarget.Name => info.DisplayName,
                EnumInfoDisplayTarget.ShortName => info.DisplayShortName,
                EnumInfoDisplayTarget.Description => info.DisplayDescription,
                _ => info.DisplayName,
            };
        }
    }

    /// <param name="value">枚举值</param>
    extension(Enum value)
    {
        /// <summary>
        /// 获取枚举信息
        /// </summary>
        /// <returns>枚举信息接口</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumInfo GetInfo()
        {
            var type = value.GetType();
            if (EnumInfo.InfosByType.TryGetValue(type, out var infos))
                return infos.Values.First().Create(value);
            return EnumInfo.CreateEnumInfoExpression(type).Invoke(value);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>目标信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetDisplayInfo(EnumInfoDisplayTarget target)
        {
            var info = value.GetInfo();
            return target switch
            {
                EnumInfoDisplayTarget.Name => info.DisplayName,
                EnumInfoDisplayTarget.ShortName => info.DisplayShortName,
                EnumInfoDisplayTarget.Description => info.DisplayDescription,
                _ => info.DisplayName,
            };
        }
    }
}
