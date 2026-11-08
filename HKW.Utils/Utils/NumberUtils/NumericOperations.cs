using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    private static T ArithmeticCore<T>(T left, T right, ArithmeticOperatorType operatorType)
        where T : struct, INumber<T>
    {
        // C# 小整数运算先提升到 int, 再截断到目标位宽.
        if (
            typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(byte)
            || typeof(T) == typeof(short)
            || typeof(T) == typeof(ushort)
        )
            return T.CreateTruncating(
                ArithmeticCore(int.CreateChecked(left), int.CreateChecked(right), operatorType)
            );

        return operatorType switch
        {
            ArithmeticOperatorType.Addition => unchecked(left + right),
            ArithmeticOperatorType.Subtraction => unchecked(left - right),
            ArithmeticOperatorType.Multiply => unchecked(left * right),
            ArithmeticOperatorType.Division => unchecked(left / right),
            ArithmeticOperatorType.Modulus => unchecked(left % right),
            _ => throw CreateUnsupportedOperatorException(operatorType),
        };
    }

    private static bool CompareByCore<T>(T left, T right, ComparisonOperatorType operatorType)
        where T : struct, INumber<T>
    {
        return operatorType switch
        {
            ComparisonOperatorType.Equality => left == right,
            ComparisonOperatorType.Inequality => left != right,
            ComparisonOperatorType.LessThan => left < right,
            ComparisonOperatorType.GreaterThan => left > right,
            ComparisonOperatorType.LessThanOrEqual => left <= right,
            ComparisonOperatorType.GreaterThanOrEqual => left >= right,
            _ => throw CreateUnsupportedOperatorException(operatorType),
        };
    }
}
