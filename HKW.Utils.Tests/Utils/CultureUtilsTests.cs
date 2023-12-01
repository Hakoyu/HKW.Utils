using HKW.HKWUtils;

namespace HKWTests.Utils;

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
            Assert.IsTrue(cultureInfo1 is not null);

        if (CultureUtils.TryGetCultureInfo("aaaa", out var cultureInfo2) is false)
            Assert.IsTrue(cultureInfo2 is null);
    }
}
