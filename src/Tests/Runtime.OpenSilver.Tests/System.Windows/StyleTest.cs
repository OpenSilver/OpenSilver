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

namespace System.Windows.Tests
{
    [TestClass]
    public class StyleTest
    {
        [TestMethod]
        public void Set_TargetType_To_Null()
        {
            var s = new Style();
            Assert.Throws<ArgumentNullException>(() => s.TargetType = null);
        }

        [TestMethod]
        public void BasedOn_Circular_Reference()
        {
            var s1 = new Style(typeof(ContentControl));
            var s2 = new Style(typeof(ContentControl));
            var s3 = new Style(typeof(ContentControl));
            s1.BasedOn = s2;
            s2.BasedOn = s3;
            s3.BasedOn = s1;

            Assert.Throws<InvalidOperationException>(() => s1.Seal());
        }

        [TestMethod]
        public void BasedOn_Self()
        {
            var s = new Style(typeof(FrameworkElement));

            Assert.Throws<ArgumentException>(() => s.BasedOn = s);
        }

        [TestMethod]
        public void CheckTargetType_When_Not_TargetType_IsInstanceOfType_Target()
        {
            var s = new Style(typeof(ItemsControl));
            var b = new Button();

            Assert.Throws<InvalidOperationException>(() => s.CheckTargetType(b));
        }

        [TestMethod]
        public void CheckTargetType_When_TargetType_Is_Null()
        {
            var s = new Style();
            var cc = new ContentControl();

            Assert.IsNull(s.TargetType);
            Assert.Throws<InvalidOperationException>(() => s.CheckTargetType(cc));
        }

        [TestMethod]
        public void Set_TargetType_When_Sealed()
        {
            var s = new Style(typeof(ContentControl));
            s.Seal();

            Assert.Throws<InvalidOperationException>(() => s.TargetType = typeof(ItemsControl));
        }

        [TestMethod]
        public void Set_BasedOn_When_Sealed()
        {
            var s1 = new Style(typeof(TextBlock));
            var s2 = new Style(typeof(FrameworkElement));
            s1.BasedOn = s2;
            s1.Seal();

            Assert.Throws<InvalidOperationException>(() => s1.BasedOn = new Style(typeof(DependencyObject)));
        }

        [TestMethod]
        public void Seal_When_BasedOn_Is_Null()
        {
            var s1 = new Style(typeof(ContentControl));
            s1.Setters.Add(new Setter(FrameworkElement.HeightProperty, 40d));
            s1.Setters.Add(new Setter(FrameworkElement.WidthProperty, 20d));
            var contentTemplate = new DataTemplate();
            s1.Setters.Add(new Setter(ContentControl.ContentTemplateProperty, contentTemplate));

            s1.Seal();

            Assert.IsTrue(s1.IsSealed);
            Assert.IsTrue(s1.Setters.IsSealed);

            // Check s1.EffectiveValues
            var effectiveValues = s1.EffectiveValues;

            Assert.HasCount(3, effectiveValues);
            Assert.IsTrue(effectiveValues.ContainsKey(FrameworkElement.HeightProperty.GlobalIndex));
            Assert.AreEqual(40d, effectiveValues[FrameworkElement.HeightProperty.GlobalIndex]);
            Assert.IsTrue(effectiveValues.ContainsKey(FrameworkElement.WidthProperty.GlobalIndex));
            Assert.AreEqual(20d, effectiveValues[FrameworkElement.WidthProperty.GlobalIndex]);
            Assert.IsTrue(effectiveValues.ContainsKey(ContentControl.ContentTemplateProperty.GlobalIndex));
            Assert.AreEqual(effectiveValues[ContentControl.ContentTemplateProperty.GlobalIndex], contentTemplate);
        }

        [TestMethod]
        public void Seal_When_BasedOn_Is_Not_Null()
        {
            var s1 = new Style(typeof(ListBox));
            s1.Setters.Add(new Setter(ListBox.SelectionModeProperty, SelectionMode.Extended));
            s1.Setters.Add(new Setter(ItemsControl.DisplayMemberPathProperty, "DisplayMemberPathListBox"));
            var s2 = new Style(typeof(ItemsControl));
            s2.Setters.Add(new Setter(ItemsControl.DisplayMemberPathProperty, "DisplayMemberPathItemsControl"));
            var itemTemplate = new DataTemplate();
            s2.Setters.Add(new Setter(ItemsControl.ItemTemplateProperty, itemTemplate));
            s2.Setters.Add(new Setter(FrameworkElement.WidthProperty, 22d));
            var s3 = new Style(typeof(ItemsControl));
            s3.Setters.Add(new Setter(FrameworkElement.HeightProperty, 40d));
            s3.Setters.Add(new Setter(FrameworkElement.WidthProperty, 44d));
            s1.BasedOn = s2;
            s2.BasedOn = s3;

            s1.Seal();

            Assert.IsTrue(s1.IsSealed);
            Assert.IsTrue(s2.IsSealed);
            Assert.IsTrue(s3.IsSealed);
            Assert.IsTrue(s1.Setters.IsSealed);
            Assert.IsTrue(s2.Setters.IsSealed);
            Assert.IsTrue(s3.Setters.IsSealed);

            var effectiveValues = s1.EffectiveValues;

            Assert.HasCount(5, effectiveValues);
            Assert.IsTrue(effectiveValues.ContainsKey(ListBox.SelectionModeProperty.GlobalIndex));
            Assert.AreEqual(SelectionMode.Extended, effectiveValues[ListBox.SelectionModeProperty.GlobalIndex]);
            Assert.IsTrue(effectiveValues.ContainsKey(ItemsControl.DisplayMemberPathProperty.GlobalIndex));
            Assert.AreEqual("DisplayMemberPathListBox", effectiveValues[ItemsControl.DisplayMemberPathProperty.GlobalIndex]);
            Assert.IsTrue(effectiveValues.ContainsKey(ItemsControl.ItemTemplateProperty.GlobalIndex));
            Assert.AreEqual(effectiveValues[ItemsControl.ItemTemplateProperty.GlobalIndex], itemTemplate);
            Assert.IsTrue(effectiveValues.ContainsKey(FrameworkElement.WidthProperty.GlobalIndex));
            Assert.AreEqual(22d, effectiveValues[FrameworkElement.WidthProperty.GlobalIndex]);
            Assert.IsTrue(effectiveValues.ContainsKey(FrameworkElement.HeightProperty.GlobalIndex));
            Assert.AreEqual(40d, effectiveValues[FrameworkElement.HeightProperty.GlobalIndex]);
        }
    }
}