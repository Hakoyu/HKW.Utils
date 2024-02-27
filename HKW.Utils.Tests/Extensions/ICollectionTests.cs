using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class ICollectionTests
{
    [TestMethod]
    public void HasValue_True()
    {
        var list = Enumerable.Range(0, 10).ToList();
        var hasValue = list.HasValue();
        Assert.IsTrue(hasValue);
    }

    [TestMethod]
    public void HasValue_False()
    {
        var list = new List<int>();
        var hasValue = list.HasValue();
        Assert.IsTrue(hasValue is false);
    }
}
