﻿
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
using System.Collections;
using System.Collections.ObjectModel;
using OpenSilver;

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
    public partial class FrameworkElementTest
    {
        #region Logical Tree

        [TestMethod]
        public void FE_AddLogicalChild_Self()
        {
            var fe = new TestFE2();

            Assert.ThrowsException<InvalidOperationException>(() => fe.Children.Add(fe));
        }

        [TestMethod]
        public void FE_AddLogicalChild_Parent()
        {
            var parent = new TestFE2();
            var child = new TestFE1();
            parent.Children.Add(child);

            parent.Children[0].Parent.Should().Be(parent);
        }

        [TestMethod]
        public void FE_AddLogicalChild_When_Child_Already_Has_Logical_Parent()
        {
            var parent = new TestFE2();
            var child = new TestFE1();
            parent.Children.Add(child);
            var newParent = new TestFE1();

            Assert.ThrowsException<InvalidOperationException>(() => newParent.AddLogicalChild(child));
        }

        [TestMethod]
        public void FE_AddLogicalChild_Inherit_Properties()
        {
            var parent = new TestFE2();
            parent.Inherits = 42d;
            var child = new TestFE2();
            parent.Children.Add(child);

            child.Inherits.Should().Be(42d);
        }

        [TestMethod]
        public void FE_RemoveLogicalChild_Parent()
        {
            var parent = new TestFE2();
            var child = new TestFE1();
            parent.Children.Add(child);

            child.Parent.Should().Be(parent);

            parent.Children.Remove(child);

            child.Parent.Should().BeNull();
        }

        [TestMethod]
        public void FE_RemoveLogicalChild_When_Child_Is_Not_LogicalChild()
        {
            var parent = new TestFE2();
            var child = new TestFE1();
            parent.Children.Add(child);
            var otherParent = new TestFE1();
            otherParent.RemoveLogicalChild(child);

            child.Parent.Should().Be(parent);
        }

        [TestMethod]
        public void FE_RemoveLogicalChild_Inherited_Properties()
        {
            var parent = new TestFE2();
            parent.Inherits = 42d;
            var child = new TestFE2();
            parent.Children.Add(child);

            child.Inherits.Should().Be(42d);

            parent.Children.Remove(child);

            child.Inherits.Should().Be((double)TestFE2.InheritsProperty.GetMetadata(child.GetType()).DefaultValue);
        }

        #endregion Logical Tree

        private static TestFE1 TestFeWithStyle()
        {
            var fe = new TestFE1();

            var style = new Style(typeof(TestFE1));
            style.Setters.Add(new Setter(TestFE1.Prop1Property, 55d));
            style.Setters.Add(new Setter(TestFE1.Prop2Property, null));

            fe.Style = style;

            return fe;
        }
    }

    #region Helpers

    public class TestFE1 : FrameworkElement
    {
        public static readonly DependencyProperty Prop1Property =
            DependencyProperty.Register(
                nameof(Prop1),
                typeof(double),
                typeof(TestFE1),
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
                typeof(TestFE1),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush Prop2
        {
            get => (Brush)GetValue(Prop2Property);
            set => SetValue(Prop2Property, value);
        }
    }

    public class TestFE2 : FrameworkElement
    {
        public TestFE2()
        {
            Children = new LogicalChildrenCollection(this);
        }

        public Collection<FrameworkElement> Children { get; }

        public static readonly DependencyProperty InheritsProperty =
            DependencyProperty.Register(
                nameof(Inherits),
                typeof(double),
                typeof(TestFE2),
                new PropertyMetadata(0d) { Inherits = true });

        public double Inherits
        {
            get { return (double)GetValue(InheritsProperty); }
            set { SetValue(InheritsProperty, value); }
        }

        internal override IEnumerator LogicalChildren
            => Children.GetEnumerator();

        #region Private classes

        private class LogicalChildrenCollection : Collection<FrameworkElement>
        {
            public FrameworkElement Parent { get; }

            public LogicalChildrenCollection(FrameworkElement parent)
            {
                Parent = parent;
            }

            protected override void ClearItems()
            {
                foreach (var item in Items)
                {
                    Parent.RemoveLogicalChild(item);
                }
                base.ClearItems();
            }

            protected override void InsertItem(int index, FrameworkElement item)
            {
                if (index >= 0 && index <= Count)
                {
                    Parent.AddLogicalChild(item);
                }
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                if (index >= 0 && index < Count)
                {
                    Parent.RemoveLogicalChild(Items[index]);
                }
                base.RemoveItem(index);
            }
            
            protected override void SetItem(int index, FrameworkElement item)
            {
                if (index >= 0 && index < Count)
                {
                    Parent.RemoveLogicalChild(Items[index]);
                    Parent.AddLogicalChild(item);
                }
                base.SetItem(index, item);
            }
        }

        #endregion Private classes
    }

    #endregion Helpers
}
