using System;
using System.Collections.Generic;
using System.Text;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

#pragma warning disable S1199
[TestClass]
public sealed class BidirectionalDictionaryTests : BidirectionalDictionaryTestsBase
{
    protected override IBidirectionalDictionary<int, string> CreateDictionary() =>
        new BidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString())
        );
}

[TestClass]
public sealed class BidirectionalDictionaryWrapperTests : BidirectionalDictionaryTestsBase
{
    protected override IBidirectionalDictionary<int, string> CreateDictionary() =>
        new BidirectionalDictionaryWrapper<
            int,
            string,
            Dictionary<int, string>,
            Dictionary<string, int>
        >(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString()),
            Enumerable.Range(1, 10).ToDictionary(i => i.ToString(), i => i),
            null,
            null
        );
}

[TestClass]
public sealed class ConcurrentBidirectionalDictionaryTests : BidirectionalDictionaryTestsBase
{
    protected override IBidirectionalDictionary<int, string> CreateDictionary() =>
        new ConcurrentBidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString())
        );
}

public abstract class BidirectionalDictionaryTestsBase
{
    protected abstract IBidirectionalDictionary<int, string> CreateDictionary();
    readonly IReadOnlyCollection<KeyValuePair<int, string>> _newItems = Enumerable
        .Range(100, 10)
        .Select(i => KeyValuePair.Create(i, i.ToString()))
        .ToArray();

    [TestMethod]
    public void IDictionaryTest()
    {
        IReadOnlyDictionaryTTestUtils.ItemGet(CreateDictionary, _newItems);
        IDictionaryTTestUtils.Remove(CreateDictionary);
        IDictionaryTTestUtils.Clear(CreateDictionary);
        IReadOnlyDictionaryTTestUtils.Keys(CreateDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.Values(CreateDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.TryGetValue(CreateDictionary, _newItems);
    }

    [TestMethod]
    public void Test()
    {
        var getDictionary = () =>
            new BidirectionalDictionary<int, string>(
                Enumerable.Range(1, 10).Select(i => KeyValuePair.Create(i, i.ToString()))
            );

        IDictionaryTTestUtils.Remove<int, string>(getDictionary);
        IDictionaryTTestUtils.Clear<int, string>(getDictionary);
    }

    [TestMethod]
    public void TrySetValue()
    {
        {
            var dictionary = CreateDictionary();
            Assert.Throws<UseAlternativeMethodException>(() => dictionary[1] = "1");
        }

        {
            // 1
            var dictionary = CreateDictionary();
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            // 1
            var dictionary = CreateDictionary();
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            // 2
            var dictionary = CreateDictionary();
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
        }

        {
            // 3
            var dictionary = CreateDictionary();
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsFalse(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.IsTrue(dictionary.SequenceEqual(CreateDictionary()));
        }

        {
            // 4
            var dictionary = CreateDictionary();
            var pair = dictionary.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.IsTrue(dictionary.SequenceEqual(CreateDictionary()));
        }
        {
            // 4
            var dictionary = CreateDictionary();
            var pair = dictionary.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.IsTrue(dictionary.SequenceEqual(CreateDictionary()));
        }
    }

    [TestMethod]
    public void Fails()
    {
        var dictionary = CreateDictionary();
        var pair = KeyValuePair.Create(1, "1");
        Assert.Throws<UseAlternativeMethodException>(() => dictionary[pair.Key] = pair.Value);
        Assert.Throws<UseAlternativeMethodException>(() => dictionary[pair.Value] = pair.Key);

        Assert.Throws<UseAlternativeMethodException>(() =>
            dictionary.As<IDictionary<int, string>>().Add(pair.Key, pair.Value)
        );

        Assert.Throws<UseAlternativeMethodException>(() =>
            dictionary
                .As<ICollection<KeyValuePair<int, string>>>()
                .Add(KeyValuePair.Create(pair.Key, pair.Value))
        );
    }

    [TestMethod]
    public void TryAdd()
    {
        {
            var dictionary = CreateDictionary();
            // 重复的键值对, 失败
            var pair = dictionary.First();
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = CreateDictionary();
            // 同Key但Value不一样, 失败
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = CreateDictionary();
            // 同Value但Key不一样. 失败
            var pair = KeyValuePair.Create(_newItems.First().Key, dictionary.First().Value);
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = CreateDictionary();
            // 不重复的键值对, 成功
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            var dictionary = CreateDictionary();
            // 不重复的键值对, 成功
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TryAdd(pair.Value, pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }
    }
}

[TestClass]
public sealed class FrozenBidirectionalDictionaryTests
{
    private FrozenBidirectionalDictionary<int, string> CreateDictionary() =>
        new FrozenBidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString())
        );

    readonly IReadOnlyCollection<KeyValuePair<int, string>> _newItems = Enumerable
        .Range(100, 10)
        .Select(i => KeyValuePair.Create(i, i.ToString()))
        .ToArray();

    [TestMethod]
    public void IDictionaryTest()
    {
        IReadOnlyDictionaryTTestUtils.ItemGet(CreateDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.Keys(CreateDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.Values(CreateDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.TryGetValue(CreateDictionary, _newItems);
    }
}

#pragma warning restore S1199
