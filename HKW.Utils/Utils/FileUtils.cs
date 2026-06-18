using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using HKW.HKWUtils.Extensions;

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
    /// <param name="chunkSize">byte缓冲区大小, 大于 4096 时将使用内存池</param>
    /// <returns>相同为 <see langword="true"/>, 不相同为 <see langword="false"/></returns>
    public static bool Compare(FileInfo fileInfo1, FileInfo fileInfo2, int chunkSize = 4096)
    {
        if (fileInfo1.Exists is false || fileInfo2.Exists is false)
            return false;
        if (fileInfo1.Length != fileInfo2.Length)
            return false;

        var result = false;

        var usePool = chunkSize > 4096;
        byte[] buffer1Array = usePool ? ArrayPool<byte>.Shared.Rent(chunkSize) : default!;
        byte[] buffer2Array = usePool ? ArrayPool<byte>.Shared.Rent(chunkSize) : default!;
        Span<byte> buffer1 = usePool ? buffer1Array : stackalloc byte[chunkSize];
        Span<byte> buffer2 = usePool ? buffer2Array : stackalloc byte[chunkSize];

        using var stream1 = fileInfo1.OpenRead();
        using var stream2 = fileInfo2.OpenRead();
        while (true)
        {
            var count1 = stream1.Read(buffer1);
            var count2 = stream2.Read(buffer2);

            if (count1 != count2)
                break;

            if (count1 == 0)
            {
                result = true;
                break;
            }

            if (buffer1.SequenceEqual(buffer2) is false)
                break;
        }

        if (usePool)
        {
            ArrayPool<byte>.Shared.Return(buffer1Array);
            ArrayPool<byte>.Shared.Return(buffer2Array);
        }
        return result;
    }
}
