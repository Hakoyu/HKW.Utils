using System;
using System.Collections.Generic;
using System.Text;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

#pragma warning disable S1199
[TestClass]
public class BidirectionalDictionaryWrapperTests
{
    readonly Func<
        BidirectionalDictionaryWrapper<
            int,
            string,
            Dictionary<int, string>,
            Dictionary<string, int>
        >
    > _createDictionary = () =>
        new(
            Enumerable.Range(1, 10).ToDictionary(x => x, x => x.ToString()),
            Enumerable.Range(1, 10).ToDictionary(x => x.ToString(), x => x),
            null,
            null
        );

    readonly IReadOnlyCollection<KeyValuePair<int, string>> _newItems = Enumerable
        .Range(100, 10)
        .Select(i => KeyValuePair.Create(i, i.ToString()))
        .ToArray();

    [TestMethod]
    public void IDictionaryTest()
    {
        IReadOnlyDictionaryTTestUtils.ItemGet(_createDictionary, _newItems);
        IDictionaryTTestUtils.Remove(_createDictionary);
        IDictionaryTTestUtils.Clear(_createDictionary);
        IReadOnlyDictionaryTTestUtils.Keys(_createDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.Values(_createDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.TryGetValue(_createDictionary, _newItems);
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
            var dictionary = _createDictionary();
            Assert.Throws<UseAlternativeMethodException>(() => dictionary[1] = "1");
        }

        {
            // 1
            var dictionary = _createDictionary();
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            // 1
            var dictionary = _createDictionary();
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            // 2
            var dictionary = _createDictionary();
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
        }

        {
            // 3
            var dictionary = _createDictionary();
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsFalse(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.IsTrue(dictionary.SequenceEqual(_createDictionary()));
        }

        {
            // 4
            var dictionary = _createDictionary();
            var pair = dictionary.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Key, pair.Value));
            Assert.IsTrue(dictionary.SequenceEqual(_createDictionary()));
        }
        {
            // 4
            var dictionary = _createDictionary();
            var pair = dictionary.First();
            Assert.IsTrue(dictionary.TrySetValue(pair.Value, pair.Key));
            Assert.IsTrue(dictionary.SequenceEqual(_createDictionary()));
        }
    }

    [TestMethod]
    public void Fails()
    {
        var dictionary = _createDictionary();
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
            var dictionary = _createDictionary();
            // 重复的键值对, 失败
            var pair = dictionary.First();
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = _createDictionary();
            // 同Key但Value不一样, 失败
            var pair = KeyValuePair.Create(dictionary.First().Key, _newItems.First().Value);
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = _createDictionary();
            // 同Value但Key不一样. 失败
            var pair = KeyValuePair.Create(_newItems.First().Key, dictionary.First().Value);
            Assert.IsFalse(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.IsFalse(dictionary.TryAdd(pair.Value, pair.Key));
        }

        {
            var dictionary = _createDictionary();
            // 不重复的键值对, 成功
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TryAdd(pair.Key, pair.Value));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }

        {
            var dictionary = _createDictionary();
            // 不重复的键值对, 成功
            var pair = _newItems.First();
            Assert.IsTrue(dictionary.TryAdd(pair.Value, pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(dictionary[pair.Value], pair.Key);
        }
    }
}
#pragma warning restore S1199
