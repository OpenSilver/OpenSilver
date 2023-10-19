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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Tests
{
    [TestClass]
    public class SetterBaseCollectionTest
    {
        [TestMethod]
        public void SetterBaseCollection_Seal()
        {
            var sbc = new SetterBaseCollection
            {
                new Setter(FrameworkElement.MarginProperty, new Thickness(10d)),
                new Setter(Control.PaddingProperty, new Thickness(20d)),
                new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Red)),
            };

            sbc.IsSealed.Should().BeFalse();

            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            foreach (var setter in sbc)
            {
                setter.IsSealed.Should().BeTrue();
            }
        }

        #region Check for null values

        [TestMethod]
        public void SetterBaseCollection_Add_Null()
        {
            var sbc = new SetterBaseCollection();
            Assert.ThrowsException<ArgumentNullException>(() => sbc.Add(null));
        }

        [TestMethod]
        public void SetterBaseCollection_Insert_Null()
        {
            var sbc = new SetterBaseCollection();
            Assert.ThrowsException<ArgumentNullException>(() => sbc.Insert(0, null));
        }

        [TestMethod]
        public void SetterBaseCollection_Set_Item_Null()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.PaddingProperty, new Thickness(20d)));
            Assert.ThrowsException<ArgumentNullException>(() => sbc[0] = null);
        }

        #endregion Check for null values

        #region IsSealed

        [TestMethod]
        public void SetterBaseCollection_Add_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));

            Assert.ThrowsException<InvalidOperationException>(() => sbc.Add(setter));
        }

        [TestMethod]
        public void SetterBaseCollection_Clear_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            Assert.ThrowsException<InvalidOperationException>(() => sbc.Clear());
        }

        [TestMethod]
        public void SetterBaseCollection_Insert_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));

            Assert.ThrowsException<InvalidOperationException>(() => sbc.Insert(0, setter));
        }

        [TestMethod]
        public void SetterBaseCollection_RemoveAt_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.IsTabStopProperty, false));
            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            Assert.ThrowsException<InvalidOperationException>(() => sbc.RemoveAt(0));
        }

        [TestMethod]
        public void SetterBaseCollection_Remove_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));
            sbc.Add(setter);

            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            Assert.ThrowsException<InvalidOperationException>(() => sbc.Remove(setter));
        }

        [TestMethod]
        public void SetterBaseCollection_Set_Item_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.PaddingProperty, new Thickness(20d)));
            sbc.Seal();

            sbc.IsSealed.Should().BeTrue();

            var setter = new Setter(Control.IsTabStopProperty, false);

            Assert.ThrowsException<InvalidOperationException>(() => sbc[0] = setter);
        }

        #endregion IsSealed
    }
}
