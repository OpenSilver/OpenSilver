
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests;
#else
namespace Windows.UI.Xaml.Tests;
#endif

public partial class DependencyPropertyTest
{
    [TestMethod]
    public void OverrideMetadata_Should_Throw_When_ForType_Is_Null()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(OverrideMetadata_Should_Throw_When_ForType_Is_Null),
            typeof(string),
            typeof(MyDependencyObject1),
            null);

        Assert.ThrowsException<ArgumentNullException>(
            () => dp.OverrideMetadata(null, new PropertyMetadata("a")));
    }

    [TestMethod]
    public void OverrideMetadata_Should_Throw_When_TypeMetadata_Is_Null()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(OverrideMetadata_Should_Throw_When_TypeMetadata_Is_Null),
            typeof(string),
            typeof(MyDependencyObject1),
            null);

        Assert.ThrowsException<ArgumentNullException>(
            () => dp.OverrideMetadata(typeof(MyDependencyObject2), null));
    }

    [TestMethod]
    public void OverrideMetadata_Should_Throw_When_ForType_Is_Not_DependencyObject()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(OverrideMetadata_Should_Throw_When_ForType_Is_Not_DependencyObject),
            typeof(string),
            typeof(MyDependencyObject1),
            null);

        Assert.ThrowsException<ArgumentException>(
            () => dp.OverrideMetadata(typeof(string), new PropertyMetadata("b")));
    }

    [TestMethod]
    public void OverrideMetadata_Should_Throw_When_Same_ForType_Twice()
    {
        Assert.ThrowsException<ArgumentException>(
            () => MyDependencyObject1.Property1.OverrideMetadata(typeof(MyDependencyObject1), new PropertyMetadata(100.0)));

        Assert.ThrowsException<ArgumentException>(
            () => MyDependencyObject1.Property1.OverrideMetadata(typeof(MyDependencyObject2), new PropertyMetadata(100.0)));
    }

    [TestMethod]
    public void OverrideMetadata_Should_Throw_When_Metadata_Is_Not_Derived_From_BaseMetadata()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(OverrideMetadata_Should_Throw_When_Metadata_Is_Not_Derived_From_BaseMetadata),
            typeof(string),
            typeof(MyDependencyObject1),
            new MyPropertyMetadata(string.Empty));

        Assert.ThrowsException<ArgumentException>(
            () => dp.OverrideMetadata(typeof(MyDependencyObject2), new PropertyMetadata(string.Empty)));
    }

    [TestMethod]
    public void OverrideMetadata_Should_Call_BaseType_Callback_First()
    {
        var o = new MyDependencyObject2();
        o.Property1Changed1 += OnProperty1Changed;

        o.SetValue(MyDependencyObject1.Property1, 100.0);

        Assert.AreEqual(o.Property1ChangedCounter1, 1);
        Assert.AreEqual(o.Property1ChangedCounter2, 1);

        static void OnProperty1Changed(object sender, EventArgs e)
        {
            var o = (MyDependencyObject2)sender;
            Assert.AreEqual(o.Property1ChangedCounter1, 1);
            Assert.AreEqual(o.Property1ChangedCounter2, 0);
        }
    }

    [TestMethod]
    public void OverrideMetadata_DefaultValue()
    {
        var o1 = new MyDependencyObject1();
        var o2 = new MyDependencyObject2();

        Assert.AreEqual(42.0, (double)MyDependencyObject1.Property1.GetMetadata(typeof(MyDependencyObject1)).DefaultValue);
        Assert.AreEqual(69.0, (double)MyDependencyObject1.Property1.GetMetadata(typeof(MyDependencyObject2)).DefaultValue);

        Assert.AreEqual(42.0, (double)o1.GetValue(MyDependencyObject1.Property1));
        Assert.AreEqual(69.0, (double)o2.GetValue(MyDependencyObject1.Property1));
    }

    [TestMethod]
    public void DependencyProperty_OverrideMetadata_Should_Throw_For_ReadOnly_Property()
    {
        DependencyPropertyKey key = DependencyProperty.RegisterReadOnly(
            nameof(DependencyProperty_OverrideMetadata_Should_Throw_For_ReadOnly_Property),
            typeof(string),
            typeof(MyDependencyObject1),
            null);

        DependencyProperty dp = key.DependencyProperty;

        Assert.ThrowsException<InvalidOperationException>(
            () => dp.OverrideMetadata(typeof(MyDependencyObject2), new PropertyMetadata("test")));
    }

    [TestMethod]
    public void DependencyPropertyKey_OverrideMetadata()
    {
        DependencyPropertyKey key = DependencyProperty.RegisterReadOnly(
            nameof(DependencyPropertyKey_OverrideMetadata),
            typeof(string),
            typeof(MyDependencyObject1),
            new PropertyMetadata(""));

        DependencyProperty dp = key.DependencyProperty;
        
        key.OverrideMetadata(typeof(MyDependencyObject2), new PropertyMetadata(nameof(DependencyPropertyKey_OverrideMetadata), OnChanged));

        bool propertyChanged = false;

        var o = new MyDependencyObject2();

        Assert.AreEqual((string)o.GetValue(dp), nameof(DependencyPropertyKey_OverrideMetadata));

        o.SetValue(key, "new value");

        Assert.IsTrue(propertyChanged);

        void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            propertyChanged = true;
        }
    }
}

file class MyDependencyObject2 : DependencyPropertyTest.MyDependencyObject1
{
    public int Property1ChangedCounter2 { get; private set; }

    public event EventHandler Property1Changed2;

    static MyDependencyObject2()
    {
        Property1.OverrideMetadata(typeof(MyDependencyObject2), new PropertyMetadata(69.0, OnProperty1Changed));
    }

    private static void OnProperty1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var o = (MyDependencyObject2)d;
        o.Property1ChangedCounter2++;
        o.Property1Changed2?.Invoke(o, EventArgs.Empty);
    }
}

file class MyPropertyMetadata : PropertyMetadata
{
    public MyPropertyMetadata(object defaultValue)
        : base(defaultValue)
    {
    }
}
