using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Utils;

[TestClass]
public class SpanSplitTests
{
    [TestMethod]
    public void SpanSplit()
    {
        var str = "aaa,bbb,,ccc";
        var span = str.AsSpan();
        var strs = str.Split(',');
        var index = 0;
        foreach (var splitSpan in span.Split(','))
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.AreEqual(strs[index++], splitStr);
        }
        Assert.AreEqual(strs.Length, index);
    }

    [TestMethod]
    public void SpanSplit_RemoveEmptyEntries()
    {
        var str = "aaa,bbb,,ccc";
        var span = str.AsSpan();
        var strs = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var index = 0;
        foreach (var splitSpan in span.Split(',', true))
        {
            var splitStr = splitSpan.ToString();
            Assert.IsTrue(splitStr.Contains(',') is false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(splitStr) is false);
            Assert.AreEqual(strs[index++], splitStr);
        }
        Assert.AreEqual(strs.Length, index);
    }
}
