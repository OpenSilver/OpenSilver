extern alias opensilver;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TestApplication.Tests.DataGrids
{
    public partial class DataGridCustomTypeProviderTest : Page
    {
        private static readonly DynamicRowType OriginalType = new DynamicRowType(
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Age", typeof(int)),
            new DynamicProperty("City", typeof(string)));

        private static readonly DynamicRowType EmailType = new DynamicRowType(
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Age", typeof(int)),
            new DynamicProperty("City", typeof(string)),
            new DynamicProperty("Email", typeof(string)));

        private readonly List<string> _generatedColumns = new List<string>();
        private ObservableCollection<DynamicRow> _rows;
        private DynamicRowType _currentType;
        private int _addedRowCount;

        public DataGridCustomTypeProviderTest()
        {
            InitializeComponent();
            Load(OriginalType);
        }

        private void Load(DynamicRowType type)
        {
            _currentType = type;
            _rows = new ObservableCollection<DynamicRow>
            {
                CreateRow("Ada", 36, "London"),
                CreateRow("Grace", 45, "New York"),
                CreateRow("Linus", 28, "Helsinki"),
            };

            _generatedColumns.Clear();
            GeneratedColumnsText.Text = string.Empty;
            ExpectedColumnsText.Text = string.Join(", ", type.Properties.Select(p => p.Name));
            CustomRowsGrid.ItemsSource = _rows;
        }

        private DynamicRow CreateRow(string name, int age, string city)
        {
            var row = new DynamicRow(_currentType);
            row.SetValue("Name", name);
            row.SetValue("Age", age);
            row.SetValue("City", city);
            if (_currentType == EmailType)
            {
                row.SetValue("Email", name.ToLowerInvariant() + "@example.com");
            }
            return row;
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            _generatedColumns.Add(e.PropertyName);
            GeneratedColumnsText.Text = string.Join(", ", _generatedColumns);
        }

        private void OnAddRowClick(object sender, RoutedEventArgs e)
        {
            _addedRowCount++;
            _rows.Add(CreateRow("Added " + _addedRowCount, 20 + _addedRowCount, "Paris"));
        }

        private void OnIncrementAgeClick(object sender, RoutedEventArgs e)
        {
            if (_rows.Count > 0)
            {
                DynamicRow first = _rows[0];
                first.SetValue("Age", (int)first.GetValue("Age") + 1);
            }
        }

        private void OnReloadWithEmailClick(object sender, RoutedEventArgs e) => Load(EmailType);

        private void OnReloadOriginalClick(object sender, RoutedEventArgs e) => Load(OriginalType);

        private sealed class DynamicRow : opensilver::System.Reflection.ICustomTypeProvider, INotifyPropertyChanged
        {
            private readonly DynamicRowType _type;
            private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

            public DynamicRow(DynamicRowType type)
            {
                _type = type;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public Type GetCustomType() => _type;

            public object GetValue(string name)
            {
                object value;
                return _values.TryGetValue(name, out value) ? value : null;
            }

            public void SetValue(string name, object value)
            {
                _values[name] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public override string ToString() => Convert.ToString(GetValue("Name"));
        }

        private sealed class DynamicRowType : TypeDelegator
        {
            private readonly DynamicPropertyInfo[] _properties;

            public DynamicRowType(params DynamicProperty[] properties)
                : base(typeof(DynamicRow))
            {
                Properties = properties;
                _properties = properties.Select(p => new DynamicPropertyInfo(this, p)).ToArray();
            }

            public IReadOnlyList<DynamicProperty> Properties { get; }

            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            {
                return IsPublicInstance(bindingAttr) ? _properties.ToArray<PropertyInfo>() : new PropertyInfo[0];
            }

            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                if (IsPublicInstance(bindingAttr))
                {
                    DynamicPropertyInfo property = _properties.FirstOrDefault(p => p.Name == name);
                    if (property != null)
                    {
                        return property;
                    }
                }

                return base.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
            }

            private static bool IsPublicInstance(BindingFlags bindingAttr)
            {
                return (bindingAttr & BindingFlags.Public) != 0 && (bindingAttr & BindingFlags.Instance) != 0;
            }
        }

        private sealed class DynamicProperty
        {
            public DynamicProperty(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; }

            public Type Type { get; }
        }

        private sealed class DynamicPropertyInfo : PropertyInfo
        {
            private readonly DynamicProperty _property;

            public DynamicPropertyInfo(Type declaringType, DynamicProperty property)
            {
                DeclaringType = declaringType;
                _property = property;
            }

            public override PropertyAttributes Attributes => PropertyAttributes.None;

            public override bool CanRead => true;

            public override bool CanWrite => true;

            public override Type PropertyType => _property.Type;

            public override string Name => _property.Name;

            public override Type DeclaringType { get; }

            public override Type ReflectedType => DeclaringType;

            public override MemberTypes MemberType => MemberTypes.Property;

            public override MethodInfo[] GetAccessors(bool nonPublic) => new MethodInfo[0];

            public override object[] GetCustomAttributes(bool inherit) => new object[0];

            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => new object[0];

            public override MethodInfo GetGetMethod(bool nonPublic) => null;

            public override ParameterInfo[] GetIndexParameters() => new ParameterInfo[0];

            public override MethodInfo GetSetMethod(bool nonPublic) => null;

            public override bool IsDefined(Type attributeType, bool inherit) => false;

            public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                return ((DynamicRow)obj).GetValue(Name);
            }

            public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                ((DynamicRow)obj).SetValue(Name, value);
            }
        }
    }
}
