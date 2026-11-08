using System.Buffers;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

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
    public void EmptyStringConversionsRemainEmpty()
    {
        Assert.AreEqual(string.Empty, string.Empty.FirstToLower());
        Assert.AreEqual(string.Empty, string.Empty.FirstToUpper());
        Assert.AreEqual(string.Empty, string.Empty.FirstToUpper(otherToLower: true));
        Assert.AreEqual(string.Empty, string.Empty.ToPascal('_'));
        Assert.AreEqual(string.Empty, string.Empty.ToPascal(SearchValues.Create("_")));
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
