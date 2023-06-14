using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class SpanT
{
    [TestMethod]
    public void Split()
    {
        string str = "666 666 666";
        Span<char> chars = stackalloc char[str.Length];
        str.AsSpan().CopyTo(chars);
        foreach (var six in chars.Split(' '))
        {
            Assert.AreEqual(six.ToString(), "666");
        }
    }
}
