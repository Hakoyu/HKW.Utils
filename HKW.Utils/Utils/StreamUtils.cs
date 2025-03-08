namespace HKW.HKWUtils;

/// <summary>
/// 流工具
/// </summary>
public static class StreamUtils
{
    /// <summary>
    /// 将流数据读进缓存
    /// </summary>
    /// <param name="stream">流</param>
    /// <param name="buffer">缓存</param>
    /// <returns>读取的数据大小</returns>
    public static int ReadIntoBuffer(in Stream stream, in byte[] buffer)
    {
        var bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            var read = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
            if (read == 0)
                return bytesRead;
            bytesRead += read;
        }
        return bytesRead;
    }
}
