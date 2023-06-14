using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class ReadOnlySpanT
{
    [TestMethod]
    public void Split()
    {
        string str = "666 666 666";
        foreach (var six in str.AsSpan().Split(' '))
        {
            Assert.AreEqual(six.ToString(), "666");
        }
    }
}
