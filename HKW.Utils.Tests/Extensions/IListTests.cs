using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class IListTests
{
    [TestMethod]
    public void Random()
    {
        var set = Enumerable.Range(0, 10).ToList();
        var randomItem = set.Random();
        Assert.IsTrue(set.Contains(randomItem));
    }
}
