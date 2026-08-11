using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

#pragma warning disable S6608
[TestClass]
public class ObservableI18nResourceTests
{
    public static CultureInfo[] Cultures { get; } =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures);
    public static CultureInfo CurrentCulture { get; } = CultureInfo.CurrentCulture;

    public static Func<ObservableI18nResource<string, string>> GetI18nResource =>
        () =>
        {
            var i18nResource = new ObservableI18nResource<string, string>(
                "Main",
                Cultures,
                Cultures.First()
            );
            foreach (var c in i18nResource.Cultures)
                i18nResource.SetDatas(
                    Enumerable
                        .Range(0, 10)
                        .Select(x => KeyValuePair.Create(x.ToString(), GetValue(x.ToString(), c))),
                    c
                );

            return i18nResource;
        };
    public static string[] Keys { get; } = Enumerable.StringRange(0, 10).ToArray();

    public static string GetValue(string key, CultureInfo culture) => $"{key}_{culture.Name}";

    private static CultureInfo[] GetTestCultures(int count)
    {
        var cultures = Cultures.Take(count).ToArray();
        if (cultures.Length < count)
            throw new InvalidOperationException("Not enough cultures for tests.");
        return cultures;
    }

    [TestMethod]
    public void ChangeCurrentCulture()
    {
        var resource = GetI18nResource();
        foreach (var culture in resource.Cultures)
        {
            resource.CurrentCulture = culture;
            foreach (var key in Keys)
            {
                Assert.AreEqual(
                    GetValue(key, culture),
                    [resource.GetData(key), resource.GetData(key, culture)]
                );
            }
        }
    }

    [TestMethod]
    public void SetData()
    {
        var resource = new ObservableI18nResource<string, string>(
            "Main",
            Cultures,
            Cultures.First()
        );
        foreach (var (c, i) in resource.Cultures.WithIndex())
        {
            Assert.IsTrue(resource.SetData(i.ToString(), GetValue(i.ToString(), c), c));
            Parallel.For(
                0,
                resource.Cultures.Count,
                j =>
                {
                    var key = j.ToString();
                    if (j == i)
                    {
                        // 同Key的值相等
                        Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                    }
                    else if (j < i)
                    {
                        // 小于i代表此key被添加过, 但是默认值
                        Assert.AreEqual(
                            resource.GetData(key, c),
                            resource.GetDefaultValue("", null!)
                        );
                    }
                    else
                    {
                        // 未被添加的key抛出异常
                        Assert.Throws<KeyNotFoundException>(() => resource.GetData(key, c));
                    }
                }
            );
        }
    }

    [TestMethod]
    public void SetDataWhenDefault()
    {
        var resource = new ObservableI18nResource<string, string>(
            "Main",
            Cultures,
            Cultures.First()
        );
        foreach (var (c, i) in resource.Cultures.WithIndex())
        {
            Assert.IsTrue(resource.SetData(i.ToString(), GetValue(i.ToString(), c), c));
            for (var j = 0; j < resource.Cultures.Count; j++)
            {
                var key = j.ToString();
                if (j == i)
                {
                    // 同Key的值不是Default, 所以失败
                    Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                    Assert.IsFalse(
                        resource.SetDataWhenDefault(
                            key,
                            GetValue(key, c) + "_SetDataWhenDefault",
                            c
                        )
                    );
                    Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                }
                else if (j < i)
                {
                    // 小于i代表此key被添加过, 但是默认值, 所以添加成功
                    Assert.AreEqual(resource.GetData(key, c), resource.GetDefaultValue("", null!));
                    Assert.IsTrue(
                        resource.SetDataWhenDefault(
                            key,
                            GetValue(key, c) + "_SetDataWhenDefault",
                            c
                        )
                    );
                    Assert.AreEqual(
                        resource.GetData(key, c),
                        GetValue(key, c) + "_SetDataWhenDefault"
                    );
                }
            }
        }
    }

    [TestMethod]
    public void RemoveData()
    {
        var resource = GetI18nResource();
        var dic = resource.DatasByKey.ToDictionary();
        var count = dic.Count;
        for (var i = 0; i < count; i++)
        {
            var pair = dic.Last();
            Assert.IsTrue(resource.RemoveData(pair.Key));
            dic.Remove(pair.Key);
            Assert.IsTrue(resource.DatasByKey.SequenceEqual(dic));
        }
        Assert.IsEmpty(resource.DatasByKey);
        Assert.IsTrue(resource.DatasByKey.SequenceEqual(dic));
    }

    [TestMethod]
    public void GetDataOrDefault_WhenMissingKeyOrCulture_ReturnsFallback()
    {
        var cultures = GetTestCultures(3);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var c3 = cultures[2];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1);
        resource.GetDefaultValue = (key, culture) => $"fallback_{key}_{culture.Name}";
        resource.SetData("k1", "v1", c1);

        Assert.AreEqual("v1", resource.GetDataOrDefault("k1", c1));
        Assert.AreEqual("fallback_k2_" + c1.Name, resource.GetDataOrDefault("k2", c1));
        Assert.AreEqual("fallback_k1_" + c3.Name, resource.GetDataOrDefault("k1", c3));
        Assert.AreEqual("custom", resource.GetDataOrDefault("k3", c1, static (_, _) => "custom"));
    }

    [TestMethod]
    public void SetDatasWhenDefault_OnlyOverwriteDefault()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1)
        {
            DefaultValue = "__default__",
        };

        resource.SetData("k1", "v1", c1);
        resource.SetDatasWhenDefault(
            [KeyValuePair.Create("k1", "override"), KeyValuePair.Create("k2", "v2")],
            c1
        );

        Assert.AreEqual("v1", resource.GetData("k1", c1));
        Assert.AreEqual("v2", resource.GetData("k2", c1));
        Assert.AreEqual("__default__", resource.GetData("k2", c2));
    }

    [TestMethod]
    public void AddCulture_ShouldAppendDefaultValueForExistingKeys()
    {
        var cultures = GetTestCultures(3);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var c3 = cultures[2];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1)
        {
            DefaultValue = "__default__",
        };

        resource.SetData("k1", "v1", c1);
        Assert.IsTrue(resource.AddCulture(c3));
        Assert.AreEqual("__default__", resource.GetData("k1", c3));
        Assert.IsFalse(resource.AddCulture(c3));
    }

    [TestMethod]
    public void RemoveCulture_ShouldReindexAndKeepData()
    {
        var cultures = GetTestCultures(3);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var c3 = cultures[2];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2, c3], c1);

        resource.SetData("k1", "v1", c1);
        resource.SetData("k1", "v2", c2);
        resource.SetData("k1", "v3", c3);

        Assert.IsTrue(resource.RemoveCulture(c2));
        Assert.AreEqual("v1", resource.GetData("k1", c1));
        Assert.AreEqual("v3", resource.GetData("k1", c3));
        Assert.DoesNotContain(c2, resource.Cultures);
    }

    [TestMethod]
    public void RemoveCulture_WhenRemovingCurrentCulture_ShouldThrow()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1);

        Assert.Throws<ArgumentException>(() => resource.RemoveCulture(c1));
    }

    [TestMethod]
    public void RenameKey_ShouldMoveDataAndKeepValues()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1);

        resource.SetData("old", "v1", c1);
        resource.SetData("old", "v2", c2);

        Assert.IsTrue(resource.RenameKey("old", "new"));
        Assert.AreEqual("v1", resource.GetData("new", c1));
        Assert.AreEqual("v2", resource.GetData("new", c2));
        Assert.Throws<KeyNotFoundException>(() => resource.GetData("old", c1));
        Assert.IsFalse(resource.RenameKey("missing", "new2"));

        resource.SetData("exists", "value", c1);
        Assert.IsFalse(resource.RenameKey("new", "exists"));
    }

    [TestMethod]
    public void CurrentCulture_ShouldRaiseEventsAndRefreshIndexers()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1);

        resource.SetData("k1", "v1", c1);
        resource.SetData("k1", "v2", c2);

        var currentCultureChangedCount = 0;
        var currentCulturePropertyChangedCount = 0;
        var getDataCoreRefreshCount = 0;
        var getDataOrDefaultCoreRefreshCount = 0;

        resource.CurrentCultureChanged += (_, culture) =>
        {
            if (culture.Equals(c2))
                currentCultureChangedCount++;
        };
        resource.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(resource.CurrentCulture))
                currentCulturePropertyChangedCount++;
        };
        resource.GetCurrentCultureData.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "")
                getDataCoreRefreshCount++;
        };
        resource.GetCurrentCultureDataOrDefault.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "")
                getDataOrDefaultCoreRefreshCount++;
        };

        resource.CurrentCulture = c2;
        resource.CurrentCulture = c2;

        Assert.AreEqual(1, currentCultureChangedCount);
        Assert.AreEqual(1, currentCulturePropertyChangedCount);
        Assert.AreEqual(1, getDataCoreRefreshCount);
        Assert.AreEqual(1, getDataOrDefaultCoreRefreshCount);
        Assert.AreEqual("v2", resource.GetCurrentCultureData["k1"]);
    }

    [TestMethod]
    public void SetData_ShouldRaiseCultureDataChanged()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", [c1, c2], c1)
        {
            DefaultValue = "__default__",
        };

        CultureDataChangedEventArgs<string, string>? lastArgs = null;
        var changedCount = 0;

        resource.CultureDataChanged += (_, e) =>
        {
            lastArgs = e;
            changedCount++;
        };

        resource.SetData("k1", "v1", c1);
        Assert.AreEqual(1, changedCount);
        Assert.IsNotNull(lastArgs);
        Assert.AreEqual("k1", lastArgs.Key);
        Assert.AreEqual("__default__", lastArgs.OldValue);
        Assert.AreEqual("v1", lastArgs.NewValue);
        Assert.AreEqual(c1, lastArgs.CultureInfo);

        resource.SetData("k1", "v2", c1);
        Assert.AreEqual(2, changedCount);
        Assert.IsNotNull(lastArgs);
        Assert.AreEqual("v1", lastArgs.OldValue);
        Assert.AreEqual("v2", lastArgs.NewValue);
    }

    [TestMethod]
    public void ClearData_ShouldRemoveAllItems()
    {
        var resource = GetI18nResource();
        resource.ClearData();
        Assert.IsEmpty(resource.DatasByKey);
    }

    [TestMethod]
    public void RemoveData_WhenKeyNotExists_ShouldReturnFalse()
    {
        var resource = GetI18nResource();
        Assert.IsFalse(resource.RemoveData("__missing__"));
    }
}
#pragma warning restore S6608
