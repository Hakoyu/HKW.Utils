using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集合改变方案
/// </summary>
public enum NotifySetChangeAction
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
    /// 相交
    /// </summary>
    Intersect,

    /// <summary>
    /// 除外
    /// </summary>
    Except,

    /// <summary>
    /// 对称除外
    /// </summary>
    SymmetricExcept,

    /// <summary>
    /// 合并
    /// </summary>
    Union,
}
