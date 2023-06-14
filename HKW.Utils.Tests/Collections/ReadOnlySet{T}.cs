using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKWTests.CollectionsTests;

[TestClass]
public class ReadOnlySetT
{
    [TestMethod]
    public void ReadOnly()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var readOnlySet = new ReadOnlySet<int>(set);
        Assert.IsTrue(set.SequenceEqual(readOnlySet));
    }
}
