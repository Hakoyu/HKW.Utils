using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
[DoNotParallelize]
public class EnumInfoTests
{
    [TestMethod]
    public void EnumInfoT_InstanceProperties_ForNormalEnum()
    {
        var info = EnumInfo<TestEnum>.GetInfo(TestEnum.B);
        var untypedInfo = (IEnumInfo)info;

        Assert.AreSame(EnumInfo<TestEnum>.StaticInfoDictionary[TestEnum.B], info);

        Assert.AreEqual(TestEnum.B, info.Value);
        Assert.AreEqual(TestEnum.B, untypedInfo.Value);
        Assert.IsFalse(info.IsNone);

        Assert.AreEqual("B_Name", info.DisplayName);
        Assert.AreEqual("B_ShortName", info.DisplayShortName);
        Assert.AreEqual("B_Description", info.DisplayDescription);

        Assert.IsNotNull(info.Display);
        Assert.AreEqual("B_Name", info.Display.Name);
        Assert.AreEqual("B_ShortName", info.Display.ShortName);
        Assert.AreEqual("B_Description", info.Display.Description);

        Assert.AreEqual(typeof(TestEnum), info.EnumType);
        Assert.AreEqual(typeof(int), info.UnderlyingType);
        Assert.IsFalse(info.IsFlaggable);

        AssertSequenceEqual(info.Names, "None", "A", "B", "C");
        AssertSequenceEqual(info.ValidNames, "None", "A", "B", "C");

        Assert.AreSame(EnumInfo<TestEnum>.StaticInfoDictionary, info.InfoDictionary);
        Assert.AreSame(EnumInfo<TestEnum>.StaticValidInfoDictionary, info.ValidInfoDictionary);
        Assert.AreSame(EnumInfo<TestEnum>.StaticInfoDictionary.Untyped, untypedInfo.InfoDictionary);
        Assert.AreSame(
            EnumInfo<TestEnum>.StaticValidInfoDictionary.Untyped,
            untypedInfo.ValidInfoDictionary
        );

        AssertSequenceEqual(info.Values, TestEnum.None, TestEnum.A, TestEnum.B, TestEnum.C);
        AssertSequenceEqual(info.ValidValues, TestEnum.None, TestEnum.A, TestEnum.B, TestEnum.C);
        AssertSequenceEqual(
            untypedInfo.Values.Cast<TestEnum>(),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.ValidValues.Cast<TestEnum>(),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );

        AssertSequenceEqual(
            info.Infos.Select(static x => x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            info.ValidInfos.Select(static x => x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.Infos.Select(static x => (TestEnum)x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.ValidInfos.Select(static x => (TestEnum)x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );

        Assert.AreEqual("B_Name", info.GetDisplayName(info));
        Assert.AreEqual("B_ShortName", info.GetDisplayShortName(info));
        Assert.AreEqual("B_Description", info.GetDisplayDescription(info));
        Assert.AreEqual("B", info.ToStringFunc(info));

        static string getDisplayName(EnumInfo<TestEnum> _) => "Custom_Name";
        static string getDisplayShortName(EnumInfo<TestEnum> _) => "Custom_ShortName";
        static string getDisplayDescription(EnumInfo<TestEnum> _) => "Custom_Description";
        static string toStringFunc(EnumInfo<TestEnum> _) => "Custom_ToString";

        info.GetDisplayName = getDisplayName;
        info.GetDisplayShortName = getDisplayShortName;
        info.GetDisplayDescription = getDisplayDescription;
        info.ToStringFunc = toStringFunc;

        Assert.AreEqual("Custom_Name", info.DisplayName);
        Assert.AreEqual("Custom_ShortName", info.DisplayShortName);
        Assert.AreEqual("Custom_Description", info.DisplayDescription);
        Assert.AreEqual("Custom_ToString", info.ToString());
        Assert.AreSame(getDisplayName, info.GetDisplayName);
        Assert.AreSame(getDisplayShortName, info.GetDisplayShortName);
        Assert.AreSame(getDisplayDescription, info.GetDisplayDescription);
        Assert.AreSame(toStringFunc, info.ToStringFunc);
    }

    [TestMethod]
    public void EnumInfoT_InstanceProperties_ForFlaggableEnum()
    {
        var value = TestFlaggableEnum.A | TestFlaggableEnum.C;
        var info = EnumInfo<TestFlaggableEnum>.GetInfo(value);
        var untypedInfo = (IEnumInfo)info;

        Assert.AreEqual(value, info.Value);
        Assert.AreEqual(value, untypedInfo.Value);
        Assert.IsFalse(info.IsNone);

        Assert.AreEqual("A_Name, C_Name", info.DisplayName);
        Assert.AreEqual("A_ShortName, C_ShortName", info.DisplayShortName);
        Assert.AreEqual("A_Description, C_Description", info.DisplayDescription);

        Assert.IsNull(info.Display);

        Assert.AreEqual(typeof(TestFlaggableEnum), info.EnumType);
        Assert.AreEqual(typeof(int), info.UnderlyingType);
        Assert.IsTrue(info.IsFlaggable);

        AssertSequenceEqual(info.Names, "None", "A", "B", "C");
        AssertSequenceEqual(info.ValidNames, "A", "B", "C");

        Assert.AreSame(EnumInfo<TestFlaggableEnum>.StaticInfoDictionary, info.InfoDictionary);
        Assert.AreSame(
            EnumInfo<TestFlaggableEnum>.StaticValidInfoDictionary,
            info.ValidInfoDictionary
        );
        Assert.AreSame(
            EnumInfo<TestFlaggableEnum>.StaticInfoDictionary.Untyped,
            untypedInfo.InfoDictionary
        );
        Assert.AreSame(
            EnumInfo<TestFlaggableEnum>.StaticValidInfoDictionary.Untyped,
            untypedInfo.ValidInfoDictionary
        );

        AssertSequenceEqual(
            info.Values,
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            info.ValidValues,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.Values.Cast<TestFlaggableEnum>(),
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.ValidValues.Cast<TestFlaggableEnum>(),
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );

        AssertSequenceEqual(
            info.Infos.Select(static x => x.Value),
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            info.ValidInfos.Select(static x => x.Value),
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.Infos.Select(static x => (TestFlaggableEnum)x.Value),
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            untypedInfo.ValidInfos.Select(static x => (TestFlaggableEnum)x.Value),
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );

        Assert.AreEqual("A_Name, C_Name", info.GetDisplayName(info));
        Assert.AreEqual("A_ShortName, C_ShortName", info.GetDisplayShortName(info));
        Assert.AreEqual("A_Description, C_Description", info.GetDisplayDescription(info));
        Assert.AreEqual("A, C", info.ToStringFunc(info));
    }

    [TestMethod]
    public void EnumInfoT_StaticProperties_ForNormalEnum()
    {
        Assert.AreEqual(typeof(TestEnum), EnumInfo<TestEnum>.StaticEnumType);
        Assert.AreEqual(typeof(int), EnumInfo<TestEnum>.StaticUnderlyingType);
        Assert.IsFalse(EnumInfo<TestEnum>.StaticIsFlaggable);

        AssertSequenceEqual(EnumInfo<TestEnum>.StaticNames, "None", "A", "B", "C");
        AssertSequenceEqual(
            EnumInfo<TestEnum>.StaticValues,
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            EnumInfo<TestEnum>.StaticInfos.Select(static x => x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );

        Assert.AreEqual(4, EnumInfo<TestEnum>.StaticInfoDictionary.Count);
        Assert.AreSame(
            EnumInfo<TestEnum>.StaticInfoDictionary,
            EnumInfo<TestEnum>.StaticValidInfoDictionary
        );

        AssertSequenceEqual(EnumInfo<TestEnum>.StaticValidNames, "None", "A", "B", "C");
        AssertSequenceEqual(
            EnumInfo<TestEnum>.StaticValidValues,
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );
        AssertSequenceEqual(
            EnumInfo<TestEnum>.StaticValidInfos.Select(static x => x.Value),
            TestEnum.None,
            TestEnum.A,
            TestEnum.B,
            TestEnum.C
        );

        Assert.AreEqual(4, EnumInfo<TestEnum>.EnumDisplays.Count);
        Assert.AreEqual("None_Name", EnumInfo<TestEnum>.EnumDisplays[TestEnum.None]?.Name);
        Assert.AreEqual("A_Name", EnumInfo<TestEnum>.EnumDisplays[TestEnum.A]?.Name);
        Assert.AreEqual("B_Name", EnumInfo<TestEnum>.EnumDisplays[TestEnum.B]?.Name);
        Assert.AreEqual("C_Name", EnumInfo<TestEnum>.EnumDisplays[TestEnum.C]?.Name);

        var defaultInfo = EnumInfo<TestEnum>.GetInfo();
        var valueInfo = EnumInfo<TestEnum>.GetInfo(TestEnum.C);

        Assert.AreSame(EnumInfo<TestEnum>.StaticInfoDictionary[TestEnum.None], defaultInfo);
        Assert.AreSame(EnumInfo<TestEnum>.StaticInfoDictionary[TestEnum.C], valueInfo);
        Assert.AreEqual(TestEnum.None, defaultInfo.Value);
        Assert.AreEqual(TestEnum.C, valueInfo.Value);
    }

    [TestMethod]
    public void EnumInfoT_StaticProperties_ForFlaggableEnum()
    {
        Assert.AreEqual(typeof(TestFlaggableEnum), EnumInfo<TestFlaggableEnum>.StaticEnumType);
        Assert.AreEqual(typeof(int), EnumInfo<TestFlaggableEnum>.StaticUnderlyingType);
        Assert.IsTrue(EnumInfo<TestFlaggableEnum>.StaticIsFlaggable);

        AssertSequenceEqual(EnumInfo<TestFlaggableEnum>.StaticNames, "None", "A", "B", "C");
        AssertSequenceEqual(
            EnumInfo<TestFlaggableEnum>.StaticValues,
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            EnumInfo<TestFlaggableEnum>.StaticInfos.Select(static x => x.Value),
            TestFlaggableEnum.None,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );

        Assert.AreEqual(4, EnumInfo<TestFlaggableEnum>.StaticInfoDictionary.Count);

        AssertSequenceEqual(EnumInfo<TestFlaggableEnum>.StaticValidNames, "A", "B", "C");
        AssertSequenceEqual(
            EnumInfo<TestFlaggableEnum>.StaticValidValues,
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        AssertSequenceEqual(
            EnumInfo<TestFlaggableEnum>.StaticValidInfos.Select(static x => x.Value),
            TestFlaggableEnum.A,
            TestFlaggableEnum.B,
            TestFlaggableEnum.C
        );
        Assert.AreEqual(3, EnumInfo<TestFlaggableEnum>.StaticValidInfoDictionary.Count);
        Assert.IsFalse(
            EnumInfo<TestFlaggableEnum>.StaticValidInfoDictionary.ContainsKey(
                TestFlaggableEnum.None
            )
        );

        Assert.AreEqual(4, EnumInfo<TestFlaggableEnum>.EnumDisplays.Count);
        Assert.AreEqual(
            "None_Name",
            EnumInfo<TestFlaggableEnum>.EnumDisplays[TestFlaggableEnum.None]?.Name
        );
        Assert.AreEqual(
            "A_Name",
            EnumInfo<TestFlaggableEnum>.EnumDisplays[TestFlaggableEnum.A]?.Name
        );
        Assert.AreEqual(
            "B_Name",
            EnumInfo<TestFlaggableEnum>.EnumDisplays[TestFlaggableEnum.B]?.Name
        );
        Assert.AreEqual(
            "C_Name",
            EnumInfo<TestFlaggableEnum>.EnumDisplays[TestFlaggableEnum.C]?.Name
        );

        var defaultInfo = EnumInfo<TestFlaggableEnum>.GetInfo();
        var valueInfo = EnumInfo<TestFlaggableEnum>.GetInfo(TestFlaggableEnum.B);

        Assert.AreSame(
            EnumInfo<TestFlaggableEnum>.StaticInfoDictionary[TestFlaggableEnum.None],
            defaultInfo
        );
        Assert.AreSame(
            EnumInfo<TestFlaggableEnum>.StaticInfoDictionary[TestFlaggableEnum.B],
            valueInfo
        );
        Assert.AreEqual(TestFlaggableEnum.None, defaultInfo.Value);
        Assert.AreEqual(TestFlaggableEnum.B, valueInfo.Value);
    }

    [TestMethod]
    public void EnumInfoT_StaticProperties_ForUnderlyingTypes()
    {
        Assert.AreEqual(typeof(TestEnum_Short), EnumInfo<TestEnum_Short>.StaticEnumType);
        Assert.AreEqual(typeof(short), EnumInfo<TestEnum_Short>.StaticUnderlyingType);
        Assert.IsFalse(EnumInfo<TestEnum_Short>.StaticIsFlaggable);

        Assert.AreEqual(typeof(TestEnum_Long), EnumInfo<TestEnum_Long>.StaticEnumType);
        Assert.AreEqual(typeof(long), EnumInfo<TestEnum_Long>.StaticUnderlyingType);
        Assert.IsFalse(EnumInfo<TestEnum_Long>.StaticIsFlaggable);
    }

    [TestMethod]
    public void EnumInfoT_StaticDefaultProperties_CanOverrideAndFallback()
    {
        var globalType = typeof(EnumInfo);
        var enumInfoType = typeof(EnumInfo<TestEnum>);
        var info = EnumInfo<TestEnum>.GetInfo(TestEnum.A);

        var originalGlobalDefaultToString = GetPrivateStaticField(globalType, "_defaultToString");
        var originalGlobalDefaultGetDisplayName = GetPrivateStaticField(
            globalType,
            "_defaultGetDisplayName"
        );
        var originalGlobalDefaultGetDisplayShortName = GetPrivateStaticField(
            globalType,
            "_defaultGetDisplayShortName"
        );
        var originalGlobalDefaultGetDisplayDescription = GetPrivateStaticField(
            globalType,
            "_defaultGetDisplayDescription"
        );

        var originalTypeDefaultToString = GetPrivateStaticField(enumInfoType, "_defaultToString");
        var originalTypeDefaultGetDisplayName = GetPrivateStaticField(
            enumInfoType,
            "_defaultGetDisplayName"
        );
        var originalTypeDefaultGetDisplayShortName = GetPrivateStaticField(
            enumInfoType,
            "_defaultGetDisplayShortName"
        );
        var originalTypeDefaultGetDisplayDescription = GetPrivateStaticField(
            enumInfoType,
            "_defaultGetDisplayDescription"
        );

        try
        {
            EnumInfo.DefaultToString = static _ => "Global_ToString";
            EnumInfo.DefaultGetDisplayName = static _ => "Global_Name";
            EnumInfo.DefaultGetDisplayShortName = static _ => "Global_ShortName";
            EnumInfo.DefaultGetDisplayDescription = static _ => "Global_Description";

            Assert.AreEqual("Global_ToString", EnumInfo.DefaultToString(info));
            Assert.AreEqual("Global_Name", EnumInfo.DefaultGetDisplayName(info));
            Assert.AreEqual("Global_ShortName", EnumInfo.DefaultGetDisplayShortName(info));
            Assert.AreEqual("Global_Description", EnumInfo.DefaultGetDisplayDescription(info));

            Assert.AreEqual("Global_ToString", EnumInfo<TestEnum>.DefaultToString(info));
            Assert.AreEqual("Global_Name", EnumInfo<TestEnum>.DefaultGetDisplayName(info));
            Assert.AreEqual(
                "Global_ShortName",
                EnumInfo<TestEnum>.DefaultGetDisplayShortName(info)
            );
            Assert.AreEqual(
                "Global_Description",
                EnumInfo<TestEnum>.DefaultGetDisplayDescription(info)
            );

            EnumInfo<TestEnum>.DefaultToString = static _ => "Type_ToString";
            EnumInfo<TestEnum>.DefaultGetDisplayName = static _ => "Type_Name";
            EnumInfo<TestEnum>.DefaultGetDisplayShortName = static _ => "Type_ShortName";
            EnumInfo<TestEnum>.DefaultGetDisplayDescription = static _ => "Type_Description";

            Assert.AreEqual("Type_ToString", EnumInfo<TestEnum>.DefaultToString(info));
            Assert.AreEqual("Type_Name", EnumInfo<TestEnum>.DefaultGetDisplayName(info));
            Assert.AreEqual("Type_ShortName", EnumInfo<TestEnum>.DefaultGetDisplayShortName(info));
            Assert.AreEqual(
                "Type_Description",
                EnumInfo<TestEnum>.DefaultGetDisplayDescription(info)
            );
        }
        finally
        {
            SetPrivateStaticField(globalType, "_defaultToString", originalGlobalDefaultToString);
            SetPrivateStaticField(
                globalType,
                "_defaultGetDisplayName",
                originalGlobalDefaultGetDisplayName
            );
            SetPrivateStaticField(
                globalType,
                "_defaultGetDisplayShortName",
                originalGlobalDefaultGetDisplayShortName
            );
            SetPrivateStaticField(
                globalType,
                "_defaultGetDisplayDescription",
                originalGlobalDefaultGetDisplayDescription
            );

            SetPrivateStaticField(enumInfoType, "_defaultToString", originalTypeDefaultToString);
            SetPrivateStaticField(
                enumInfoType,
                "_defaultGetDisplayName",
                originalTypeDefaultGetDisplayName
            );
            SetPrivateStaticField(
                enumInfoType,
                "_defaultGetDisplayShortName",
                originalTypeDefaultGetDisplayShortName
            );
            SetPrivateStaticField(
                enumInfoType,
                "_defaultGetDisplayDescription",
                originalTypeDefaultGetDisplayDescription
            );
        }
    }

    [TestMethod]
    public void EnumInfo_StaticProperties_AndMethods()
    {
        EnumInfo.InfosByType.TryGetValue(typeof(TestEnum), out var originalTestEnumInfos);
        EnumInfo.InfosByType.TryGetValue(
            typeof(TestFlaggableEnum),
            out var originalTestFlaggableEnumInfos
        );

        try
        {
            EnumInfo.InfosByType.TryRemove(typeof(TestEnum), out _);
            EnumInfo.InfosByType.TryRemove(typeof(TestFlaggableEnum), out _);

            Assert.IsFalse(EnumInfo.InfosByType.ContainsKey(typeof(TestEnum)));
            Assert.IsFalse(EnumInfo.InfosByType.ContainsKey(typeof(TestFlaggableEnum)));

            var testEnumInfo = TestEnum.C.GetInfo();
            var flagInfo = (TestFlaggableEnum.A | TestFlaggableEnum.C).GetInfo();

            Assert.AreEqual(TestEnum.C, testEnumInfo.Value);
            Assert.AreEqual(TestFlaggableEnum.A | TestFlaggableEnum.C, flagInfo.Value);

            Assert.IsTrue(EnumInfo.InfosByType.ContainsKey(typeof(TestEnum)));
            Assert.IsTrue(EnumInfo.InfosByType.ContainsKey(typeof(TestFlaggableEnum)));
            Assert.AreSame(
                EnumInfo<TestEnum>.StaticInfoDictionary.Untyped,
                EnumInfo.InfosByType[typeof(TestEnum)]
            );
            Assert.AreSame(
                EnumInfo<TestFlaggableEnum>.StaticInfoDictionary.Untyped,
                EnumInfo.InfosByType[typeof(TestFlaggableEnum)]
            );

            var normalInfo = EnumInfo<TestEnum>.GetInfo(TestEnum.B);
            var noneFlagInfo = EnumInfo<TestFlaggableEnum>.GetInfo();
            var compositeFlagInfo = EnumInfo<TestFlaggableEnum>.GetInfo(
                TestFlaggableEnum.A | TestFlaggableEnum.C
            );

            Assert.AreEqual("B", EnumInfo.GlobalDefaultToString(normalInfo));
            Assert.AreEqual("B_Name", EnumInfo.GlobalDefaultGetDisplayName(normalInfo));
            Assert.AreEqual("B_ShortName", EnumInfo.GlobalDefaultGetDisplayShortName(normalInfo));
            Assert.AreEqual(
                "B_Description",
                EnumInfo.GlobalDefaultGetDisplayDescription(normalInfo)
            );

            Assert.AreEqual("None_Name", EnumInfo.GlobalDefaultGetDisplayName(noneFlagInfo));
            Assert.AreEqual(
                "None_ShortName",
                EnumInfo.GlobalDefaultGetDisplayShortName(noneFlagInfo)
            );
            Assert.AreEqual(
                "None_Description",
                EnumInfo.GlobalDefaultGetDisplayDescription(noneFlagInfo)
            );

            Assert.AreEqual("A, C", EnumInfo.GlobalDefaultToString(compositeFlagInfo));
            Assert.AreEqual(
                "A_Name, C_Name",
                EnumInfo.GlobalDefaultGetDisplayName(compositeFlagInfo)
            );
            Assert.AreEqual(
                "A_ShortName, C_ShortName",
                EnumInfo.GlobalDefaultGetDisplayShortName(compositeFlagInfo)
            );
            Assert.AreEqual(
                "A_Description, C_Description",
                EnumInfo.GlobalDefaultGetDisplayDescription(compositeFlagInfo)
            );

            var method = typeof(EnumInfo).GetMethod(
                "CreateEnumInfoExpression",
                BindingFlags.NonPublic | BindingFlags.Static
            );
            Assert.IsNotNull(method);

            var factory =
                method.Invoke(null, new object[] { typeof(TestEnum) }) as Func<Enum, IEnumInfo>;
            Assert.IsNotNull(factory);

            var createdInfo = factory.Invoke(TestEnum.A);

            Assert.AreEqual(TestEnum.A, createdInfo.Value);
            Assert.AreEqual("A_Name", createdInfo.DisplayName);
            Assert.AreEqual("A_ShortName", createdInfo.DisplayShortName);
            Assert.AreEqual("A_Description", createdInfo.DisplayDescription);
            Assert.AreEqual(typeof(TestEnum), createdInfo.EnumType);
        }
        finally
        {
            RestoreInfosByType(typeof(TestEnum), originalTestEnumInfos);
            RestoreInfosByType(typeof(TestFlaggableEnum), originalTestFlaggableEnumInfos);
        }
    }

    private static void AssertSequenceEqual<T>(IEnumerable<T> actual, params T[] expected)
    {
        CollectionAssert.AreEqual(expected, actual.ToArray());
    }

    private static object? GetPrivateStaticField(Type type, string fieldName)
    {
        return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?.GetValue(null);
    }

    private static void SetPrivateStaticField(Type type, string fieldName, object? value)
    {
        type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?.SetValue(null, value);
    }

    private static void RestoreInfosByType(Type enumType, IDictionary<Enum, IEnumInfo>? dictionary)
    {
        if (dictionary is null)
        {
            EnumInfo.InfosByType.TryRemove(enumType, out _);
            return;
        }
        EnumInfo.InfosByType[enumType] = dictionary;
    }
}

internal enum TestEnum
{
    [Display(Name = "None_Name", ShortName = "None_ShortName", Description = "None_Description")]
    None,

    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 2,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 3,
}

[Flags]
internal enum TestFlaggableEnum
{
    [Display(Name = "None_Name", ShortName = "None_ShortName", Description = "None_Description")]
    None,

    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1 << 0,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 1 << 1,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 1 << 2,
}

internal enum TestEnum_Short : short
{
    None,
    A = 1,
    B = 2,
    C = 3,
}

internal enum TestEnum_Long : long
{
    None,
    A = 1,
    B = 2,
    C = 3,
}
