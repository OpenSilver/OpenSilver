
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
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace System.ComponentModel.Tests;

[TestClass]
public class DependencyPropertyDescriptorTests
{
    [TestMethod]
    public void Add_Remove_ValueChanged()
    {
        var component = new SubDependencyObject();
        var property = TypeDescriptor.GetProperties(component)[SubDependencyObject.PropertyProperty.Name];

        int count = 0;

        void OnChanged(object sender, EventArgs e)
        {
            count++;
        }

        property.AddValueChanged(component, OnChanged);

        component.Property++;

        Assert.AreEqual(1, count);

        component.Property++;

        Assert.AreEqual(2, count);

        property.RemoveValueChanged(component, OnChanged);

        component.Property++;

        Assert.AreEqual(2, count);
    }

    public static IEnumerable<object[]> FromProperty_TestData()
    {
        DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(
            TypeDescriptor.GetProperties(new SubDependencyObject())[SubDependencyObject.PropertyProperty.Name]);

        yield return new object[] {
            descriptor, false, false, SubDependencyObject.PropertyProperty, typeof(SubDependencyObject),
            typeof(int), SubDependencyObject.Category, SubDependencyObject.Description, false,
            SubDependencyObject.DisplayName, false, true, true, typeof(Int32Converter),
        };

        descriptor = DependencyPropertyDescriptor.FromProperty(
            SubDependencyObject.AttachedPropertyProperty, typeof(SubDependencyObject));

        yield return new object[] {
            descriptor, true, false, SubDependencyObject.AttachedPropertyProperty, typeof(SubDependencyObject),
            typeof(double), CategoryAttribute.Default.Category, string.Empty, false,
            "SubDependencyObject.AttachedProperty", true, false, true, typeof(DoubleConverter),
        };
    }

    [TestMethod]
    [DynamicData(nameof(FromProperty_TestData))]
    public void FromProperty_InvokeDependencyObjectPropertyDescriptor_Success(
        DependencyPropertyDescriptor descriptor,
        bool isAttached,
        bool isReadonly,
        DependencyProperty dp,
        Type componentType,
        Type propertyType,
        string category,
        string description,
        bool designTimeOnly,
        string displayName,
        bool isBrowsable,
        bool isLocalizable,
        bool supportsChangeEvents,
        Type converterType)
    {
        Assert.AreEqual(descriptor.IsAttached, isAttached);
        Assert.AreEqual(descriptor.IsReadOnly, isReadonly);
        Assert.AreSame(descriptor.DependencyProperty, dp);
        Assert.AreSame(descriptor.ComponentType, componentType);
        Assert.AreSame(descriptor.PropertyType, propertyType);
        Assert.AreEqual(descriptor.Category, category);
        Assert.AreEqual(descriptor.Description, description);
        Assert.AreEqual(descriptor.DesignTimeOnly, designTimeOnly);
        Assert.AreEqual(descriptor.DisplayName, displayName);
        Assert.AreEqual(descriptor.IsBrowsable, isBrowsable);
        Assert.AreEqual(descriptor.IsLocalizable, isLocalizable);
        Assert.AreEqual(descriptor.SupportsChangeEvents, supportsChangeEvents);
        if (converterType is null)
        {
            Assert.IsNull(descriptor.Converter);
        }
        else
        {
            Assert.IsInstanceOfType(descriptor.Converter, converterType);
        }
    }

    [TestMethod]
    public void FromProperty_InvokeNotDependencyObjectPropertyDescriptor_Success()
    {
        var component = new NotDependencyObject();
        PropertyDescriptor property = TypeDescriptor.GetProperties(component)[nameof(NotDependencyObject.Property)];

        // Get descriptor.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property));

        // Get descriptor again.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property));
    }

    [TestMethod]
    public void FromProperty_NullProperty_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => DependencyPropertyDescriptor.FromProperty(null));
        Assert.AreEqual("property", ex.ParamName);
    }

    [TestMethod]
    public void FromProperty_InvokeTypeDependencyProperty_ReturnsNull()
    {
        DependencyProperty property = DependencyProperty.Register(nameof(DependencyPropertyDescriptorTests) + MethodBase.GetCurrentMethod().Name, typeof(bool), typeof(DependencyObject));

        // Get descriptor.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property, typeof(DependencyProperty)));

        // Get descriptor again.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property, typeof(DependencyProperty)));
    }

    [TestMethod]
    public void FromProperty_InvokeTypeNotDependencyProperty_ReturnsNull()
    {
        DependencyProperty property = DependencyProperty.Register(nameof(DependencyPropertyDescriptorTests) + MethodBase.GetCurrentMethod().Name, typeof(bool), typeof(DependencyObject));

        // Get descriptor.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property, typeof(int)));

        // Get descriptor again.
        Assert.IsNull(DependencyPropertyDescriptor.FromProperty(property, typeof(int)));
    }

    [TestMethod]
    public void FromProperty_NullDependencyProperty_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => DependencyPropertyDescriptor.FromProperty(null, typeof(object)));
        Assert.AreEqual("dependencyProperty", ex.ParamName);
    }

    [TestMethod]
    public void FromProperty_NullTargetType_ThrowsArgumentNullException()
    {
        DependencyProperty property = DependencyProperty.Register(MethodBase.GetCurrentMethod().Name, typeof(string), typeof(DependencyObject));
        var ex = Assert.Throws<ArgumentNullException>(() => DependencyPropertyDescriptor.FromProperty(property, null));
        Assert.AreEqual("targetType", ex.ParamName);
    }

    public class NotDependencyObject
    {
        public int Property { get; set; }
    }

    private class SubDependencyObject : DependencyObject
    {
        public const string Category = "TestCategory";
        public const string Description = "TestDescription";
        public const string DisplayName = "TestDisplayName";

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(
                nameof(Property),
                typeof(int),
                typeof(SubDependencyObject),
                new PropertyMetadata(0));

        [Category(Category)]
        [Description(Description)]
        [DisplayName(DisplayName)]
        [Browsable(false)]
        [Localizable(true)]
        public int Property
        {
            get { return (int)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        public static readonly DependencyProperty AttachedPropertyProperty =
            DependencyProperty.RegisterAttached(
                "AttachedProperty",
                typeof(double),
                typeof(SubDependencyObject),
                new PropertyMetadata(0.0));

        public static double GetAttachedProperty(DependencyObject d) => (double)d.GetValue(AttachedPropertyProperty);

        public static void SetAttachedProperty(DependencyObject d, double value) => d.SetValue(AttachedPropertyProperty, value);
    }
}