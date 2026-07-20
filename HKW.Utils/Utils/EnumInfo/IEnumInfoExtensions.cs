using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils.Utils.EnumInfo;

/// <summary>
///
/// </summary>
public static class IEnumInfoExtensions
{
    extension(IEnumInfo enumInfo)
    {
        /// <inheritdoc/>
        public static bool operator ==(IEnumInfo a, Enum b)
        {
            return a.Value.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(IEnumInfo a, Enum b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public static bool operator ==(Enum a, IEnumInfo b)
        {
            return a.Equals(b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=(Enum a, IEnumInfo b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public static bool operator ==(IEnumInfo a, IEnumInfo b)
        {
            return a.Value.Equals(b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=(IEnumInfo a, IEnumInfo b)
        {
            return !(a == b);
        }
    }

    extension<TEnum>(IEnumInfo<TEnum> enumInfo)
        where TEnum : struct, Enum
    {
        /// <inheritdoc/>
        public static bool operator ==(IEnumInfo<TEnum> a, TEnum b)
        {
            return EqualityComparer<TEnum>.Default.Equals(a.Value, b);
        }

        /// <inheritdoc/>
        public static bool operator !=(IEnumInfo<TEnum> a, TEnum b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public static bool operator ==(TEnum a, IEnumInfo<TEnum> b)
        {
            return EqualityComparer<TEnum>.Default.Equals(a, b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=(TEnum a, IEnumInfo<TEnum> b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public static bool operator ==(IEnumInfo<TEnum> a, IEnumInfo<TEnum> b)
        {
            return EqualityComparer<TEnum>.Default.Equals(a.Value, b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=(IEnumInfo<TEnum> a, IEnumInfo<TEnum> b)
        {
            return !(a == b);
        }
    }
}
