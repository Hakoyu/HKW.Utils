using HKW.HKWUtils;

namespace HKW.HKWUtils.Tests.Utils;

[TestClass]
public class CultureUtilsTests
{
    [TestMethod]
    public void Exists()
    {
        Assert.IsTrue(CultureUtils.Exists("en"));
        Assert.IsTrue(CultureUtils.Exists("aaaa") is false);
    }

    [TestMethod]
    public void TryGetCultureInfo()
    {
        if (CultureUtils.TryGetCultureInfo("en", out var cultureInfo1))
            Assert.IsNotNull(cultureInfo1);

        if (CultureUtils.TryGetCultureInfo("aaaa", out var cultureInfo2) is false)
            Assert.IsNull(cultureInfo2);
    }
}
