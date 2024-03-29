using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

/// <summary>
/// ID接口
/// <typeparamref name="T">ID类型</typeparamref>
/// </summary>
public interface IID<T>
{
    /// <summary>
    /// ID
    /// </summary>
    public T ID { get; }
}
