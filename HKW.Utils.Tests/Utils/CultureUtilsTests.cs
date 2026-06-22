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
        if (CultureInfo.TryGetCultureInfo("en", out var cultureInfo1))
            Assert.IsNotNull(cultureInfo1);

        if (CultureInfo.TryGetCultureInfo("aaaa", out var cultureInfo2) is false)
            Assert.IsNull(cultureInfo2);
    }
}
