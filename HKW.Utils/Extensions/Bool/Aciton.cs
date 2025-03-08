using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 行动
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="trueAction">为真时行动</param>
    /// <param name="falseAction">为假时行动</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Action(this bool value, Action trueAction, Action falseAction)
    {
        ArgumentNullException.ThrowIfNull(trueAction, nameof(trueAction));
        ArgumentNullException.ThrowIfNull(falseAction, nameof(falseAction));
        if (value is true)
            trueAction();
        else
            falseAction();
        return value;
    }

    /// <summary>
    /// 为真时行动
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="action">行动</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ActionWhenTrue(this bool value, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        if (value is true)
            action();
        return value;
    }

    /// <summary>
    /// 为假时行动
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="action">行动</param>
    /// <returns>值</returns>
    public static bool ActionWhenFalse(this bool value, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        if (value is not true)
            action();
        return value;
    }
}
