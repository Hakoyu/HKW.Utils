using System.Buffers;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class ReadOnlySpanTests
{
    #region SplitSearchValues
    [TestMethod]
    public void SplitSearchValues()
    {
        var str1 = "1aaa2bbb3ccc4 d 5   6";
        var str2 = "999999999";
        var str3 = "";
        var separators = new char[] { '1', '2', '3', '4', '5', '6' };

        SplitSearchValuesTest(str1, separators);
        SplitSearchValuesTest(str2, separators);
        SplitSearchValuesTest(str3, separators);
    }

    private static void SplitSearchValuesTest(string str, char[] separators)
    {
        var searchValue = SearchValues.Create(separators);
        var split = str.Split(separators);
        var span = str.AsSpan();
        var resultList = new List<string>();
        var e = span.Split(searchValue);
        while (e.MoveNext())
        {
            var result = e.CurrentValue.ToString();
            resultList.Add(result);
        }
        Assert.IsTrue(split.SequenceEqual(resultList));

        var list = e.ToList(x => x.CurrentValue.ToString());
        Assert.IsTrue(split.SequenceEqual(list));
    }
    #endregion

    #region SplitSearchChars

    [TestMethod]
    public void SplitSearchChars()
    {
        var str1 = "1aaa2bbb3ccc4 d 5   6";
        var str2 = "999999999";
        var str3 = "     ";
        var str4 = "";
        var separators = new char[] { '1', '2', '3', '4', '5', '6' };

        SplitSearchCharsTest(str1, separators, StringSplitOptions.None);
        SplitSearchCharsTest(str2, separators, StringSplitOptions.None);
        SplitSearchCharsTest(str3, separators, StringSplitOptions.None);
        SplitSearchCharsTest(str4, separators, StringSplitOptions.None);

        SplitSearchCharsTest(str1, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitSearchCharsTest(str2, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitSearchCharsTest(str3, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitSearchCharsTest(str4, separators, StringSplitOptions.RemoveEmptyEntries);

        SplitSearchCharsTest(str1, separators, StringSplitOptions.TrimEntries);
        SplitSearchCharsTest(str2, separators, StringSplitOptions.TrimEntries);
        SplitSearchCharsTest(str3, separators, StringSplitOptions.TrimEntries);
        SplitSearchCharsTest(str4, separators, StringSplitOptions.TrimEntries);

        SplitSearchCharsTest(
            str1,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitSearchCharsTest(
            str2,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitSearchCharsTest(
            str3,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitSearchCharsTest(
            str4,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
    }

    private static void SplitSearchCharsTest(
        string str,
        char[] separators,
        StringSplitOptions options
    )
    {
        var searchValue = SearchValues.Create(separators);
        var split = str.Split(separators, options);
        var span = str.AsSpan();
        var resultList = new List<string>();

        var e = span.Split(searchValue, options);
        while (e.MoveNext())
        {
            var result = e.CurrentValue.ToString();
            resultList.Add(result);
        }
        Assert.IsTrue(split.SequenceEqual(resultList));

        var list = e.ToList(x => x.CurrentValue.ToString());
        Assert.IsTrue(split.SequenceEqual(list));
    }

    #endregion

    #region SplitLine
    [TestMethod]
    public void SplitLine()
    {
        var str1 = "aaa\rbbb\nccc\r\nddd\n\reee\r\rfff\n\nggg\r\n";
        var str2 = "\r\n\r\r\n\n";
        var str3 = "999999999";
        var str4 = "     ";
        var str5 = "";
        var separators = new string[] { "\r\n", "\r", "\n" };

        SplitLineTest(str1, separators, StringSplitOptions.None);
        SplitLineTest(str2, separators, StringSplitOptions.None);
        SplitLineTest(str3, separators, StringSplitOptions.None);
        SplitLineTest(str4, separators, StringSplitOptions.None);
        SplitLineTest(str5, separators, StringSplitOptions.None);

        SplitLineTest(str1, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitLineTest(str2, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitLineTest(str3, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitLineTest(str4, separators, StringSplitOptions.RemoveEmptyEntries);
        SplitLineTest(str5, separators, StringSplitOptions.RemoveEmptyEntries);

        SplitLineTest(str1, separators, StringSplitOptions.TrimEntries);
        SplitLineTest(str2, separators, StringSplitOptions.TrimEntries);
        SplitLineTest(str3, separators, StringSplitOptions.TrimEntries);
        SplitLineTest(str4, separators, StringSplitOptions.TrimEntries);
        SplitLineTest(str5, separators, StringSplitOptions.TrimEntries);

        SplitLineTest(
            str1,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitLineTest(
            str2,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitLineTest(
            str3,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitLineTest(
            str4,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        SplitLineTest(
            str5,
            separators,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
    }

    private static void SplitLineTest(string str, string[] separators, StringSplitOptions options)
    {
        var span = str.AsSpan();
        var split = str.Split(separators, options);
        var resultList = new List<string>();
        var e = span.SplitLine(options);
        while (e.MoveNext())
        {
            var result = e.CurrentValue.ToString();
            resultList.Add(result);
        }
        Assert.IsTrue(split.SequenceEqual(resultList));

        var list = e.ToList(x => x.CurrentValue.ToString());
        Assert.IsTrue(split.SequenceEqual(list));
    }

    #endregion
}
