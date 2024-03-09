using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Utils;

[TestClass]
public class CharSpanSplitTests
{
    [TestMethod]
    public void CharSpanSplit()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(new char[] { ',', ';' });
        var index = 0;
        foreach (var splitSpan in span.Split(StringSplitOptions.None, ',', ';'))
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.IsTrue(splitStr.Contains(';') is false);
            Assert.IsTrue(splitStr == strs[index++]);
        }
        Assert.IsTrue(index == strs.Length);
    }

    [TestMethod]
    public void CharSpanSplit_RemoveEmptyEntries()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        var index = 0;
        foreach (var splitSpan in span.Split(StringSplitOptions.RemoveEmptyEntries, ',', ';'))
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.IsTrue(splitStr.Contains(';') is false);
            Assert.IsTrue(string.IsNullOrEmpty(splitStr) is false);
            Assert.IsTrue(splitStr == strs[index++]);
        }
        Assert.IsTrue(index == strs.Length);
    }

    [TestMethod]
    public void CharSpanSplit_TrimEntries()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(new char[] { ',', ';' }, StringSplitOptions.TrimEntries);
        var index = 0;
        foreach (var splitSpan in span.Split(StringSplitOptions.TrimEntries, ',', ';'))
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.IsTrue(splitStr.Contains(';') is false);
            Assert.IsTrue(splitStr.Contains(' ') is false);
            Assert.IsTrue(splitStr == strs[index++]);
        }
        Assert.IsTrue(index == strs.Length);
    }

    [TestMethod]
    public void CharSpanSplit_RemoveEmptyAndTrimEntries()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(
            new char[] { ',', ';' },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );
        var index = 0;
        foreach (
            var splitSpan in span.Split(
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries,
                ',',
                ';'
            )
        )
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.IsTrue(splitStr.Contains(';') is false);
            Assert.IsTrue(splitStr.Contains(' ') is false);
            Assert.IsTrue(string.IsNullOrEmpty(splitStr) is false);
            Assert.IsTrue(splitStr == strs[index++]);
        }
        Assert.IsTrue(index == strs.Length);
    }
}
