using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class CultureUtilsTests
{
    [TestMethod]
    public void Exists()
    {
        Assert.IsTrue(CultureInfo.Exists("en"));
        Assert.IsFalse(CultureInfo.Exists("aaaa"));
    }

    [TestMethod]
    public void TryGetCultureInfo()
    {
        Assert.IsTrue(CultureInfo.TryGetCultureInfo("en", out var cultureInfo1));
        Assert.IsNotNull(cultureInfo1);
        Assert.AreEqual("en", cultureInfo1.Name);

        Assert.IsFalse(CultureInfo.TryGetCultureInfo("aaaa", out var cultureInfo2));
        Assert.IsNull(cultureInfo2);
    }
}
