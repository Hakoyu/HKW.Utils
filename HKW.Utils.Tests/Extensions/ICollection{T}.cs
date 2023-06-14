using System.Collections.Generic;
using System.Linq;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class ICollectionT
{
    [TestMethod]
    public void HasValue_True()
    {
        var list = new List<int>(Enumerable.Range(0, 10));
        var hasValue = list.HasValue();
        Assert.IsTrue(hasValue);
    }

    [TestMethod]
    public void HasValue_False()
    {
        var list = new List<int>();
        var hasValue = list.HasValue();
        Assert.IsFalse(hasValue);
    }
}
