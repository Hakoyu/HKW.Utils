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
    /// <param name="expression">属性访问表达式</param>
    /// <returns>属性名</returns>
    /// <exception cref="ArgumentException">表达式不是属性访问表达式</exception>
    /// <remarks><![CDATA[
    /// GetPropertyName(x => x.ID) // "ID"
    /// ]]></remarks>
    public static string GetPropertyName(this LambdaExpression expression)
    {
        if (expression.Body is MemberExpression member)
        {
            return member.Member.Name;
        }
        else if (
            expression.Body is UnaryExpression unary
            && unary.Operand is MemberExpression unaryMember
        )
        {
            return unaryMember.Member.Name;
        }
        else
            throw new ArgumentException($"Not supported expression \"{expression}\"");
    }
}
