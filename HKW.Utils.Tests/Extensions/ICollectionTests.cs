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

    [TestMethod]
    public void Random_1()
    {
        var list = Enumerable.Range(0, 10).ToList();
        var randomItem = list.Random();
        Assert.IsTrue(list.Contains(randomItem));
    }

    [TestMethod]
    public void Random_2()
    {
        var list = Enumerable.Range(0, 10).ToList();
        var random = new Random(list.GetHashCode());
        var randomItem = list.Random(random);
        Assert.IsTrue(list.Contains(randomItem));
    }

    [TestMethod]
    public void RandomOrder_1()
    {
        var list = Enumerable.Range(0, 10).ToList();
        var randomOrder = list.RandomOrder();
        Assert.IsTrue(list.ItemsEqual(randomOrder));
    }

    [TestMethod]
    public void RandomOrder_2()
    {
        var list = Enumerable.Range(0, 10).ToList();
        var random = new Random(list.GetHashCode());
        var randomOrder = list.RandomOrder();
        Assert.IsTrue(list.ItemsEqual(randomOrder));
    }
}
