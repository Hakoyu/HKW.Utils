using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 列表改变方案
/// </summary>
public enum NotifyListChangeAction
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
    /// 值改变
    /// </summary>
    ValueChange,
}
