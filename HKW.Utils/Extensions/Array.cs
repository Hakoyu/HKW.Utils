using System.Text;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    extension<T>(T[,] array)
    {
        #region ToStringX
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <returns>字符串</returns>
        public string ToStringX(string separator = " ")
        {
            var sb = new StringBuilder();
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    sb.Append(array[i, j]);
                    sb.Append(separator);
                }
                sb.Remove(sb.Length - separator.Length, separator.Length);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="getString">获取字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns>字符串</returns>
        public string ToStringX(
            Func<T, string> getString,
            string separator = " "
        )
        {
            var sb = new StringBuilder();
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    sb.Append(getString(array[i, j]));
                    sb.Append(separator);
                }
                sb.Remove(sb.Length - separator.Length, separator.Length);
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion ToStringX

    }
}
