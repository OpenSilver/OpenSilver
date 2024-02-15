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

namespace System.Windows.Tests
{
    [TestClass]
    public class StyleTest
    {
        [TestMethod]
        public void Set_TargetType_To_Null()
        {
            var s = new Style();
            Assert.ThrowsException<ArgumentNullException>(() => s.TargetType = null);
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

            Assert.ThrowsException<InvalidOperationException>(() => s1.Seal());
        }

        [TestMethod]
        public void BasedOn_Self()
        {
            var s = new Style(typeof(FrameworkElement));

            Assert.ThrowsException<ArgumentException>(() => s.BasedOn = s);
        }

        [TestMethod]
        public void CheckTargetType_When_Not_TargetType_IsInstanceOfType_Target()
        {
            var s = new Style(typeof(ItemsControl));
            var b = new Button();

            Assert.ThrowsException<InvalidOperationException>(() => s.CheckTargetType(b));
        }

        [TestMethod]
        public void CheckTargetType_When_TargetType_Is_Null()
        {
            var s = new Style();
            s.TargetType.Should().BeNull();
            var cc = new ContentControl();

            Assert.ThrowsException<InvalidOperationException>(() => s.CheckTargetType(cc));
        }

        [TestMethod]
        public void Set_TargetType_When_Sealed()
        {
            var s = new Style(typeof(ContentControl));
            s.Seal();

            Assert.ThrowsException<InvalidOperationException>(() => s.TargetType = typeof(ItemsControl));
        }

        [TestMethod]
        public void Set_BasedOn_When_Sealed()
        {
            var s1 = new Style(typeof(TextBlock));
            var s2 = new Style(typeof(FrameworkElement));
            s1.BasedOn = s2;
            s1.Seal();

            Assert.ThrowsException<InvalidOperationException>(() => s1.BasedOn = new Style(typeof(DependencyObject)));
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

            s1.IsSealed.Should().BeTrue();
            s1.Setters.IsSealed.Should().BeTrue();

            // Check s1.EffectiveValues
            var effectiveValues = s1.EffectiveValues;
            effectiveValues.Count.Should().Be(3);
            effectiveValues.ContainsKey(FrameworkElement.HeightProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[FrameworkElement.HeightProperty.GlobalIndex].Should().Be(40d);
            effectiveValues.ContainsKey(FrameworkElement.WidthProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[FrameworkElement.WidthProperty.GlobalIndex].Should().Be(20d);
            effectiveValues.ContainsKey(ContentControl.ContentTemplateProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[ContentControl.ContentTemplateProperty.GlobalIndex].Should().Be(contentTemplate);
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

            s1.IsSealed.Should().BeTrue();
            s2.IsSealed.Should().BeTrue();
            s3.IsSealed.Should().BeTrue();
            s1.Setters.IsSealed.Should().BeTrue();
            s2.Setters.IsSealed.Should().BeTrue();
            s3.Setters.IsSealed.Should().BeTrue();

            var effectiveValues = s1.EffectiveValues;
            effectiveValues.Count.Should().Be(5);
            effectiveValues.ContainsKey(ListBox.SelectionModeProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[ListBox.SelectionModeProperty.GlobalIndex].Should().Be(SelectionMode.Extended);
            effectiveValues.ContainsKey(ItemsControl.DisplayMemberPathProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[ItemsControl.DisplayMemberPathProperty.GlobalIndex].Should().Be("DisplayMemberPathListBox");
            effectiveValues.ContainsKey(ItemsControl.ItemTemplateProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[ItemsControl.ItemTemplateProperty.GlobalIndex].Should().Be(itemTemplate);
            effectiveValues.ContainsKey(FrameworkElement.WidthProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[FrameworkElement.WidthProperty.GlobalIndex].Should().Be(22d);
            effectiveValues.ContainsKey(FrameworkElement.HeightProperty.GlobalIndex).Should().BeTrue();
            effectiveValues[FrameworkElement.HeightProperty.GlobalIndex].Should().Be(40d);
        }
    }
}