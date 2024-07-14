namespace HKW.HKWUtils;

/// <summary>
/// 这是一个没有任何意义的特性, 仅用于标记引用程序集, 让其不被编译器优化
/// </summary>
/// <param name="type">类型</param>
public sealed class ReferenceTypeAttribute(Type type) : Attribute
{
    /// <summary>
    /// 类型
    /// </summary>
    public Type Value { get; } = type;
}
