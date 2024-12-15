using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

/// <summary>
/// 可重置接口, 用于需要复用的类型中
/// </summary>
public interface IResettable
{
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset();
}
