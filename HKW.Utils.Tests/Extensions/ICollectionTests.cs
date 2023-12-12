using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class ICollectionTests
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
        Assert.IsTrue(hasValue is false);
    }
}
