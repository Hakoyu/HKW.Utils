using System.Numerics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

internal static class NumberTestData
{
    internal static readonly Type[] Integers =
    [
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(nint),
        typeof(nuint),
        typeof(Int128),
        typeof(UInt128),
    ];

    internal static readonly Type[] Numbers =
    [
        .. Integers,
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(Half),
    ];

    internal static void Run(Type owner, string method, Type numberType, params object[] arguments)
    {
        try
        {
            owner
                .GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(numberType)
                .Invoke(null, arguments);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }

    internal static (T Left, T Right) Operands<T>(string sample)
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        return sample switch
        {
            "basic" => (T.CreateChecked(13), T.CreateChecked(3)),
            "equal" => (T.CreateChecked(3), T.CreateChecked(3)),
            "less" => (T.CreateChecked(3), T.CreateChecked(13)),
            "max" => (T.MaxValue, T.One),
            "multiply" => (T.MaxValue, T.CreateChecked(2)),
            "min" => (T.MinValue, T.One),
            "zero" => (T.One, T.Zero),
            "negative" => (T.MinValue, T.CreateChecked(-1)),
            "fraction" => (T.CreateChecked(5.5), T.CreateChecked(2.5)),
            "nan-left" => (T.CreateChecked(double.NaN), T.One),
            "nan-right" => (T.One, T.CreateChecked(double.NaN)),
            "nan-both" => (T.CreateChecked(double.NaN), T.CreateChecked(double.NaN)),
            "infinity" => (
                T.CreateChecked(double.PositiveInfinity),
                T.CreateChecked(double.NegativeInfinity)
            ),
            _ => throw new ArgumentException("Unknown sample.", nameof(sample)),
        };
    }

    internal static IEnumerable<string> Samples(Type type)
    {
        yield return "basic";
        yield return "equal";
        yield return "less";
        yield return "max";
        yield return "multiply";
        yield return "min";
        yield return "zero";
        if (
            type != typeof(byte)
            && type != typeof(ushort)
            && type != typeof(uint)
            && type != typeof(ulong)
            && type != typeof(nuint)
            && type != typeof(UInt128)
        )
            yield return "negative";
        if (Integers.Contains(type) is false)
            yield return "fraction";
        if (type == typeof(float) || type == typeof(double) || type == typeof(Half))
        {
            yield return "nan-left";
            yield return "nan-right";
            yield return "nan-both";
            yield return "infinity";
        }
    }

    internal static void Check(Func<object> expected, params Func<object>[] operations)
    {
        object? value = null;
        Exception? error = null;
        try
        {
            value = expected();
        }
        catch (Exception exception)
        {
            error = exception;
        }
        foreach (var operation in operations)
        {
            if (error is not null)
            {
                Exception? actual = null;
                try
                {
                    operation();
                }
                catch (Exception exception)
                {
                    actual = exception;
                }
                Assert.IsNotNull(actual);
                Assert.AreEqual(error.GetType(), actual.GetType());
            }
            else
            {
                var actual = operation();
                Assert.AreEqual(value!.GetType(), actual.GetType());
                Assert.AreEqual(value, actual);
            }
        }
    }
}
