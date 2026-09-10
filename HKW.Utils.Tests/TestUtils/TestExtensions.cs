using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace HKW.HKWUtilsTests;

public static class TestExtensions
{
    /// <summary>
    /// 可使用匿名方法作为比较器的序列相等
    /// </summary>
    /// <param name="source">源比较器</param>
    /// <param name="second">第二个集合</param>
    /// <param name="comparer">比较器</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    public static bool SequenceEqual<T>(
        this IEnumerable<T> source,
        IEnumerable<T> second,
        Func<T, T, bool> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(comparer);
        var c = EqualityComparer<T>.Create(comparer!);
        return source.SequenceEqual(second, c);
    }

    extension(Assert assert)
    {
        public static void AreEqual<T>(
            T? expected,
            IEnumerable<T?> actuals,
            string? message = "",
            [CallerArgumentExpression("expected")] string expectedExpression = "",
            [CallerArgumentExpression("actuals")] string actualsExpression = ""
        )
        {
            foreach (var actual in actuals)
            {
                Assert.AreEqual(
                    expected,
                    actual,
                    EqualityComparer<T>.Default,
                    message,
                    expectedExpression,
                    actualsExpression
                );
            }
        }

        public static void IsTrue(
            [DoesNotReturnIf(false)] IEnumerable<bool?> conditions,
            string? message = "",
            [CallerArgumentExpression(nameof(conditions))] string conditionExpression = ""
        )
        {
            foreach (var condition in conditions)
            {
                Assert.IsTrue(condition, message, conditionExpression);
            }
        }

        public static void IsFalse(
            [DoesNotReturnIf(false)] IEnumerable<bool?> conditions,
            string? message = "",
            [CallerArgumentExpression(nameof(conditions))] string conditionExpression = ""
        )
        {
            foreach (var condition in conditions)
            {
                Assert.IsFalse(condition, message, conditionExpression);
            }
        }
    }
}
