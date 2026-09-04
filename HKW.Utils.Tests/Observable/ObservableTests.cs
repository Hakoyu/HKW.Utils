using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableTests
{
    [TestMethod]
    public void ObservableValue()
    {
        var value = new ObservableValue<int>();
        var changing = false;
        var changed = false;
        value.PropertyChanging += (s, e) =>
        {
            changing = true;
        };
        value.PropertyChanged += (s, e) =>
        {
            changed = true;
        };
        value.Value = 666;
        Assert.IsTrue(changing);
        Assert.IsTrue(changed);
    }

    //[TestMethod]
    //public void ObservableWrapper()
    //{
    //    var vm = new TestViewModel();
    //    var wrapper = new ObservableWrapper<TestViewModel, string>(
    //        vm,
    //        vm => vm.Value,
    //        (vm, r) => vm.Value = r
    //    );
    //    Assert.AreEqual(vm.Value, wrapper.Value);
    //    vm.Value = "999";
    //    Assert.AreEqual(vm.Value, wrapper.Value);
    //}
}
