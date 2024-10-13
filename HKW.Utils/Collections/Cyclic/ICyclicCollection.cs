using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 循环集合接口
/// </summary>
/// <typeparam name="T">项类型</typeparam>
public interface ICyclicCollection<T> : ICollection<T>, ICyclic<T> { }
