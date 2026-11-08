using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class ObjectTests
{
    [TestMethod]
    public void CastTo_AcceptsObjectReceiver()
    {
        var method = typeof(ObjectExtensions).GetMethod("CastTo")!;
        Assert.AreEqual(typeof(object), method.GetParameters()[0].ParameterType);
    }

    [TestMethod]
    public void CastTo_PreservesClrCastSemantics()
    {
        object text = "value";
        Assert.AreEqual("value", text.CastTo<string>());
        object number = 42;
        Assert.AreEqual(42, number.CastTo<int>());
        Assert.ThrowsExactly<InvalidCastException>(() => number.CastTo<long>());
        Assert.ThrowsExactly<InvalidCastException>(() => text.CastTo<int>());
        object? missing = null;
        Assert.IsNull(missing.CastTo<string?>());
        Assert.IsNull(missing.CastTo<int?>());
        Assert.ThrowsExactly<NullReferenceException>(() => missing.CastTo<int>());
    }
}
