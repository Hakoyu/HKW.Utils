using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    extension(bool value)
    {
        /// <summary>
        /// 行动
        /// </summary>
        /// <param name="trueAction">为真时行动</param>
        /// <param name="falseAction">为假时行动</param>
        /// <returns>值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Action(Action trueAction, Action falseAction)
        {
            if (value is true)
                trueAction();
            else
                falseAction();
            return value;
        }

        /// <summary>
        /// 为真时行动
        /// </summary>
        /// <param name="action">行动</param>
        /// <returns>值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ActionWhenTrue(Action action)
        {
            if (value is true)
                action();
            return value;
        }

        /// <summary>
        /// 为假时行动
        /// </summary>
        /// <param name="action">行动</param>
        /// <returns>值</returns>
        public bool ActionWhenFalse(Action action)
        {
            if (value is not true)
                action();
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
        /// <returns>解析成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
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
