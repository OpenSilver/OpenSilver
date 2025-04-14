// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Tests;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Input.Tests;

[TestClass]
public sealed class MouseGestureConverterTest : TypeConverterTestBase
{
    protected override TypeConverter Converter { get; } = new MouseGestureConverter();

    [DataTestMethod]
    // Valid type
    [DataRow(true, typeof(string))]
    // Invalid types
    [DataRow(false, typeof(MouseAction))]
    [DataRow(false, typeof(ModifierKeys))]
    [DataRow(false, typeof(KeyGesture))]
    [DataRow(false, typeof(MouseGesture))]
    [DataRow(false, typeof(InstanceDescriptor))]
    public void CanConvertFrom_ReturnsExpected(bool expected, Type sourceType)
    {
        Assert.AreEqual(expected, Converter.CanConvertFrom(sourceType));
    }

    [DataTestMethod]
    [DynamicData(nameof(CanConvertTo_Data), DynamicDataSourceType.Property)]
    public void CanConvertTo_ReturnsExpected(bool expected, bool passContext, object value, Type destinationType)
    {
        StandardContextImpl context = new() { Instance = value };

        Assert.AreEqual(expected, Converter.CanConvertTo(passContext ? context : null, destinationType));
    }

    public static IEnumerable<object[]> CanConvertTo_Data
    {
        get
        {
            // Supported cases
            yield return new object[] { true, true, new MouseGesture(MouseAction.None, ModifierKeys.Control), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.None, ModifierKeys.Alt), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.MiddleDoubleClick, ModifierKeys.Shift), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Control | ModifierKeys.Windows | ModifierKeys.Alt), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control | ModifierKeys.Windows), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Alt | ModifierKeys.Windows), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt | ModifierKeys.Control), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Alt | ModifierKeys.Windows | ModifierKeys.Control), typeof(string) };
            yield return new object[] { true, true, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.None), typeof(string) };

            // Unsupported cases (Null Context)
            yield return new object[] { false, false, null, typeof(string) };
            // Unsupported cases (Null Context/Destination Type)
            yield return new object[] { false, false, null, null };
            // Unsupported cases (Null Instance)
            yield return new object[] { false, true, null, typeof(string) };
            // Unsupported cases (Null Instance/Destination Type)
            yield return new object[] { false, true, null, null };
            // Unsupported cases (Wrong destination type)
            yield return new object[] { false, true, new MouseGesture(MouseAction.None, ModifierKeys.Control), null };
            yield return new object[] { false, true, new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control | ModifierKeys.Windows), typeof(KeyGesture) };
            yield return new object[] { false, true, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Alt | ModifierKeys.Windows), typeof(MouseGesture) };
            yield return new object[] { false, true, new MouseGesture(MouseAction.None, ModifierKeys.Control), typeof(MouseAction) };
            yield return new object[] { false, true, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Alt | ModifierKeys.Windows | ModifierKeys.Control), typeof(Key) };
            yield return new object[] { false, true, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.None), typeof(ModifierKeys) };
            // Unsupported cases (Wrong Context Instance)
            yield return new object[] { false, true, new KeyGesture(Key.None, ModifierKeys.Alt), typeof(string) };
            yield return new object[] { false, true, MouseAction.WheelClick, typeof(string) };
            yield return new object[] { false, true, Key.F1, typeof(string) };

            // We do not test for malformed MouseGesture as MouseGesture has to perform its own validation and shall be enforced via its own unit tests
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ConvertFrom_ReturnsExpected_Data), DynamicDataSourceType.Property)]
    public void ConvertFrom_ReturnsExpected(MouseGesture expected, ITypeDescriptorContext context, CultureInfo cultureInfo, string value)
    {
        MouseGesture converted = (MouseGesture)Converter.ConvertFrom(context, cultureInfo, value);
        Assert.AreEqual(expected.Modifiers, converted.Modifiers);
        Assert.AreEqual(expected.MouseAction, converted.MouseAction);
    }

    public static IEnumerable<object[]> ConvertFrom_ReturnsExpected_Data
    {
        get
        {
            // Supported cases (Culture must stay irrelevant, MouseAction/ModifierKeys also do not care)
            yield return new object[] { new MouseGesture(MouseAction.None, ModifierKeys.None), null, CultureInfo.InvariantCulture, string.Empty };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.None), null, new CultureInfo("ru-RU"), "LeftClick" };
            yield return new object[] { new MouseGesture(MouseAction.None, ModifierKeys.Control), null, CultureInfo.InvariantCulture, "Ctrl+" };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), null, CultureInfo.InvariantCulture, "Ctrl+LeftClick" };
            yield return new object[] { new MouseGesture(MouseAction.MiddleDoubleClick, ModifierKeys.Alt), null, new CultureInfo("no-NO"), "Alt+MiddleDoubleClick" };
            yield return new object[] { new MouseGesture(MouseAction.WheelClick, ModifierKeys.Shift), null, CultureInfo.InvariantCulture, "Shift+WheelClick" };
            yield return new object[] { new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Windows), null, CultureInfo.InvariantCulture, "Windows+LeftDoubleClick" };
            yield return new object[] { new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Alt), null, CultureInfo.InvariantCulture, "Ctrl+Alt+RightClick" };
            yield return new object[] { new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Control | ModifierKeys.Windows | ModifierKeys.Alt), null, CultureInfo.InvariantCulture, "Ctrl+Alt+Windows+RightDoubleClick" };

            // Supported cases (fuzzed)
            yield return new object[] { new MouseGesture(MouseAction.None, ModifierKeys.Alt), null, CultureInfo.InvariantCulture, "Alt+                " };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.None), null, CultureInfo.InvariantCulture, "   LeftClick  " };
            yield return new object[] { new MouseGesture(MouseAction.None, ModifierKeys.None), null, CultureInfo.InvariantCulture, "                   " };
            yield return new object[] { new MouseGesture(MouseAction.WheelClick, ModifierKeys.Shift), null, CultureInfo.InvariantCulture, "Shift      +WheelClick" };
            yield return new object[] { new MouseGesture(MouseAction.MiddleClick, ModifierKeys.Windows | ModifierKeys.Shift), null, new CultureInfo("no-NO"), " Shift +   Windows  +    MiddleClick   " };
            yield return new object[] { new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Windows), null, CultureInfo.InvariantCulture, "Windows+       LeftDoubleClick   " };
            yield return new object[] { new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Alt), null, CultureInfo.InvariantCulture, "Ctrl+Alt+ RightClick" };
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ConvertFrom_ThrowsNotSupportedException_Data), DynamicDataSourceType.Property)]
    public void ConvertFrom_ThrowsNotSupportedException(CultureInfo cultureInfo, object value)
    {
        Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(null, cultureInfo, value));
    }

    public static IEnumerable<object[]> ConvertFrom_ThrowsNotSupportedException_Data
    {
        get
        {
            // Nulls are not supported
            yield return new object[] { CultureInfo.InvariantCulture, null };
            // Anything that isn't a string ain't supported
            yield return new object[] { CultureInfo.InvariantCulture, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control) };
            yield return new object[] { CultureInfo.InvariantCulture, new KeyGesture(Key.V, ModifierKeys.Control) };
            yield return new object[] { CultureInfo.InvariantCulture, ModifierKeys.Control };
            yield return new object[] { CultureInfo.InvariantCulture, MouseAction.LeftDoubleClick };
            yield return new object[] { CultureInfo.InvariantCulture, Key.V };
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ConvertTo_ReturnsExpected_Data), DynamicDataSourceType.Property)]
    public void ConvertTo_ReturnsExpected(string expected, ITypeDescriptorContext context, CultureInfo cultureInfo, object value)
    {
        // Culture and context must not have any meaning
        Assert.AreEqual(expected, Converter.ConvertTo(context, cultureInfo, value, typeof(string)));
    }

    public static IEnumerable<object[]> ConvertTo_ReturnsExpected_Data
    {
        get
        {
            // Supported null value case that returns string.Empty
            yield return new object[] { string.Empty, null, CultureInfo.InvariantCulture, null };

            // Supported cases (Culture must stay irrelevant, MouseAction/ModifierKeys also do not care)
            yield return new object[] { string.Empty, null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.None, ModifierKeys.None) };
            yield return new object[] { "Alt+", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.None, ModifierKeys.Alt) };
            yield return new object[] { "Windows+", null, new CultureInfo("de-DE"), new MouseGesture(MouseAction.None, ModifierKeys.Windows) };
            yield return new object[] { "Shift+", null, new CultureInfo("ru-RU"), new MouseGesture(MouseAction.None, ModifierKeys.Shift) };
            yield return new object[] { "LeftClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.LeftClick) };
            yield return new object[] { "Ctrl+LeftClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control) };
            yield return new object[] { "Alt+RightClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt) };
            yield return new object[] { "Windows+WheelClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.WheelClick, ModifierKeys.Windows) };
            yield return new object[] { "Alt+RightDoubleClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Alt) };
            yield return new object[] { "Ctrl+Alt+Windows+WheelClick", null, new CultureInfo("de-DE"), new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control | ModifierKeys.Windows | ModifierKeys.Alt) };
            yield return new object[] { "Alt+Windows+MiddleDoubleClick", null, new CultureInfo("ru-RU"), new MouseGesture(MouseAction.MiddleDoubleClick, ModifierKeys.Alt | ModifierKeys.Windows) };
            yield return new object[] { "Ctrl+Alt+Windows+MiddleClick", null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.MiddleClick, ModifierKeys.Alt | ModifierKeys.Windows | ModifierKeys.Control) };
        }
    }

    [TestMethod]
    public void ConvertTo_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => Converter.ConvertTo(null, CultureInfo.InvariantCulture, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), null));
    }

    [DataTestMethod]
    [DynamicData(nameof(ConvertTo_ThrowsNotSupportedException_Data), DynamicDataSourceType.Property)]
    public void ConvertTo_ThrowsNotSupportedException(object value, Type destinationType)
    {
        Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertTo(null, CultureInfo.InvariantCulture, value, destinationType));
    }

    public static IEnumerable<object[]> ConvertTo_ThrowsNotSupportedException_Data
    {
        get
        {
            // Wrong destination types
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), typeof(MouseGesture) };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), typeof(KeyGesture) };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), typeof(MouseAction) };
            yield return new object[] { new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), typeof(Key) };
            yield return new object[] { new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control), typeof(ModifierKeys) };
            // Wrong value types
            yield return new object[] { new KeyGesture(Key.V, ModifierKeys.Control), typeof(string) };
            yield return new object[] { MouseAction.MiddleDoubleClick, typeof(string) };
            yield return new object[] { ModifierKeys.Control, typeof(string) };
            yield return new object[] { Key.V, typeof(string) };
        }
    }

    public sealed class StandardContextImpl : ITypeDescriptorContext
    {
        public IContainer Container => throw new NotImplementedException();

        public object Instance { get; set; }

        public PropertyDescriptor PropertyDescriptor => throw new NotImplementedException();
        public object GetService(Type serviceType) => throw new NotImplementedException();
        public void OnComponentChanged() => throw new NotImplementedException();
        public bool OnComponentChanging() => throw new NotImplementedException();
    }
}
