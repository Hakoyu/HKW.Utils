using System.ComponentModel;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环接口
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
public interface ICyclic<T>
{
    /// <summary>
    /// 当前项目
    /// </summary>
    public T Current { get; }

    /// <summary>
    /// 自动重置
    /// </summary>
    [DefaultValue(false)]
    public bool AutoReset { get; set; }

    /// <summary>
    /// 移动到下一个项目
    /// </summary>
    /// <returns>移动成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool MoveNext();

    /// <summary>
    /// 重置循环
    /// </summary>
    public void Reset();
}
