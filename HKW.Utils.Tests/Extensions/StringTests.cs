using System.Buffers;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void FirstToLower()
    {
        Assert.AreEqual("rED", "RED".FirstToLower());
    }

    [TestMethod]
    public void FirstToUpper()
    {
        Assert.AreEqual("Red", "red".FirstToUpper());
        Assert.AreEqual("Redred", "REDRED".FirstToUpper(otherToLower: true));
    }

    [TestMethod]
    public void ToPascal()
    {
        Assert.AreEqual("RedRedRed", "red red red".ToPascal(' '));
        Assert.AreEqual("RedRedRed", "rED reD rEd".ToPascal(' ', sourceToLower: true));
        Assert.AreEqual(
            "RedRedRedRed",
            "red red-red_red".ToPascal(SearchValues.Create([' ', '-', '_']))
        );
    }
}
