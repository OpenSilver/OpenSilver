
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

namespace System.Windows.Tests;

[TestClass]
public class DependencyObjectTest
{
    [TestMethod]
    public void DependencyObject_Should_Accept_DependencyObject_Inheritance_Context()
    {
        var do1 = new MyDependencyObject();
        var do2 = new MyDependencyObject();

        do2.MyProperty = do1;

        Assert.AreSame(do1.InheritanceContext, do2);
    }

    [TestMethod]
    public void DependencyObject_Should_Accept_FrameworkElement_Inheritance_Context()
    {
        var do1 = new MyDependencyObject();
        var fe1 = new MyFrameworkElement();

        fe1.MyProperty = do1;

        Assert.AreSame(do1.InheritanceContext, fe1);
    }

    [TestMethod]
    public void FrameworkElement_Should_Not_Accept_Inheritance_Context()
    {
        var fe1 = new MyFrameworkElement();
        var fe2 = new MyFrameworkElement();

        fe1.MyProperty = fe2;

        Assert.IsNull(fe2.InheritanceContext);
    }

    [TestMethod]
    public void DependencyObject_Should_Not_Replace_Inheritance_Context()
    {
        var do1 = new MyDependencyObject();
        var fe1 = new MyFrameworkElement();
        var fe2 = new MyFrameworkElement();

        fe1.MyProperty = do1;
        fe2.MyProperty = do1;

        Assert.AreSame(do1.InheritanceContext, fe1);
    }

    [TestMethod]
    public void DependencyObject_Should_Remove_Inheritance_Context()
    {
        var do1 = new MyDependencyObject();
        var fe1 = new MyFrameworkElement();

        fe1.MyProperty = do1;

        Assert.AreSame(do1.InheritanceContext, fe1);

        fe1.MyProperty = null;

        Assert.IsNull(do1.InheritanceContext);
    }

    [TestMethod]
    public void DependencyObject_Should_Fire_InheritedContextChanged_Event()
    {
        var do1 = new MyDependencyObject();
        var fe1 = new MyFrameworkElement();

        int changedCount = 0;
        do1.InheritedContextChanged += (o, e) =>
        {
            changedCount++;
        };

        fe1.MyProperty = do1;

        Assert.AreEqual(changedCount, 1);
    }

    [TestMethod]
    public void DependencyObject_Should_Fire_InheritedContextChanged_Event_On_Children()
    {
        var do1 = new MyDependencyObject();
        var do2 = new MyDependencyObject();
        var do3 = new MyDependencyObject();
        var do4 = new MyDependencyObject();

        int changedCount = 0;
        do4.InheritedContextChanged += (o, e) =>
        {
            changedCount++;
        };

        do3.MyProperty = do4;

        Assert.AreEqual(changedCount, 1);

        do2.MyProperty = do3;

        Assert.AreEqual(changedCount, 2);

        do1.MyProperty = do2;

        Assert.AreEqual(changedCount, 3);
    }
}

file class MyDependencyObject : DependencyObject
{
    public object MyProperty
    {
        get => GetValue(MyPropertyProperty);
        set => SetValue(MyPropertyProperty, value);
    }

    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.Register(
            nameof(MyProperty),
            typeof(object),
            typeof(MyDependencyObject),
            null);
}

file class MyFrameworkElement : FrameworkElement
{
    public object MyProperty
    {
        get => GetValue(MyPropertyProperty);
        set => SetValue(MyPropertyProperty, value);
    }

    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.Register(
            nameof(MyProperty),
            typeof(object),
            typeof(MyFrameworkElement),
            null);
}
