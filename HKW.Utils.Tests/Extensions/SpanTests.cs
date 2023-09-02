using HKW.HKWUtils.Extensions;

namespace HKWTests.Extensions;

[TestClass]
public class SpanTests
{
    [TestMethod]
    public void Split()
    {
        string str = "666 666 666";
        Span<char> chars = stackalloc char[str.Length];
        str.AsSpan().CopyTo(chars);
        foreach (var six in chars.Split(' '))
        {
            Assert.AreEqual(six.ToString(), "666");
        }
    }
}
