using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class ArrayExtensions
{
    extension<T>(T[,] array)
    {
        #region ToDelimitedString
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <returns>字符串</returns>
        /// <remarks>
        /// 支持非零数组下界. 每行均保留末尾换行; 零列输出空行, 零行输出空字符串.
        /// </remarks>
        public string ToDelimitedString(string separator = " ")
        {
            var sb = new StringBuilder();
            var rowStart = array.GetLowerBound(0);
            var columnStart = array.GetLowerBound(1);
            var columns = array.GetLength(1);
            for (var i = 0; i < array.GetLength(0); i++)
            {
                if (columns > 0)
                {
                    sb.Append(array[rowStart + i, columnStart]);
                    for (var j = 1; j < columns; j++)
                    {
                        sb.Append(separator);
                        sb.Append(array[rowStart + i, columnStart + j]);
                    }
                }
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
        /// <remarks>
        /// 支持非零数组下界. 每行均保留末尾换行; 零列输出空行, 零行输出空字符串.
        /// </remarks>
        public string ToDelimitedString(Func<T, string> getString, string separator = " ")
        {
            var sb = new StringBuilder();
            var rowStart = array.GetLowerBound(0);
            var columnStart = array.GetLowerBound(1);
            var columns = array.GetLength(1);
            for (var i = 0; i < array.GetLength(0); i++)
            {
                if (columns > 0)
                {
                    sb.Append(getString(array[rowStart + i, columnStart]));
                    for (var j = 1; j < columns; j++)
                    {
                        sb.Append(separator);
                        sb.Append(getString(array[rowStart + i, columnStart + j]));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion ToDelimitedString
    }
}
