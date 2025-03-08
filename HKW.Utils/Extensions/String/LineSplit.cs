namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 行分割
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="stringSplitOptions">字符串分割设置</param>
    /// <returns>行分割枚举器</returns>
    public static LineSplitEnumerator LineSplit(
        this string str,
        StringSplitOptions stringSplitOptions = StringSplitOptions.None
    )
    {
        return new LineSplitEnumerator(str.AsSpan(), stringSplitOptions);
    }

    /// <summary>
    /// 行分割
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="stringSplitOptions">字符串分割设置</param>
    /// <returns>行分割枚举器</returns>
    public static LineSplitEnumerator LineSplit(
        this ReadOnlySpan<char> span,
        StringSplitOptions stringSplitOptions = StringSplitOptions.None
    )
    {
        return new LineSplitEnumerator(span, stringSplitOptions);
    }
}
