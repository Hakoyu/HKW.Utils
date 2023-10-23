using HKW.HKWUtils.Extensions;

namespace HKWTests.Extensions;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void FirstLetterCapital()
    {
        string str1 = "red";
        string str2 = str1.FirstLetterCapital();
        Assert.IsTrue(str2 == "Red");
    }

    [TestMethod]
    public void FirstLetterCapital_OtherToLower()
    {
        string str1 = "rEd";
        string str2 = str1.FirstLetterCapital();
        Assert.IsTrue(str2 == "REd");
        string str3 = str1.FirstLetterCapital(true);
        Assert.IsTrue(str3 == "Red");
    }

    [TestMethod]
    public void ToPascal()
    {
        string str1 = "red red red";
        string str2 = str1.ToPascal();
        Assert.IsTrue(str2 == "RedRedRed");
    }

    [TestMethod]
    public void ToPascal_NotRemoveSeparator()
    {
        string str1 = "red red red";
        string str2 = str1.ToPascal(removeSeparator: false);
        Assert.IsTrue(str2 == "Red Red Red");
    }

    [TestMethod]
    public void ToPascal_OtherToLower()
    {
        string str1 = "rEd rEd rEd";
        string str2 = str1.ToPascal();
        Assert.IsTrue(str2 == "REdREdREd");
        string str3 = str1.ToPascal(otherToLower: true);
        Assert.IsTrue(str3 == "RedRedRed");
    }
}
