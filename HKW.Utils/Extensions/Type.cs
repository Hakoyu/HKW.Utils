using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class TypeExtensions
{
    extension(Type type)
    {
        /// <inheritdoc cref="Type.IsSubclassOf(Type)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubclassOf<T>()
        {
            return type.IsSubclassOf(typeof(T));
        }
    }
}
