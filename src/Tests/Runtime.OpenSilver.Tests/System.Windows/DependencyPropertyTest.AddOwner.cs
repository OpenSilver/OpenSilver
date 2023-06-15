
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
    public void AddOwner_Should_Throw_When_OwnerType_Is_Null()
    {
        MyDependencyObject1.Property1.AddOwner(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddOwner_Should_Throw_When_Same_OwnerType_Twice()
    {
        MyDependencyObject1.Property1.AddOwner(typeof(MyDependencyObject1));
    }

    [TestMethod]
    public void AddOwner_Should_Return_Reference_To_Original_Property()
    {
        Assert.AreSame(MyDependencyObject1.Property1, MyDependencyObject2.Property1);
    }

    [TestMethod]
    public void AddOwner_Should_Find_Property_With_FromName()
    {
        DependencyProperty dp = DependencyProperty.FromName(MyDependencyObject2.Property1.Name, typeof(MyDependencyObject2));

        Assert.AreSame(dp, MyDependencyObject2.Property1);
    }

    [TestMethod]
    public void AddOwner_Should_Override_Metadata()
    {
        DependencyProperty dp = MyDependencyObject1.Property1.AddOwner(
            typeof(MyDependencyObject3),
            new PropertyMetadata(69.0, Changed));

        var o = new MyDependencyObject3();

        Assert.AreEqual((double)o.GetValue(dp), 69.0);
        Assert.AreEqual((double)o.GetValue(MyDependencyObject1.Property1), 69.0);

        o.SetValue(dp, 420.0);

        Assert.AreEqual(o.Property1ChangedCounter, 1);

        static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (MyDependencyObject3)d;
            o.Property1ChangedCounter++;
        }
    }
}

file class MyDependencyObject2 : DependencyObject
{
    public static readonly DependencyProperty Property1 =
        DependencyPropertyTest.MyDependencyObject1.Property1.AddOwner(typeof(MyDependencyObject2));
}

file class MyDependencyObject3 : DependencyObject
{
    public int Property1ChangedCounter { get; set; }
}
