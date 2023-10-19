
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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Tests;

[TestClass]
public partial class DependencyPropertyTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Register_Should_Throw_When_Name_Is_Null()
    {
        DependencyProperty.Register(
            null,
            typeof(double),
            typeof(MyDependencyObject1),
            null); 
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_Should_Throw_When_Name_Is_Empty()
    {
        DependencyProperty.Register(
            string.Empty,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Register_Should_Throw_When_PropertyType_Is_Null()
    {
        DependencyProperty.Register(
            nameof(Register_Should_Throw_When_PropertyType_Is_Null),
            null,
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Register_Should_Throw_When_OwnerType_Is_Null()
    {
        DependencyProperty.Register(
            nameof(Register_Should_Throw_When_OwnerType_Is_Null),
            typeof(double),
            null,
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType()
    {
        DependencyProperty.Register(
            nameof(Register_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType),
            typeof(double),
            typeof(MyDependencyObject1),
            new PropertyMetadata(string.Empty));
    }

    [TestMethod]
    public void Register_Properties_And_Metadata()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(Register_Properties_And_Metadata),
            typeof(char),
            typeof(MyDependencyObject1),
            new PropertyMetadata('x', propertyChangedCallback, coerceValueCallback),
            validateValueCallback);
        
        Assert.IsFalse(dp.IsAttached);
        Assert.IsFalse(dp.ReadOnly);
        Assert.IsNull(dp.DependencyPropertyKey);
        Assert.AreEqual(dp.Name, nameof(Register_Properties_And_Metadata));
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
    public void RegisterAttached_Should_Throw_When_Name_Is_Null()
    {
        DependencyProperty.RegisterAttached(
            null,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterAttached_Should_Throw_When_Name_Is_Empty()
    {
        DependencyProperty.RegisterAttached(
            string.Empty,
            typeof(double),
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAttached_Should_Throw_When_PropertyType_Is_Null()
    {
        DependencyProperty.RegisterAttached(
            nameof(RegisterAttached_Should_Throw_When_PropertyType_Is_Null),
            null,
            typeof(MyDependencyObject1),
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAttached_Should_Throw_When_OwnerType_Is_Null()
    {
        DependencyProperty.RegisterAttached(
            nameof(RegisterAttached_Should_Throw_When_OwnerType_Is_Null),
            typeof(double),
            null,
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterAttached_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType()
    {
        DependencyProperty.RegisterAttached(
            nameof(RegisterAttached_Should_Throw_When_DefaultValue_Type_Does_Not_Match_PropertyType),
            typeof(float),
            typeof(MyDependencyObject1),
            new PropertyMetadata('a'));
    }

    [TestMethod]
    public void RegisterAttached_Properties_And_Metadata()
    {
        DependencyProperty dp = DependencyProperty.RegisterAttached(
            nameof(RegisterAttached_Properties_And_Metadata),
            typeof(char),
            typeof(MyDependencyObject1),
            new PropertyMetadata('x', propertyChangedCallback, coerceValueCallback),
            validateValueCallback);

        Assert.IsTrue(dp.IsAttached);
        Assert.IsFalse(dp.ReadOnly);
        Assert.IsNull(dp.DependencyPropertyKey);
        Assert.AreEqual(dp.Name, nameof(RegisterAttached_Properties_And_Metadata));
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
    public void Register_DefaultMetadata_Should_Have_Only_DefaultValue()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(Register_DefaultMetadata_Should_Have_Only_DefaultValue),
            typeof(double),
            typeof(MyDependencyObject1),
            new PropertyMetadata(42d, OnChanged, Coerce)
            {
                MethodToUpdateDom = UpdateDom,
                MethodToUpdateDom2 = UpdateDom2,
                GetCSSEquivalent = GetCSSEquivalent,
                GetCSSEquivalents = GetCSSEquivalents,
            });

        Assert.AreEqual(dp.DefaultMetadata.DefaultValue, 42d);
        Assert.IsNull(dp.DefaultMetadata.PropertyChangedCallback);
        Assert.IsNull(dp.DefaultMetadata.CoerceValueCallback);
        Assert.IsNull(dp.DefaultMetadata.MethodToUpdateDom);
        Assert.IsNull(dp.DefaultMetadata.MethodToUpdateDom2);
        Assert.IsNull(dp.DefaultMetadata.GetCSSEquivalent);
        Assert.IsNull(dp.DefaultMetadata.GetCSSEquivalents);

        static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        static object Coerce(DependencyObject d, object baseValue) => baseValue;
        static void UpdateDom(DependencyObject d, object value) { }
        static void UpdateDom2(DependencyObject d, object oldValue, object newValue) { }
        static CSSEquivalent GetCSSEquivalent(DependencyObject d) => null;
        static List<CSSEquivalent> GetCSSEquivalents(DependencyObject d) => null;
    }
    
    [TestMethod]
    public void Register_Should_Allow_Overridding_Property()
    {
        DependencyProperty dp1 = DependencyProperty.Register(
            nameof(Register_Should_Allow_Overridding_Property),
            typeof(int),
            typeof(MyDependencyObject1),
            new PropertyMetadata(42));

        DependencyProperty dp2 = DependencyProperty.Register(
            nameof(Register_Should_Allow_Overridding_Property),
            typeof(int),
            typeof(MyDependencyObject1),
            new PropertyMetadata(42));

        Assert.AreEqual(dp2, DependencyProperty.FromName(nameof(Register_Should_Allow_Overridding_Property), typeof(MyDependencyObject1)));
    }

    [TestMethod]
    public void Callback_Should_Not_Be_Called_When_Value_Does_Not_Change()
    {
        var o = new MyDependencyObject1();
        o.SetValue(MyDependencyObject1.Property1, MyDependencyObject1.Property1.GetMetadata(o.GetType()).DefaultValue);

        Assert.AreEqual(o.Property1ChangedCounter1, 0);
    }

    [TestMethod]
    public void Callback_Should_Be_Called_When_Value_Change()
    {
        var o = new MyDependencyObject1();
        o.SetValue(MyDependencyObject1.Property1, (double)MyDependencyObject1.Property1.GetMetadata(o.GetType()).DefaultValue + 1);

        Assert.AreEqual(o.Property1ChangedCounter1, 1);
    }

    [TestMethod]
    public void SetValue_Should_Call_CoerceValueCallback()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(SetValue_Should_Call_CoerceValueCallback),
            typeof(double),
            typeof(MyDependencyObject1),
            new PropertyMetadata(10.0, null, Coerce));

        var o = new MyDependencyObject1();
        o.SetValue(dp, 1.0);

        Assert.AreEqual(1.0, (double)o.GetValue(dp));

        o.SetValue(dp, 15.0);
        
        Assert.AreEqual(150.0, (double)o.GetValue(dp));

        static object Coerce(DependencyObject d, object baseValue)
        {
            double v = (double)baseValue;
            if (v >= 10.0)
            {
                return 150.0;
            }

            return v;
        }
    }

    [TestMethod]
    public void ValidateValueCallback_Should_Throw_When_Condition_Is_Not_Met()
    {
        DependencyProperty dp = DependencyProperty.Register(
            nameof(ValidateValueCallback_Should_Throw_When_Condition_Is_Not_Met),
            typeof(int),
            typeof(MyDependencyObject1),
            null,
            ValidateValue);

        var o = new MyDependencyObject1();
        o.SetValue(dp, 0);
        o.SetValue(dp, 1);
        o.SetValue(dp, 2);
        o.SetValue(dp, 3);
        o.SetValue(dp, 4);
        o.SetValue(dp, 5);

        Assert.ThrowsException<ArgumentException>(() => o.SetValue(dp, -1));
        Assert.ThrowsException<ArgumentException>(() => o.SetValue(dp, 6));

        static bool ValidateValue(object value)
        {
            int v = (int)value;
            return v >= 0 && v <= 5;
        }
    }
    
    internal partial class MyDependencyObject1 : DependencyObject
    {
        public int Property1ChangedCounter1 { get; private set; }

        public event EventHandler Property1Changed1;

        public static readonly DependencyProperty Property1 =
            DependencyProperty.Register(
                nameof(Property1),
                typeof(double),
                typeof(MyDependencyObject1),
                new PropertyMetadata(42.0, OnProperty1Changed));

        private static void OnProperty1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (MyDependencyObject1)d;
            o.Property1ChangedCounter1++;
            o.Property1Changed1?.Invoke(o, EventArgs.Empty);
        }
    }
}
