using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
