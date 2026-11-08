using HKW.HKWUtils.Collections;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class AttributeDictionaryTests
{
    [TestMethod]
    public void RepeatedDerivedAttributes_AreConvertedElementByElement()
    {
        var attributes = new AttributeDictionary(typeof(Decorated), false);
        var result = attributes.GetAttributes<MarkerAttribute>();
        CollectionAssert.AreEqual(new[] { 1, 2 }, result.Select(a => a.Id).ToArray());
        Assert.IsTrue(attributes.Contains<MarkerAttribute>());
        Assert.IsTrue(
            attributes.TryGetAttributes<MarkerAttribute>(
                out System.Collections.Immutable.ImmutableArray<MarkerAttribute> typed
            )
        );
        CollectionAssert.AreEqual(new[] { 1, 2 }, typed.Select(a => a.Id).ToArray());
        Assert.IsFalse(attributes.Contains<Attribute>());
        Assert.IsNull(attributes.GetAttribute<ObsoleteAttribute>());
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            attributes.GetAttributes<ObsoleteAttribute>()
        );
        Assert.IsFalse(attributes.TryGetAttributes<ObsoleteAttribute>(out var missing));
        Assert.IsTrue(missing.IsDefault);
    }

    [Marker(1), Marker(2)]
    private sealed class Decorated;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    private sealed class MarkerAttribute(int id) : Attribute
    {
        public int Id { get; } = id;
    }
}
