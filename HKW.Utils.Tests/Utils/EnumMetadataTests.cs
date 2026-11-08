using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
[DoNotParallelize]
public class EnumMetadataTests
{
    [TestMethod]
    [DataRow(typeof(ByteEnum))]
    [DataRow(typeof(SByteEnum))]
    [DataRow(typeof(ShortEnum))]
    [DataRow(typeof(UShortEnum))]
    [DataRow(typeof(IntEnum))]
    [DataRow(typeof(UIntEnum))]
    [DataRow(typeof(LongEnum))]
    [DataRow(typeof(ULongEnum))]
    public void Metadata_SupportsEveryEnumUnderlyingType(Type enumType)
    {
        var zero = (Enum)Enum.ToObject(enumType, 0);
        var one = (Enum)Enum.ToObject(enumType, 1);
        Assert.IsTrue(zero.GetInfo().IsNone);
        Assert.IsFalse(one.GetInfo().IsNone);
        Assert.AreEqual(enumType.GetEnumUnderlyingType(), one.GetInfo().UnderlyingType);
        Assert.AreEqual(one, one.GetInfo().Value);
        typeof(EnumMetadataTests)
            .GetMethod(
                nameof(CheckFlags),
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic
            )!
            .MakeGenericMethod(enumType)
            .Invoke(null, null);
    }

    private static void CheckFlags<TEnum>()
        where TEnum : struct, Enum
    {
        var first = (TEnum)Enum.ToObject(typeof(TEnum), 1);
        var second = (TEnum)Enum.ToObject(typeof(TEnum), 2);
        var both = (TEnum)Enum.ToObject(typeof(TEnum), 3);
        Assert.AreEqual(both, first.AddFlag(second));
        Assert.AreEqual(first, both.RemoveFlag(second));
    }

    [TestMethod]
    [DataRow(0, "0")]
    [DataRow(8, "8")]
    [DataRow(9, "9")]
    [DataRow(-1, "-1")]
    public void UnknownFlags_PreserveTheirFullNumericValue(int value, string expected)
    {
        var info = EnumInfo<NoZeroFlags>.GetInfo((NoZeroFlags)value);
        Assert.AreEqual(value == 0, info.IsNone);
        Assert.AreEqual(expected, info.ToString());
        Assert.AreEqual(expected, info.DisplayName);
        Assert.AreEqual(expected, info.DisplayShortName);
        Assert.AreEqual(expected, info.DisplayDescription);
        Assert.AreEqual($"NoZeroFlags_{expected}", EnumInfo.GlobalDefaultToString1(info));
    }

    [TestMethod]
    public void DefaultInfo_UsesZeroRatherThanFirstDeclaredMember()
    {
        Assert.AreEqual(default(NoZeroFlags), EnumInfo<NoZeroFlags>.GetInfo().Value);
        Assert.IsTrue(EnumInfo<NoZeroFlags>.GetInfo().IsNone);
        Enum empty = default(EmptyEnum);
        Assert.IsTrue(empty.GetInfo().IsNone);
        Assert.IsTrue(empty.GetInfo().IsNone);
    }

    [TestMethod]
    public void Aliases_AreIndexedByValueNotName()
    {
        Assert.AreEqual(2, EnumInfo<AliasEnum>.StaticNames.Length);
        Assert.AreEqual(1, EnumInfo<AliasEnum>.StaticValues.Length);
        Assert.AreSame(AliasEnum.First.GetInfo(), AliasEnum.Second.GetInfo());
    }

    [TestMethod]
    public void Resources_AreResolvedForTheCurrentCulture()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            var info = LocalizedEnum.Value.GetInfo();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Assert.AreEqual("Name", info.DisplayName);
            Assert.AreEqual("Short", info.DisplayShortName);
            Assert.AreEqual("Description", info.DisplayDescription);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            Assert.AreEqual("名称", info.DisplayName);
            Assert.AreEqual("短名", info.DisplayShortName);
            Assert.AreEqual("描述", info.DisplayDescription);
            Assert.AreEqual("名称", LocalizedFlags.A.GetInfo().DisplayName);
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    [TestMethod]
    public void InvalidResourceMetadata_PropagatesItsError()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            _ = InvalidResourceEnum.Value.GetInfo().DisplayName;
        });
    }

    [TestMethod]
    public void FirstUseFactories_AreSafeUnderConcurrentAccess()
    {
        var infos = new IEnumInfo[64];
        Parallel.For(0, infos.Length, i => infos[i] = ((Enum)ConcurrentEnum.Value).GetInfo());
        Assert.IsTrue(infos.All(info => ReferenceEquals(infos[0], info)));
    }

    public static class Resources
    {
        public static string Label =>
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh" ? "名称" : "Name";
        public static string ShortLabel =>
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh" ? "短名" : "Short";
        public static string Description =>
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh" ? "描述" : "Description";
    }

    public enum ByteEnum : byte
    {
        None,
        Value,
    }

    public enum SByteEnum : sbyte
    {
        None,
        Value,
    }

    public enum ShortEnum : short
    {
        None,
        Value,
    }

    public enum UShortEnum : ushort
    {
        None,
        Value,
    }

    public enum IntEnum : int
    {
        None,
        Value,
    }

    public enum UIntEnum : uint
    {
        None,
        Value,
    }

    public enum LongEnum : long
    {
        None,
        Value,
    }

    public enum ULongEnum : ulong
    {
        None,
        Value,
    }

    private enum EmptyEnum { }

    private enum AliasEnum
    {
        First = 1,
        Second = 1,
    }

    private enum ConcurrentEnum : ushort
    {
        Value = 1,
    }

    [Flags]
    private enum NoZeroFlags
    {
        A = 1,
    }

    private enum LocalizedEnum
    {
        [Display(
            Name = nameof(Resources.Label),
            ShortName = nameof(Resources.ShortLabel),
            Description = nameof(Resources.Description),
            ResourceType = typeof(Resources)
        )]
        Value = 1,
    }

    [Flags]
    private enum LocalizedFlags
    {
        [Display(Name = nameof(Resources.Label), ResourceType = typeof(Resources))]
        A = 1,
    }

    private enum InvalidResourceEnum
    {
        [Display(Name = "Missing", ResourceType = typeof(Resources))]
        Value = 1,
    }
}
