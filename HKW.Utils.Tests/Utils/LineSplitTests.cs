using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

namespace HKW.HKWUtils.Tests.Utils;

[TestClass]
public class LineSplitTests
{
    [TestMethod]
    public void LineSplit()
    {
        var str = " aaa\r bbb\n ccc\r\nddd \r\reee \n\nfff \r\n\r\nggg \n\r ";
        var lines = str.Split(StringUtils.LineSeparator, StringSplitOptions.None);
        var index = 0;
        foreach (var line in str.LineSplit())
        {
            var lineStr = line.ToString();
            Assert.IsTrue(lineStr.Contains('\r') is false);
            Assert.IsTrue(lineStr.Contains('\n') is false);
            Assert.IsTrue(lineStr == lines[index++]);
        }
        Assert.IsTrue(index == lines.Length);
    }

    [TestMethod]
    public void LineSplit_RemoveEmptyEntries()
    {
        var str = " aaa\r bbb\n ccc\r\nddd \r\reee \n\nfff \r\n\r\nggg \n\r ";
        var lines = str.Split(StringUtils.LineSeparator, StringSplitOptions.RemoveEmptyEntries);
        var index = 0;
        foreach (var line in str.LineSplit(StringSplitOptions.RemoveEmptyEntries))
        {
            var lineStr = line.ToString();
            Assert.IsTrue(lineStr.Contains('\r') is false);
            Assert.IsTrue(lineStr.Contains('\n') is false);
            Assert.IsTrue(string.IsNullOrEmpty(lineStr) is false);
            Assert.IsTrue(lineStr == lines[index++]);
        }
        Assert.IsTrue(index == lines.Length);
    }

    [TestMethod]
    public void LineSplit_TrimEntries()
    {
        var str = " aaa\r bbb\n ccc\r\nddd \r\reee \n\nfff \r\n\r\nggg \n\r ";
        var lines = str.Split(StringUtils.LineSeparator, StringSplitOptions.TrimEntries);
        var index = 0;
        foreach (var line in str.LineSplit(StringSplitOptions.TrimEntries))
        {
            var lineStr = line.ToString();
            Assert.IsTrue(lineStr.Contains('\r') is false);
            Assert.IsTrue(lineStr.Contains('\n') is false);
            Assert.IsTrue(lineStr.Contains(' ') is false);
            Assert.IsTrue(lineStr == lines[index++]);
        }
        Assert.IsTrue(index == lines.Length);
    }

    [TestMethod]
    public void LineSplit_RemoveEmptyAndTrimEntries()
    {
        var str = " aaa\r bbb\n ccc\r\nddd \r\reee \n\nfff \r\n\r\nggg \n\r ";
        var lines = str.Split(
            StringUtils.LineSeparator,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        var index = 0;
        foreach (
            var line in str.LineSplit(
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            var lineStr = line.ToString();
            Assert.IsTrue(lineStr.Contains('\r') is false);
            Assert.IsTrue(lineStr.Contains('\n') is false);
            Assert.IsTrue(lineStr.Contains(' ') is false);
            Assert.IsTrue(string.IsNullOrEmpty(lineStr) is false);
            Assert.IsTrue(lineStr == lines[index++]);
        }
        Assert.IsTrue(index == lines.Length);
    }
}
