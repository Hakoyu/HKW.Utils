using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils;

/// <summary>
/// 表达式扩展方法
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 获取表达式属性名
    /// </summary>
    /// <param name="expression">属性访问表达式, 可包含内置转换, 但不接受字段, 算术运算或用户定义转换</param>
    /// <returns>属性名</returns>
    /// <exception cref="ArgumentException">表达式不是属性访问表达式</exception>
    /// <remarks>
    /// <![CDATA[
    /// GetPropertyName(x => x.ID) // "ID"
    /// ]]>
    /// </remarks>
    public static string GetPropertyName(this LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var body = expression.Body;
        while (
            body
                is UnaryExpression
                {
                    NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked,
                    Method: null
                } conversion
        )
        {
            body = conversion.Operand;
        }
        if (body is MemberExpression { Member: System.Reflection.PropertyInfo property })
            return property.Name;
        throw new ArgumentException(
            $"The expression must access a property: \"{expression}\".",
            nameof(expression)
        );
    }
}
