namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 返回一个遍历 <see cref="ReadOnlySpan{T}"/> 的枚举器
    /// 它被 <paramref name="separator"/> 分隔
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="span">源</param>
    /// <param name="separator">用来分隔 <paramref name="span"/></param>
    /// <param name="options">分隔设置 <see cref="StringSplitOptions"/></param>
    /// <returns>返回指定序列的枚举器</returns>
    public static SpanSplitEnumerator<T> Split<T>(
        this ReadOnlySpan<T> span,
        T separator,
        StringSplitOptions options = StringSplitOptions.None
    )
        where T : IEquatable<T>
    {
        return new SpanSplitEnumerator<T>(
            span,
            separator,
            options == StringSplitOptions.RemoveEmptyEntries
        );
    }
}
