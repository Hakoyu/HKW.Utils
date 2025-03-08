namespace HKW.HKWUtils;

/// <summary>
/// 文件工具
/// </summary>
public static class FileUtils
{
    /// <summary>
    /// 比较两个文件
    /// </summary>
    /// <param name="file1">文件1</param>
    /// <param name="file2">文件2</param>
    /// <param name="chunkSize">缓冲区大小</param>
    /// <returns>相同为 <see langword="true"/>, 不相同为 <see langword="false"/></returns>
    public static bool Compare(string file1, string file2, int chunkSize = 4096)
    {
        var fileInfo1 = new FileInfo(file1);
        var fileInfo2 = new FileInfo(file2);
        return Compare(fileInfo1, fileInfo2);
    }

    /// <summary>
    /// 比较两个文件
    /// </summary>
    /// <param name="fileInfo1">文件1</param>
    /// <param name="fileInfo2">文件2</param>
    /// <param name="chunkSize">缓冲区大小</param>
    /// <returns>相同为 <see langword="true"/>, 不相同为 <see langword="false"/></returns>
    public static bool Compare(FileInfo fileInfo1, FileInfo fileInfo2, int chunkSize = 4096)
    {
        if (fileInfo1.Exists is false || fileInfo2.Exists is false)
            return false;
        if (fileInfo1.Length != fileInfo2.Length)
            return false;

        using var stream1 = fileInfo1.OpenRead();
        using var stream2 = fileInfo2.OpenRead();
        var buffer1 = new byte[chunkSize];
        var buffer2 = new byte[chunkSize];

        while (true)
        {
            var count1 = StreamUtils.ReadIntoBuffer(stream1, buffer1);
            var count2 = StreamUtils.ReadIntoBuffer(stream2, buffer2);

            if (count1 != count2)
                return false;

            if (count1 == 0)
                return true;

            Span<byte> span1 = buffer1;
            Span<byte> span2 = buffer2;
            if (span1.SequenceEqual(span2) is false)
                return false;
        }
    }
}
