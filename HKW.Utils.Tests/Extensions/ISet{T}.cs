using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class ISetT
{
    [TestMethod]
    public void AsReadOnly()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var readOnlySet = set.AsReadOnly();
        Assert.IsTrue(set.SequenceEqual(readOnlySet));
    }
}
