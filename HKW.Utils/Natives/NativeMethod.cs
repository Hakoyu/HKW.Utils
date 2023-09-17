using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Natives;

internal static class NativeMethod
{
    public static IList<T> CreateReadOnlyList<T>(T item)
    {
        return new ReadOnlyCollection<T>(new List<T>() { item });
    }

    public static IList<T> CreateReadOnlyList<T>(IList<T> items)
    {
        return new ReadOnlyCollection<T>(new List<T>(items));
    }
}
