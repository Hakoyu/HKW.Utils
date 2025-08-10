namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 行分割
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="stringSplitOptions">字符串分割设置</param>
    /// <returns>行分割枚举器</returns>
    public static List<string> SplitLine(
        this string str,
        StringSplitOptions stringSplitOptions = StringSplitOptions.None
    )
    {
        var list = new List<string>();
        var enumerator = new LineSplitEnumerator(str.AsSpan(), stringSplitOptions);
        while (enumerator.MoveNext())
            list.Add(enumerator.Current.ToString());
        return list;
    }
}
