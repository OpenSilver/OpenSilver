
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterReadOnly_Should_Throw_When_Name_Is_Null()
    {
        DependencyProperty.RegisterReadOnly(
            null,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterReadOnly_Should_Throw_When_Name_Is_Empty()
    {
        DependencyProperty.RegisterReadOnly(
            string.Empty,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterReadOnly_Should_Throw_When_PropertyType_Is_Null()
    {
        DependencyProperty.RegisterReadOnly(
            nameof(RegisterReadOnly_Should_Throw_When_PropertyType_Is_Null),
            null,
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterReadOnly_Should_Throw_When_OwnerType_Is_Null()
    {
        DependencyProperty.RegisterReadOnly(
            nameof(RegisterReadOnly_Should_Throw_When_OwnerType_Is_Null),
            typeof(double),
            null,
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterReadOnly_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType()
    {
        DependencyProperty.RegisterReadOnly(
            nameof(RegisterReadOnly_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType),
            typeof(bool),
            typeof(MyDependencyObject1),
            new PropertyMetadata(100));
    }

    [TestMethod]
    public void RegisterReadOnly_Properties_And_Metadata()
    {
        DependencyPropertyKey dpKey = DependencyProperty.RegisterReadOnly(
            nameof(RegisterReadOnly_Properties_And_Metadata),
            typeof(char),
            typeof(MyDependencyObject1),
            new PropertyMetadata('x', propertyChangedCallback, coerceValueCallback),
            validateValueCallback);

        DependencyProperty dp = dpKey.DependencyProperty;

        Assert.IsFalse(dp.IsAttached);
        Assert.IsTrue(dp.ReadOnly);
        Assert.AreSame(dp.DependencyPropertyKey, dpKey);
        Assert.AreEqual(dp.Name, nameof(RegisterReadOnly_Properties_And_Metadata));
        Assert.AreEqual(dp.PropertyType, typeof(char));
        Assert.AreEqual(dp.OwnerType, typeof(MyDependencyObject1));
        Assert.AreEqual(dp.ValidateValueCallback, validateValueCallback);
        Assert.AreEqual((char)dp.DefaultMetadata.DefaultValue, 'x');

        PropertyMetadata metadata = dp.GetMetadata(typeof(MyDependencyObject1));

        Assert.AreEqual((char)metadata.DefaultValue, 'x');
        Assert.AreEqual(metadata.PropertyChangedCallback, propertyChangedCallback);
        Assert.AreEqual(metadata.CoerceValueCallback, coerceValueCallback);

        static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        static object coerceValueCallback(DependencyObject d, object value) => value;
        static bool validateValueCallback(object value) => true;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAttachedReadOnly_Should_Throw_When_Name_Is_Null()
    {
        DependencyProperty.RegisterAttachedReadOnly(
            null,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterAttachedReadOnly_Should_Throw_When_Name_Is_Empty()
    {
        DependencyProperty.RegisterAttachedReadOnly(
            string.Empty,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAttachedReadOnly_Should_Throw_When_PropertyType_Is_Null()
    {
        DependencyProperty.RegisterAttachedReadOnly(
            nameof(RegisterAttachedReadOnly_Should_Throw_When_PropertyType_Is_Null),
            null,
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAttachedReadOnly_Should_Throw_When_OwnerType_Is_Null()
    {
        DependencyProperty.RegisterAttachedReadOnly(
            nameof(RegisterAttachedReadOnly_Should_Throw_When_OwnerType_Is_Null),
            typeof(double),
            null,
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterAttachedReadOnly_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType()
    {
        DependencyProperty.RegisterAttachedReadOnly(
            nameof(RegisterAttachedReadOnly_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType),
            typeof(string),
            typeof(MyDependencyObject1),
            new PropertyMetadata(49.0));
    }

    [TestMethod]
    public void RegisterAttachedReadOnly_Properties_And_Metadata()
    {
        DependencyPropertyKey dpKey = DependencyProperty.RegisterAttachedReadOnly(
            nameof(RegisterAttachedReadOnly_Properties_And_Metadata),
            typeof(char),
            typeof(MyDependencyObject1),
            new PropertyMetadata('x', propertyChangedCallback, coerceValueCallback),
            validateValueCallback);

        DependencyProperty dp = dpKey.DependencyProperty;

        Assert.IsTrue(dp.IsAttached);
        Assert.IsTrue(dp.ReadOnly);
        Assert.AreSame(dp.DependencyPropertyKey, dpKey);
        Assert.AreEqual(dp.Name, nameof(RegisterAttachedReadOnly_Properties_And_Metadata));
        Assert.AreEqual(dp.PropertyType, typeof(char));
        Assert.AreEqual(dp.OwnerType, typeof(MyDependencyObject1));
        Assert.AreEqual(dp.ValidateValueCallback, validateValueCallback);
        Assert.AreEqual((char)dp.DefaultMetadata.DefaultValue, 'x');

        PropertyMetadata metadata = dp.GetMetadata(typeof(MyDependencyObject1));

        Assert.AreEqual((char)metadata.DefaultValue, 'x');
        Assert.AreEqual(metadata.PropertyChangedCallback, propertyChangedCallback);
        Assert.AreEqual(metadata.CoerceValueCallback, coerceValueCallback);

        static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        static object coerceValueCallback(DependencyObject d, object value) => value;
        static bool validateValueCallback(object value) => true;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ReadOnly_Property_Should_Throw_When_Writing_Without_Key()
    {
        var o = new MyDependencyObject1();
        o.SetValue(MyDependencyObject1.ReadOnlyProperty1, nameof(ReadOnly_Property_Should_Throw_When_Writing_Without_Key));
    }

    [TestMethod]
    public void ReadOnly_Property_Write_With_Key()
    {
        var o = new MyDependencyObject1();
        o.SetValue(MyDependencyObject1.ReadOnlyProperty1Key, nameof(ReadOnly_Property_Write_With_Key));

        Assert.AreEqual(o.ReadOnlyProperty1ChangedCounter1, 1);
        Assert.AreEqual((string)o.GetValue(MyDependencyObject1.ReadOnlyProperty1), nameof(ReadOnly_Property_Write_With_Key));
    }

    [TestMethod]
    public void GetReadOnlyValueCallback_Should_Have_Priority()
    {
        DependencyPropertyKey key = DependencyProperty.RegisterAttachedReadOnly(
                nameof(GetReadOnlyValueCallback_Should_Have_Priority),
                typeof(int),
                typeof(MyDependencyObject1),
                new MyReadOnlyPropertyMetadata(1, ReadOnlyCallback));

        DependencyProperty dp = key.DependencyProperty;

        var o = new MyDependencyObject1();

        Assert.AreEqual((int)o.GetValue(dp), 100);

        o.SetValue(key, 150);

        Assert.AreEqual((int)o.GetValue(dp), 100);

        static object ReadOnlyCallback(DependencyObject d)
        {
            return 100;
        }
    }

    [TestMethod]
    public void Inherit_ReadOnly_Property_Should_Propagate()
    {
        DependencyPropertyKey key = DependencyProperty.RegisterReadOnly(
            nameof(Inherit_ReadOnly_Property_Should_Propagate),
            typeof(string),
            typeof(MyDependencyObject2),
            new PropertyMetadata(string.Empty, OnChanged)
            {
                Inherits = true,
            });

        DependencyProperty dp = key.DependencyProperty;

        var o1 = new MyDependencyObject2();
        var o2 = new MyDependencyObject2();
        var o3 = new MyDependencyObject2();

        o1.Child = o2;
        o2.Child = o3;

        o1.SetValue(key, nameof(Inherit_ReadOnly_Property_Should_Propagate));

        Assert.AreEqual(o1.PropertyChangedCounter, 1);
        Assert.AreEqual(o2.PropertyChangedCounter, 1);
        Assert.AreEqual(o3.PropertyChangedCounter, 1);

        Assert.AreEqual((string)o1.GetValue(dp), nameof(Inherit_ReadOnly_Property_Should_Propagate));
        Assert.AreEqual((string)o2.GetValue(dp), nameof(Inherit_ReadOnly_Property_Should_Propagate));
        Assert.AreEqual((string)o3.GetValue(dp), nameof(Inherit_ReadOnly_Property_Should_Propagate));

        static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (MyDependencyObject2)d;
            o.PropertyChangedCounter++;
        }
    }

    internal partial class MyDependencyObject1
    {
        public int ReadOnlyProperty1ChangedCounter1 { get; private set; }

        public event EventHandler ReadOnlyProperty1Changed1;

        public static readonly DependencyPropertyKey ReadOnlyProperty1Key =
            DependencyProperty.RegisterReadOnly(
                nameof(ReadOnlyProperty1),
                typeof(string),
                typeof(MyDependencyObject1),
                new PropertyMetadata(string.Empty, OnReadOnlyProperty1Changed));

        public static readonly DependencyProperty ReadOnlyProperty1 = ReadOnlyProperty1Key.DependencyProperty;

        private static void OnReadOnlyProperty1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (MyDependencyObject1)d;
            o.ReadOnlyProperty1ChangedCounter1++;
            o.ReadOnlyProperty1Changed1?.Invoke(o, EventArgs.Empty);
        }
    }
}

file class MyDependencyObject2 : FrameworkElement
{
    internal override int VisualChildrenCount => Child is null ? 0 : 1;

    internal override UIElement GetVisualChild(int index)
    {
        if (index != 0 || Child is null)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Child;
    }

    public int PropertyChangedCounter { get; set; }

    private MyDependencyObject2 _child;

    public MyDependencyObject2 Child
    {
        get => _child;
        set
        {
            if (_child != null)
            {
                RemoveVisualChild(_child);
            }

            _child = value;

            if (_child != null)
            {
                AddVisualChild(_child);
            }
        }
    }
}

file class MyReadOnlyPropertyMetadata : PropertyMetadata
{
    public MyReadOnlyPropertyMetadata(object defaultValue, GetReadOnlyValueCallback readOnlyCallback)
        : base(defaultValue)
    {
        GetReadOnlyValueCallback = readOnlyCallback;
    }

    internal override GetReadOnlyValueCallback GetReadOnlyValueCallback { get; }
}
