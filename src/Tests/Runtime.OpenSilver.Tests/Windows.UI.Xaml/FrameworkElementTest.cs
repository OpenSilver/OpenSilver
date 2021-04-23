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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class FrameworkElementTest
    {
        #region FrameworkElement.Style

        [TestMethod]
        public void FE_Set_Style_Invalid_TargetType()
        {
            var fe = new TestFE();

            var style = new Style(typeof(Button));

            Assert.ThrowsException<InvalidOperationException>(() => fe.Style = style);
        }

        [TestMethod]
        public void FE_Set_Style_Check_IsSealed()
        {
            var fe = new TestFE();

            var style = new Style(typeof(TestFE));

            style.IsSealed.Should().BeFalse();

            fe.Style = style;

            style.IsSealed.Should().BeTrue();
        }

        [TestMethod]
        public void FE_Set_Style_Not_Null()
        {
            var fe = TestFeWithStyle();

            fe.Prop1.Should().Be(55d);
            fe.Prop2.Should().BeNull();
        }

        [TestMethod]
        public void FE_Set_Style_Null()
        {
            var fe = TestFeWithStyle();

            fe.Style = null;

            fe.Prop1.Should().Be((double)TestFE.Prop1Property.GetMetadata(fe.GetType()).DefaultValue);
            fe.Prop2.Should().Be((Brush)TestFE.Prop2Property.GetMetadata(fe.GetType()).DefaultValue);
        }

        [TestMethod]
        public void FE_Set_Style_Not_Null_From_Not_Null()
        {
            var fe = TestFeWithStyle();

            var style = new Style(typeof(TestFE));
            style.Setters.Add(new Setter(TestFE.Prop1Property, 1d));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(-10d)));

            fe.Style = style;

            fe.Prop1.Should().Be(1d);
            fe.Prop2.Should().Be((Brush)TestFE.Prop2Property.GetMetadata(fe.GetType()).DefaultValue);
            fe.Margin.Should().Be(new Thickness(-10d));
        }

        [TestMethod]
        public void FE_Set_Style_When_Setter_Value_Is_Binding()
        {
            var fe = new TestFE();
            fe.DataContext = 100d;

            var style = new Style(typeof(TestFE));
            style.Setters.Add(new Setter(TestFE.Prop1Property, new Binding()));

            fe.Style = style;

            fe.Prop1.Should().Be(100d);
        }

        [TestMethod]
        public void FE_Set_Style_When_Setter_Value_Is_Binding_And_Share_Style()
        {
            var fe1 = new TestFE();
            fe1.DataContext = 100d;

            var fe2 = new TestFE();
            fe2.DataContext = 200d;

            var style = new Style(typeof(TestFE));
            style.Setters.Add(new Setter(TestFE.Prop1Property, new Binding()));

            fe1.Style = style;

            fe1.Prop1.Should().Be(100d);

            fe2.Style = style;

            fe2.Prop1.Should().Be(200d);
        }

        #endregion FrameworkElement.Style

        private static TestFE TestFeWithStyle()
        {
            var fe = new TestFE();

            var style = new Style(typeof(TestFE));
            style.Setters.Add(new Setter(TestFE.Prop1Property, 55d));
            style.Setters.Add(new Setter(TestFE.Prop2Property, null));

            fe.Style = style;

            return fe;
        }
    }

    #region Helpers

    public class TestFE : FrameworkElement
    {
        public static readonly DependencyProperty Prop1Property =
            DependencyProperty.Register(
                nameof(Prop1),
                typeof(double),
                typeof(TestFE),
                new PropertyMetadata(42d));

        public double Prop1
        {
            get => (double)GetValue(Prop1Property);
            set => SetValue(Prop1Property, value);
        }

        public static readonly DependencyProperty Prop2Property =
            DependencyProperty.Register(
                nameof(Prop2),
                typeof(Brush),
                typeof(TestFE),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush Prop2
        {
            get => (Brush)GetValue(Prop2Property);
            set => SetValue(Prop2Property, value);
        }
    }

    #endregion Helpers
}
