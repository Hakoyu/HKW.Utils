using System;
using System.Buffers;
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
        foreach (
            var splitSpan in span.Split(SearchValues.Create([',', ';']), StringSplitOptions.None)
        )
        {
            Assert.AreEqual(strs[index++], splitSpan.ToString());
        }
    }

    [TestMethod]
    public void CharSpanSplit_RemoveEmptyEntries()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        var index = 0;
        foreach (
            var splitSpan in span.Split(
                SearchValues.Create([',', ';']),
                StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            Assert.AreEqual(strs[index++], splitSpan.ToString());
        }
    }

    [TestMethod]
    public void CharSpanSplit_TrimEntries()
    {
        var str = "aaa ,; bbb,, ccc;; ";
        var span = str.AsSpan();
        var strs = str.Split(new char[] { ',', ';' }, StringSplitOptions.TrimEntries);
        var index = 0;
        foreach (
            var splitSpan in span.Split(
                SearchValues.Create([',', ';']),
                StringSplitOptions.TrimEntries
            )
        )
        {
            Assert.AreEqual(strs[index++], splitSpan.ToString());
        }
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
                SearchValues.Create([',', ';']),
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            )
        )
        {
            Assert.AreEqual(strs[index++], splitSpan.ToString());
        }
    }
}
