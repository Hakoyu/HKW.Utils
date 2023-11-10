namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 判断集合中是否含有值
    /// <para>比 <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/> 快一倍</para>
    /// <para>为 <c>Count != 0</c> 语法糖</para>
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="collection">集合</param>
    /// <returns>含有值为 <see langword="true"/> 否则为 <see langword="false"/></returns>
    public static bool HasValue<T>(this ICollection<T> collection)
    {
        return collection.Count != 0;
    }
}
