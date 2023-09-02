namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集合改变行动
/// </summary>
public enum SetChangeAction
{
    /// <summary>
    /// 添加
    /// </summary>
    Add,

    /// <summary>
    /// 删除
    /// </summary>
    Remove,

    /// <summary>
    /// 清理
    /// </summary>
    Clear,

    /// <summary>
    /// 集合交集
    /// </summary>
    Intersect,

    /// <summary>
    /// 集合排除
    /// </summary>
    Except,

    /// <summary>
    /// 集合相同排除 (合并后排除共有元素)
    /// <para>等同于 <c>set.Union(other).Except(set.Intersect(other))</c></para>
    /// </summary>
    SymmetricExcept,

    /// <summary>
    /// 集合合并
    /// </summary>
    Union,
}
