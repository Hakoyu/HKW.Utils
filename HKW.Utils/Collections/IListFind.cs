using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 列表查询接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IListFind<T> : IList<T>
{
    #region Find
    /// <summary>
    /// 按条件寻找项目
    /// </summary>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目</returns>
    public T? Find(Predicate<T> match);

    /// <summary>
    /// 按条件寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public (int Index, T? Value) Find(int startIndex, Predicate<T> match);

    /// <summary>
    /// 按条件寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public (int Index, T? Value) Find(int startIndex, int count, Predicate<T> match);

    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFind(Predicate<T> match, [MaybeNullWhen(false)] out T item);

    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFind(int startIndex, Predicate<T> match, out (int Index, T Value) item);

    /// <summary>
    /// 尝试按条件寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">索引</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFind(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    );

    /// <summary>
    /// 按条件寻找项目索引
    /// </summary>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindIndex(Predicate<T> match);

    /// <summary>
    /// 按条件寻找项目索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindIndex(int startIndex, Predicate<T> match);

    /// <summary>
    /// 按条件寻找项目索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindIndex(int startIndex, int count, Predicate<T> match);
    #endregion

    #region FindLast
    /// <summary>
    /// 按条件从后往前寻找项目
    /// </summary>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目</returns>
    public T? FindLast(Predicate<T> match);

    /// <summary>
    /// 按条件从后往前寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public (int Index, T? Value) FindLast(int startIndex, Predicate<T> match);

    /// <summary>
    /// 按条件从后往前寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public (int Index, T? Value) FindLast(int startIndex, int count, Predicate<T> match);

    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFindLast(Predicate<T> match, [MaybeNullWhen(false)] out T item);

    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFindLast(int startIndex, Predicate<T> match, out (int Index, T Value) item);

    /// <summary>
    /// 尝试按条件从后往前寻找项目和索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">索引</param>
    /// <param name="match">条件</param>
    /// <param name="item">项目</param>
    /// <returns>找到为 <see langword="true"/> 未找到为 <see langword="false"/></returns>
    public bool TryFindLast(
        int startIndex,
        int count,
        Predicate<T> match,
        out (int Index, T Value) item
    );

    /// <summary>
    /// 按条件从后往前寻找项目索引
    /// </summary>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindLastIndex(Predicate<T> match);

    /// <summary>
    /// 按条件从后往前寻找项目索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindLastIndex(int startIndex, Predicate<T> match);

    /// <summary>
    /// 按条件从后往前寻找项目索引
    /// </summary>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目索引</returns>
    public int FindLastIndex(int startIndex, int count, Predicate<T> match);
    #endregion
}
