namespace HKW.HKWUtils;

/// <summary>
/// 行分割内容
/// </summary>
public readonly ref struct LineSplitEntry
{
    /// <inheritdoc/>
    /// <param name="line">行</param>
    /// <param name="separator">分割字符</param>
    public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
    {
        Line = line;
        Separator = separator;
    }

    /// <summary>
    /// 行
    /// </summary>
    public ReadOnlySpan<char> Line { get; }

    /// <summary>
    /// 分割字符
    /// </summary>
    public ReadOnlySpan<char> Separator { get; }

    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="line">行</param>
    /// <param name="separator">分割字符串</param>
    public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
    {
        line = Line;
        separator = Separator;
    }

    /// <summary>
    /// 转换器
    /// </summary>
    /// <param name="entry">条目</param>
    public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
}
