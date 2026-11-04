extern alias opensilver;

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
using OpenSilver.Internal.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace System.Windows.Controls.Tests
{
    [TestClass]
    public class CustomTypeProviderTest
    {
        [TestMethod]
        public void Binding_ReadsCustomProperty()
        {
            var target = new TextBlock();
            target.SetBinding(TextBlock.TextProperty, new Binding("Title"));
            target.DataContext = new DynamicRow("Hello");

            Assert.AreEqual("Hello", target.Text);
        }

        [TestMethod]
        public void Binding_NullCustomType_ReadsClrProperty()
        {
            var target = new TextBlock();
            target.SetBinding(TextBlock.TextProperty, new Binding("Label"));
            target.DataContext = new ClrRow { Label = "Kept" };

            Assert.AreEqual("Kept", target.Text);
        }

        [TestMethod]
        public void Sort_ComparesCustomProperty()
        {
            var sortFields = new SortDescriptionCollection
            {
                new SortDescription("Title", ListSortDirection.Ascending),
            };
            var comparer = new SortFieldComparer(sortFields, CultureInfo.InvariantCulture);

            Assert.IsTrue(comparer.Compare(new DynamicRow("a"), new DynamicRow("b")) < 0);
        }

        [TestMethod]
        public void DataGrid_GeneratesColumnsFromGenericCustomRows()
        {
            var rows = new ObservableCollection<DynamicRow> { null, new DynamicRow("Hello") };

            IList<string> headers = MeasureGrid(rows);

            CollectionAssert.AreEquivalent(new[] { "Title" }, headers.ToArray());
        }

        [TestMethod]
        public void DataGrid_GeneratesColumnsFromNonGenericCustomRows()
        {
            var rows = new ArrayList { new DynamicRow("Hello") };

            IList<string> headers = MeasureGrid(rows);

            CollectionAssert.AreEquivalent(new[] { "Title" }, headers.ToArray());
        }

        [TestMethod]
        public void DataGrid_GeneratesColumnsForOrdinaryItems()
        {
            var rows = new ObservableCollection<Person> { new Person { Name = "Ada", Age = 36 } };

            IList<string> headers = MeasureGrid(rows);

            CollectionAssert.AreEquivalent(new[] { "Name", "Age" }, headers.ToArray());
        }

        [TestMethod]
        public void DataGrid_EmptyCustomCollection_DoesNotThrow()
        {
            var rows = new ObservableCollection<DynamicRow>();

            IList<string> headers = MeasureGrid(rows);

            Assert.AreEqual(0, headers.Count);
        }

        [TestMethod]
        public void CollectionView_UsesCustomProperties()
        {
            var view = new ListCollectionView(new ObservableCollection<DynamicRow> { new DynamicRow("Hello") });

            CollectionAssert.AreEquivalent(new[] { "Title" }, view.ItemProperties.Select(info => info.Name).ToArray());
        }

        [TestMethod]
        public void CollectionView_NullCustomType_UsesClrProperties()
        {
            var view = new ListCollectionView(new ObservableCollection<ClrRow> { new ClrRow { Label = "Kept" } });

            CollectionAssert.Contains(view.ItemProperties.Select(info => info.Name).ToList(), "Label");
        }

        [TestMethod]
        public void Binding_ReadsDependencyPropertyThroughCustomType()
        {
            var source = new CustomElement { Caption = "FromDp" };
            var target = new TextBlock();
            target.SetBinding(TextBlock.TextProperty, new Binding("Caption"));
            target.DataContext = source;

            Assert.AreEqual("FromDp", target.Text);
        }

        [TestMethod]
        public void DataGrid_NullCustomType_UsesClrProperties()
        {
            var rows = new ObservableCollection<ClrRow> { new ClrRow { Label = "Kept" } };

            IList<string> headers = MeasureGrid(rows);

            CollectionAssert.AreEquivalent(new[] { "Label" }, headers.ToArray());
        }

        private static IList<string> MeasureGrid(IEnumerable source)
        {
            var grid = new MeasurableDataGrid
            {
                AutoGenerateColumns = true,
                ItemsSource = source,
                Width = 400,
                Height = 200,
            };
            // Measure returns before layout when the grid is outside the visual tree.
            grid.MeasureOverride(new Size(400, 200));
            return grid.Columns.Select(column => column.Header).OfType<string>().ToList();
        }

        private sealed class MeasurableDataGrid : DataGrid
        {
            public new Size MeasureOverride(Size availableSize) => base.MeasureOverride(availableSize);
        }

        private sealed class CustomElement : DependencyObject, opensilver::System.Reflection.ICustomTypeProvider
        {
            public static readonly DependencyProperty CaptionProperty =
                DependencyProperty.Register(nameof(Caption), typeof(string), typeof(CustomElement), null);

            public string Caption
            {
                get => (string)GetValue(CaptionProperty);
                set => SetValue(CaptionProperty, value);
            }

            public Type GetCustomType() => CustomElementType.Instance;

            private sealed class CustomElementType : TypeDelegator
            {
                private CustomElementType()
                    : base(typeof(CustomElement))
                {
                }

                public static CustomElementType Instance { get; } = new CustomElementType();

                public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => Array.Empty<PropertyInfo>();

                protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => null;
            }
        }

        private sealed class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        private sealed class ClrRow : opensilver::System.Reflection.ICustomTypeProvider
        {
            public string Label { get; set; }

            public Type GetCustomType() => null;
        }

        private sealed class DynamicRow : opensilver::System.Reflection.ICustomTypeProvider
        {
            public DynamicRow(string title)
            {
                TitleValue = title;
            }

            public readonly string TitleValue;

            public Type GetCustomType() => DynamicRowType.Instance;
        }

        private sealed class DynamicRowType : TypeDelegator
        {
            private readonly TitleProperty _title;

            private DynamicRowType()
                : base(typeof(DynamicRow))
            {
                _title = new TitleProperty(this);
            }

            public static DynamicRowType Instance { get; } = new DynamicRowType();

            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            {
                if ((bindingAttr & (BindingFlags.Public | BindingFlags.Instance)) == (BindingFlags.Public | BindingFlags.Instance))
                {
                    return new PropertyInfo[] { _title };
                }

                return Array.Empty<PropertyInfo>();
            }

            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                if (name == "Title"
                    && (bindingAttr & BindingFlags.Public) != 0
                    && (bindingAttr & BindingFlags.Instance) != 0)
                {
                    return _title;
                }

                return base.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
            }
        }

        private sealed class TitleProperty : PropertyInfo
        {
            public TitleProperty(Type declaringType)
            {
                DeclaringType = declaringType;
            }

            public override PropertyAttributes Attributes => PropertyAttributes.None;

            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override Type PropertyType => typeof(string);

            public override string Name => "Title";

            public override Type DeclaringType { get; }

            public override Type ReflectedType => DeclaringType;

            public override MemberTypes MemberType => MemberTypes.Property;

            public override MethodInfo[] GetAccessors(bool nonPublic) => Array.Empty<MethodInfo>();

            public override object[] GetCustomAttributes(bool inherit) => Array.Empty<object>();

            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => Array.Empty<object>();

            public override MethodInfo GetGetMethod(bool nonPublic) => null;

            public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();

            public override MethodInfo GetSetMethod(bool nonPublic) => null;

            public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                return ((DynamicRow)obj).TitleValue;
            }

            public override bool IsDefined(Type attributeType, bool inherit) => false;

            public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
