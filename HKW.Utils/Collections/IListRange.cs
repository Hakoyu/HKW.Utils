﻿namespace HKW.HKWUtils.Collections;

/// <summary>
/// 列表范围改变接口
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
public interface IListRange<T> : IList<T>
{
    /// <summary>
    /// 范围添加
    /// </summary>
    /// <param name="collection">集合</param>
    public void AddRange(IEnumerable<T> collection);

    /// <summary>
    /// 范围插入
    /// </summary>
    /// <param name="index">起始索引</param>
    /// <param name="collection">集合</param>
    public void InsertRange(int index, IEnumerable<T> collection);

    /// <summary>
    /// 范围删除
    /// </summary>
    /// <param name="index">起始索引</param>
    /// <param name="count">数量</param>
    public void RemoveRange(int index, int count);

    /// <summary>
    /// 删除全部符合条件的项目
    /// </summary>
    /// <param name="match">条件</param>
    public void RemoveAll(Predicate<T> match);

    /// <summary>
    /// 反转列表
    /// </summary>
    public void Reverse();

    /// <summary>
    /// 反转列表
    /// </summary>
    /// <param name="index">起始索引</param>
    /// <param name="count">数量</param>
    public void Reverse(int index, int count);
}
