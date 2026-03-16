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

            Assert.IsFalse(sbc.IsSealed);

            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);

            foreach (var setter in sbc)
            {
                Assert.IsTrue(setter.IsSealed);
            }
        }

        #region Check for null values

        [TestMethod]
        public void SetterBaseCollection_Add_Null()
        {
            var sbc = new SetterBaseCollection();

            Assert.Throws<ArgumentNullException>(() => sbc.Add(null));
        }

        [TestMethod]
        public void SetterBaseCollection_Insert_Null()
        {
            var sbc = new SetterBaseCollection();

            Assert.Throws<ArgumentNullException>(() => sbc.Insert(0, null));
        }

        [TestMethod]
        public void SetterBaseCollection_Set_Item_Null()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.PaddingProperty, new Thickness(20d)));

            Assert.Throws<ArgumentNullException>(() => sbc[0] = null);
        }

        #endregion Check for null values

        #region IsSealed

        [TestMethod]
        public void SetterBaseCollection_Add_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);

            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));

            Assert.Throws<InvalidOperationException>(() => sbc.Add(setter));
        }

        [TestMethod]
        public void SetterBaseCollection_Clear_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);
            Assert.Throws<InvalidOperationException>(() => sbc.Clear());
        }

        [TestMethod]
        public void SetterBaseCollection_Insert_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);

            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));

            Assert.Throws<InvalidOperationException>(() => sbc.Insert(0, setter));
        }

        [TestMethod]
        public void SetterBaseCollection_RemoveAt_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.IsTabStopProperty, false));
            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);
            Assert.Throws<InvalidOperationException>(() => sbc.RemoveAt(0));
        }

        [TestMethod]
        public void SetterBaseCollection_Remove_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            var setter = new Setter(Control.PaddingProperty, new Thickness(20d));
            sbc.Add(setter);

            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);
            Assert.Throws<InvalidOperationException>(() => sbc.Remove(setter));
        }

        [TestMethod]
        public void SetterBaseCollection_Set_Item_When_Sealed()
        {
            var sbc = new SetterBaseCollection();
            sbc.Add(new Setter(Control.PaddingProperty, new Thickness(20d)));
            sbc.Seal();

            Assert.IsTrue(sbc.IsSealed);

            var setter = new Setter(Control.IsTabStopProperty, false);

            Assert.Throws<InvalidOperationException>(() => sbc[0] = setter);
        }

        #endregion IsSealed
    }
}
