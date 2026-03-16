
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
using OpenSilver;
using OpenSilver.Internal.Helpers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Tests
{
    public partial class FrameworkElementTest
    {
        [TestMethod]
        public void FE_Set_Style_Invalid_TargetType()
        {
            var fe = new TestFE1();

            var style = new Style(typeof(Button));

            Assert.Throws<InvalidOperationException>(() => fe.Style = style);
        }

        [TestMethod]
        public void FE_Set_Style_Check_IsSealed()
        {
            var fe = new TestFE1();

            var style = new Style(typeof(TestFE1));

            Assert.IsFalse(style.IsSealed);

            fe.Style = style;

            Assert.IsTrue(style.IsSealed);
        }

        [TestMethod]
        public void FE_Set_Style_Not_Null()
        {
            var fe = TestFeWithStyle();

            Assert.AreEqual(55d, fe.Prop1);
            Assert.IsNull(fe.Prop2);
        }

        [TestMethod]
        public void FE_Set_Style_Null()
        {
            var fe = TestFeWithStyle();

            fe.Style = null;

            Assert.AreEqual(fe.Prop1, (double)TestFE1.Prop1Property.GetMetadata(fe.GetType()).DefaultValue);
            Assert.AreEqual(fe.Prop2, (Brush)TestFE1.Prop2Property.GetMetadata(fe.GetType()).DefaultValue);
        }

        [TestMethod]
        public void FE_Set_Style_Not_Null_From_Not_Null()
        {
            var fe = TestFeWithStyle();

            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, 1d));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(-10d)));

            fe.Style = style;

            Assert.AreEqual(1d, fe.Prop1);
            Assert.AreEqual(fe.Prop2, (Brush)TestFE1.Prop2Property.GetMetadata(fe.GetType()).DefaultValue);
            Assert.AreEqual(fe.Margin, new Thickness(-10d));
        }

        [TestMethod]
        public void FE_Set_Style_When_Setter_Value_Is_Binding()
        {
            var fe = new TestFE1();
            fe.DataContext = 100d;

            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, new Binding()));

            fe.Style = style;

            Assert.AreEqual(100d, fe.Prop1);
        }

        [TestMethod]
        public void FE_Set_Style_When_Setter_Value_Is_Binding_And_Share_Style()
        {
            var fe1 = new TestFE1();
            fe1.DataContext = 100d;

            var fe2 = new TestFE1();
            fe2.DataContext = 200d;

            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, new Binding()));

            fe1.Style = style;

            Assert.AreEqual(100d, fe1.Prop1);

            fe2.Style = style;

            Assert.AreEqual(200d, fe2.Prop1);
        }

        [TestMethod]
        public void FE_Should_Apply_Implicit_Style()
        {
            var border = new Border();
            var fe1 = new TestFE1();
            border.Child = fe1;
            var implicitStyle = new Style(typeof(TestFE1));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop1Property, 100d));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Black)));
            border.Resources.Add(typeof(TestFE1), implicitStyle);

            using (var wrapper = new FocusableControlWrapper<Border>(border))
            {
                Assert.IsNull(fe1.Style);
                Assert.AreSame(fe1.ImplicitStyle, implicitStyle);
                Assert.AreEqual(100d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Black);
            }
        }

        [TestMethod]
        public void FE_Should_Not_Apply_Implicit_Style_1()
        {
            var border = new Border();
            var fe1 = new TestFE1
            {
                Style = null,
            };
            border.Child = fe1;
            var implicitStyle = new Style(typeof(TestFE1));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop1Property, 100d));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Pink)));
            border.Resources.Add(typeof(TestFE1), implicitStyle);

            using (var wrapper = new FocusableControlWrapper<Border>(border))
            {
                Assert.IsNull(fe1.Style);
                Assert.AreSame(fe1.ImplicitStyle, implicitStyle);
                Assert.AreEqual(42d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Red);
            }
        }

        [TestMethod]
        public void FE_Should_Not_Apply_Implicit_Style_2()
        {
            var border = new Border();
            var fe1 = new TestFE1();
            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, 200d));
            fe1.Style = style;
            border.Child = fe1;
            var implicitStyle = new Style(typeof(TestFE1));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop1Property, 100d));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Orange)));
            border.Resources.Add(typeof(TestFE1), implicitStyle);

            using (var wrapper = new FocusableControlWrapper<Border>(border))
            {
                Assert.AreSame(fe1.Style, style);
                Assert.AreSame(fe1.ImplicitStyle, implicitStyle);
                Assert.AreEqual(200d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Red);
            }
        }

        [TestMethod]
        public void Implicit_Style_Should_Be_Applied_After_ClearValue()
        {
            var border = new Border();
            var fe1 = new TestFE1();
            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, 200d));
            fe1.Style = style;
            border.Child = fe1;
            var implicitStyle = new Style(typeof(TestFE1));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop1Property, 100d));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Green)));
            border.Resources.Add(typeof(TestFE1), implicitStyle);

            using (var wrapper = new FocusableControlWrapper<Border>(border))
            {
                Assert.AreSame(fe1.Style, style);
                Assert.AreSame(fe1.ImplicitStyle, implicitStyle);
                Assert.AreEqual(200d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Red);

                fe1.ClearValue(FrameworkElement.StyleProperty);

                Assert.AreEqual(100d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Green);
            }
        }

        [TestMethod]
        public void Local_Style_Should_Clear_Implicit_Style()
        {
            var border = new Border();
            var fe1 = new TestFE1();
            border.Child = fe1;
            var implicitStyle = new Style(typeof(TestFE1));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop1Property, 100d));
            implicitStyle.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Yellow)));
            border.Resources.Add(typeof(TestFE1), implicitStyle);

            using (var wrapper = new FocusableControlWrapper<Border>(border))
            {
                Assert.IsNull(fe1.Style);
                Assert.AreSame(fe1.ImplicitStyle, implicitStyle);
                Assert.AreEqual(100d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Yellow);

                var style = new Style(typeof(TestFE1));
                style.Setters.Add(new Setter(TestFE1.Prop2Property, new SolidColorBrush(Colors.Blue)));
                fe1.Style = style;

                Assert.AreEqual(42d, fe1.Prop1);
                Assert.IsInstanceOfType<SolidColorBrush>(fe1.Prop2);
                Assert.AreEqual(fe1.Prop2.As<SolidColorBrush>().Color, Colors.Blue);
            }
        }
    }
}
