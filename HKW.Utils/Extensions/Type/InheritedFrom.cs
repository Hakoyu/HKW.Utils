using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <inheritdoc cref="Type.IsSubclassOf(Type)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubclassOf<T>(this Type type)
    {
        return type.IsSubclassOf(typeof(T));
    }
}
