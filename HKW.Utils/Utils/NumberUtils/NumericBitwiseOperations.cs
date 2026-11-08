using System.Numerics;

namespace HKW.HKWUtils;

public static partial class NumberUtils
{
    private static T BitwiseCore<T>(T left, T right, BitwiseOperatorType operatorType)
        where T : struct, IBinaryInteger<T>
    {
        return operatorType switch
        {
            BitwiseOperatorType.Or => left | right,
            BitwiseOperatorType.And => left & right,
            BitwiseOperatorType.LogicalOr => left ^ right,
            _ => throw CreateUnsupportedOperatorException(operatorType),
        };
    }

    private static T ShiftCore<T>(T value, int shift, BitwiseShiftType operatorType)
        where T : struct, IBinaryInteger<T>
    {
        // 保留小整数提升后的 32 位移位掩码, 不使用其泛型数学位宽.
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short))
            return T.CreateTruncating(ShiftCore(int.CreateChecked(value), shift, operatorType));
        if (typeof(T) == typeof(ushort))
            return T.CreateTruncating(ShiftCore(uint.CreateChecked(value), shift, operatorType));

        return operatorType switch
        {
            BitwiseShiftType.Left => value << shift,
            BitwiseShiftType.Right => value >> shift,
            BitwiseShiftType.UnsignedRight => value >>> shift,
            _ => throw CreateUnsupportedOperatorException(operatorType),
        };
    }
}
