using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void FirstToUpper()
    {
        string str1 = "red";
        string str2 = HKWExtensions.FirstToUpper(str1);
        Assert.IsTrue(str2 == "Red");
    }

    [TestMethod]
    public void FirstLetterCapital_OtherToLower()
    {
        string str1 = "rEd";
        string str2 = HKWExtensions.FirstToUpper(str1);
        Assert.IsTrue(str2 == "REd");
        string str3 = str1.FirstToUpper(true);
        Assert.IsTrue(str3 == "Red");
    }

    [TestMethod]
    public void ToPascal()
    {
        string str1 = "red red red";
        string str2 = str1.ToPascal();
        Assert.IsTrue(str2 == "RedRedRed");
    }
}
