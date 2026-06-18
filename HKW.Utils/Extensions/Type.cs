using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
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
