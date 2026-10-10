using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                Cultures.First(),
                Cultures
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
            Cultures.First(),
            Cultures
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
            Cultures.First(),
            Cultures
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
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2]);
        resource.GetDefaultValue = (key, culture) => $"fallback_{key}_{culture.Name}";
        resource.SetData("k1", "v1", c1);

        Assert.AreEqual("v1", resource.GetDataOrDefault("k1", c1));
        Assert.AreEqual("fallback_k2_" + c1.Name, resource.GetDataOrDefault("k2", c1));
        Assert.AreEqual("fallback_k1_" + c3.Name, resource.GetDataOrDefault("k1", c3));
        Assert.AreEqual("custom", resource.GetDataOrDefault("k3", "custom", c1));
    }

    [TestMethod]
    public void SetDatasWhenDefault_OnlyOverwriteDefault()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2])
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
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2])
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
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2, c3]);

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
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2]);

        Assert.Throws<ArgumentException>(() => resource.RemoveCulture(c1));
    }

    [TestMethod]
    public void RenameKey_ShouldMoveDataAndKeepValues()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2]);

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
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2]);

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
            if (e.IsIndexer())
                getDataCoreRefreshCount++;
        };
        resource.GetCurrentCultureDataOrDefault.PropertyChanged += (_, e) =>
        {
            if (e.IsIndexer())
                getDataOrDefaultCoreRefreshCount++;
        };

        resource.CurrentCulture = c2;
        resource.CurrentCulture = c2;

        Assert.AreEqual(1, currentCultureChangedCount);
        Assert.AreEqual(1, currentCulturePropertyChangedCount);
        Assert.AreEqual(2, getDataCoreRefreshCount);
        Assert.AreEqual(2, getDataOrDefaultCoreRefreshCount);
        Assert.AreEqual("v2", resource.GetCurrentCultureData["k1"]);
    }

    [TestMethod]
    public void SetData_ShouldRaiseCultureDataChanged()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2])
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

    [TestMethod]
    public void CulturesChanged_ShouldReportStandardCollectionChanges()
    {
        var cultures = GetTestCultures(4);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var c3 = cultures[2];
        var c4 = cultures[3];
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2]);
        var changes = new List<NotifyCollectionChangedEventArgs>();
        resource.ObservableCultures.CollectionChanged += (_, e) => changes.Add(e);

        Assert.IsTrue(resource.AddCulture(c3));
        Assert.AreEqual(NotifyCollectionChangedAction.Add, changes[^1].Action);
        Assert.AreEqual(c3, changes[^1].NewItems![0]);
        Assert.AreEqual(2, changes[^1].NewStartingIndex);

        resource.CurrentCulture = c2;
        Assert.IsTrue(resource.ReplaceCulture(c2, c4));
        Assert.AreEqual(NotifyCollectionChangedAction.Replace, changes[^1].Action);
        Assert.AreEqual(c2, changes[^1].OldItems![0]);
        Assert.AreEqual(c4, changes[^1].NewItems![0]);
        Assert.AreEqual(1, changes[^1].NewStartingIndex);
        Assert.AreSequenceEqual([c1, c4, c3], resource.Cultures);
        Assert.AreEqual(c4, resource.CurrentCulture);

        resource.CurrentCulture = c1;
        Assert.IsTrue(resource.RemoveCulture(c4));
        Assert.AreEqual(NotifyCollectionChangedAction.Remove, changes[^1].Action);
        Assert.AreEqual(c4, changes[^1].OldItems![0]);
        Assert.AreEqual(1, changes[^1].OldStartingIndex);
        Assert.AreSequenceEqual([c1, c3], resource.Cultures);
        Assert.AreEqual(1, resource.IndexByCulture[c3]);

        resource.SetData("key", "value", c1);
        resource.ClearCulture();
        Assert.AreEqual(NotifyCollectionChangedAction.Reset, changes[^1].Action);
    }

    [TestMethod]
    public void DatasByKey_Add_ShouldNormalizeAndObserveCultureDataList()
    {
        var cultures = GetTestCultures(2);
        var c1 = cultures[0];
        var c2 = cultures[1];
        var resource = new ObservableI18nResource<string, string>("Main", c1, [c1, c2])
        {
            DefaultValue = "__default__",
        };
        var data = new ObservableCultureDataList<string, string>("key") { "value" };
        CultureDataChangedEventArgs<string, string>? changed = null;
        resource.CultureDataChanged += (_, e) => changed = e;

        resource.DatasByKey.Add("key", data);

        Assert.HasCount(2, data);
        Assert.AreEqual("__default__", data[1]);
        data[1] = "changed";
        Assert.IsNotNull(changed);
        Assert.AreEqual(c2, changed.CultureInfo);
        Assert.AreEqual("changed", changed.NewValue);
    }

    [TestMethod]
    public void DatasByKey_Add_WhenCultureDataCountIsTooLarge_ShouldThrow()
    {
        var cultures = GetTestCultures(2);
        var resource = new ObservableI18nResource<string, string>("Main", cultures[0], cultures);
        var data = new ObservableCultureDataList<string, string>("key") { "1", "2", "3" };

        Assert.Throws<ArgumentException>(() => resource.DatasByKey.Add("key", data));
        Assert.IsFalse(resource.DatasByKey.ContainsKey("key"));
    }
}
#pragma warning restore S6608
