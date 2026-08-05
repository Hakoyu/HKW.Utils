using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class BoolExtensions
{
    extension(bool value)
    {
        /// <summary>
        /// 行动
        /// </summary>
        /// <param name="trueAction">为真时行动</param>
        /// <param name="falseAction">为假时行动</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">参数 <paramref name="trueAction"/> 或 <paramref name="falseAction"/> 为 <see langword="null"/></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Action(Action? trueAction, Action? falseAction)
        {
            if (value is true)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();
            return value;
        }

        /// <summary>
        /// 行动
        /// </summary>
        /// <param name="obj">参数1</param>
        /// <param name="trueAction">为真时行动</param>
        /// <param name="falseAction">为假时行动</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentNullException">参数 <paramref name="trueAction"/> 或 <paramref name="falseAction"/> 为 <see langword="null"/></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Action<T>(T obj, Action<T>? trueAction, Action<T>? falseAction)
        {
            if (value is true)
                trueAction?.Invoke(obj);
            else
                falseAction?.Invoke(obj);
            return value;
        }

        /// <summary>
        /// <see langword="true"/>
        /// </summary>
        public static char TrueChar => '1';

        /// <summary>
        /// <see langword="false"/>
        /// </summary>
        public static char FalseChar => '0';

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="c">参数</param>
        /// <returns><see langword="bool"/></returns>
        /// <exception cref="ArgumentException">异常参数</exception>
        public static bool Parse(char c)
        {
            if (TryParse(c, out var result) is false)
                throw new ArgumentException($"Bad bool parameter \"{c}\"", nameof(c));
            return result;
        }

        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <param name="c">参数</param>
        /// <param name="result">结果</param>
        /// <returns>解析是否成功</returns>
        public static bool TryParse(char c, out bool result)
        {
            result = false;
            if (c == bool.TrueChar)
            {
                result = true;
                return true;
            }
            else if (c == bool.FalseChar)
            {
                result = false;
                return true;
            }
            return false;
        }
    }
}
