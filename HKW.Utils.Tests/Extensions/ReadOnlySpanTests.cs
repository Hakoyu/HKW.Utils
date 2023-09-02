using HKW.HKWUtils.Extensions;

namespace HKWTests.Extensions;

[TestClass]
public class ReadOnlySpanTests
{
    [TestMethod]
    public void Split()
    {
        string str = "666 666 666";
        foreach (var six in str.AsSpan().Split(' '))
        {
            Assert.AreEqual(six.ToString(), "666");
        }
    }
}
