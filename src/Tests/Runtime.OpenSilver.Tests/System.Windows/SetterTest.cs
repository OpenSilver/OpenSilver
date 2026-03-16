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
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows.Tests
{
    [TestClass]
    public class SetterTest
    {
        #region Setter.Value

        [TestMethod]
        public void Setter_Set_Value_To_DependencyProperty_UnsetValue()
        {
            var setter = new Setter(Control.IsTabStopProperty, false);
            setter.Value = DependencyProperty.UnsetValue;

            Assert.IsNull(setter.Value);
        }

        #endregion region Setter.Value

        #region Setter.Property

        [TestMethod]
        public void Setter_Set_Property_To_Null()
        {
            var setter = new Setter();

            Assert.Throws<ArgumentNullException>(() => setter.Property = null);
        }

        [TestMethod]
        public void Setter_Set_Property_To_FrameworkElement_NameProperty()
        {
            var setter = new Setter();

            Assert.Throws<InvalidOperationException>(() => setter.Property = FrameworkElement.NameProperty);
        }

        #endregion Setter.Property

        #region Constructor(DependencyProperty, object)

        [TestMethod]
        public void Setter_Constructor_When_Property_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Setter(null, new object()));
        }

        [TestMethod]
        public void Setter_Constructor_When_Property_Is_FrameworkElement_NameProperty()
        {
            Assert.Throws<InvalidOperationException>(() => new Setter(FrameworkElement.NameProperty, "name"));
        }

        [TestMethod]
        public void Setter_Constuctor_When_Value_Is_DependencyProperty_UnsetValue()
        {
            var setter = new Setter(Border.BackgroundProperty, DependencyProperty.UnsetValue);

            Assert.IsNull(setter.Value);
        }

        #endregion Constructor(DependencyProperty, object)

        #region IsSealed

        [TestMethod]
        public void Setter_IsSealed()
        {
            var setter = new Setter(UIElement.IsHitTestVisibleProperty, true);

            Assert.IsFalse(setter.IsSealed);
            
            setter.Seal();

            Assert.IsTrue(setter.IsSealed);
        }

        [TestMethod]
        public void Setter_Set_Value_When_IsSealed()
        {
            var setter = new Setter(Control.BorderThicknessProperty, new Thickness(2d));
            setter.Seal();

            Assert.Throws<InvalidOperationException>(() => setter.Value = new Thickness(4d));
        }

        [TestMethod]
        public void Setter_Set_Property_When_IsSealed()
        {
            var setter = new Setter(Control.BorderThicknessProperty, new Thickness(2d));
            setter.Seal();

            Assert.Throws<InvalidOperationException>(() => setter.Property = Control.IsTabStopProperty);
        }

        #endregion IsSealed

        #region Seal

        [TestMethod]
        public void Setter_Seal_When_Property_Is_Null()
        {
            var setter = new Setter();
            setter.Value = "test";

            Assert.Throws<ArgumentException>(() => setter.Seal());
        }

        [TestMethod]
        public void Setter_Seal_When_Value_Is_Invalid()
        {
            var setter = new Setter(Border.BorderBrushProperty, "test");
            
            Assert.Throws<ArgumentException>(() => setter.Seal());
        }

        #endregion Seal
    }
}
