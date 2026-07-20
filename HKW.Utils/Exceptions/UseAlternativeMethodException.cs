using System;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Exceptions;

/// <summary>
/// 指示当前方法不受支持，并提供应使用的替代方法。
/// </summary>
public sealed class UseAlternativeMethodException : NotSupportedException
{
    /// <summary>
    /// 替代的方法名称
    /// </summary>
    public string AlternativeMethodName { get; }

    /// <inheritdoc/>
    /// <param name="alternativeMethodName">替代的方法名称</param>
    public UseAlternativeMethodException(string alternativeMethodName)
        : base($"This method is not supported. Use '{alternativeMethodName}' instead.")
    {
        AlternativeMethodName = alternativeMethodName;
    }
}
