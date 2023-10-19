
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace OpenSilver.Internal.Tests;

public partial class PFCDefaultValueFactoryTest
{
    [TestMethod]
    public void DefaultValue_Should_Be_StringCollection()
    {
        var o = new MyDependencyObject();
        var value = o.GetValue(MyDependencyObject.MyProperty1);

        Assert.IsInstanceOfType(value, typeof(StringCollection));
    }

    [TestMethod]
    public void Factory_Should_Not_Call_DependencyProperty_Callback()
    {
        var o = new MyDependencyObject();
        o.GetValue(MyDependencyObject.MyProperty1);

        Assert.AreEqual(o.MyProperty1ChangedCounter, 0);
    }

    [TestMethod]
    public void Factory_Should_Cache_And_Reuse_Value()
    {
        var o = new MyDependencyObject();
        var value1 = o.GetValue(MyDependencyObject.MyProperty1);
        var value2 = o.GetValue(MyDependencyObject.MyProperty1);

        Assert.AreSame(value1, value2);
    }

    [TestMethod]
    public void Factory_Should_Reuse_Value_After_Promotion_To_Local_Value()
    {
        var o = new MyDependencyObject();

        var value1 = o.GetValue(MyDependencyObject.MyProperty1);

        o.SetValue(MyDependencyObject.MyProperty1, new StringCollection { "test" });
        o.ClearValue(MyDependencyObject.MyProperty1);

        var value2 = o.GetValue(MyDependencyObject.MyProperty1);

        Assert.AreSame(value1, value2);
    }
}

file class MyDependencyObject : DependencyObject
{
    public int MyProperty1ChangedCounter { get; private set; }

    public static readonly DependencyProperty MyProperty1 =
        DependencyProperty.Register(
            nameof(MyProperty1),
            typeof(StringCollection),
            typeof(MyDependencyObject),
            new PropertyMetadata(
                new PFCDefaultValueFactory<string>(
                    static () => new StringCollection(),
                    static (d, dp) => new StringCollection()),
                OnMyProperty1Changed));

    private static void OnMyProperty1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var o = (MyDependencyObject)d;
        o.MyProperty1ChangedCounter++;
    }
}

file class StringCollection : PresentationFrameworkCollection<string>
{
    public StringCollection()
        : base(false)
    {
    }

    internal override void AddOverride(string value) => AddInternal(value);
    internal override void ClearOverride() => ClearInternal();
    internal override string GetItemOverride(int index) => GetItemInternal(index);
    internal override void InsertOverride(int index, string value) => InsertInternal(index, value);
    internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);
    internal override void SetItemOverride(int index, string value) => SetItemInternal(index, value);
}
