using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 字典改变方案
/// </summary>
public enum DictionaryChangeMode
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
    /// 修改值
    /// </summary>
    ValueChange,
}
