
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a System.Windows.Controls.DataGrid column that hosts System.Windows.Controls.ComboBox
    /// controls in its cells.
    /// </summary>
    class DataGridComboBoxColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
            if (b.Mode == BindingMode.OneWay)
            {
                if (!b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                {
                    b.Mode = BindingMode.TwoWay;
                }
            }
            object value = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(childData, b.Path.Path);
            if (!(value is Enum))
            {
                throw new NotImplementedException("DataGridComboBoxColumns currently only works with Enum types");
            }
            Type enumType = value.GetType();
            var enumValues = Enum.GetValues(enumType);//  enumType.GetEnumValues();
            ComboBox comboBox = new ComboBox();
            comboBox.DataContext = childData;
            if (enumValues != null && enumValues.Length > 0)
            {
#if BRIDGE
                //Note: work around the fact that Enum is not a persistant type (the value is replaced by the base type of the enum (int32 by default))
                //      because of that, we need to set the ItemsSource of the ComboBox to a list of the names (as strings) of the possible enum values.
                //      Otherwise, the value shown in the comboBox would be the base type value (so 0 or 1 for example).
                // Create a list of the enum's values' names and set it as the ItemsSource of the ComboBox:
                var enumValuesAsStrings = new List<string>();
                Type valueType = value.GetType();
                foreach (object val in enumValues)
                {
                    enumValuesAsStrings.Add(Enum.GetName(valueType, val));
                }
                comboBox.ItemsSource = enumValuesAsStrings;

                //Add a converter so that we go smoothly between the selected value in the ComboBox and the expected value in the source of the binding:
                b.Converter = new MyConverter();
                b.ConverterParameter = valueType;
#else
                comboBox.ItemsSource = enumValues;
#endif

                comboBox.SetBinding(ComboBox.SelectedItemProperty, b);
            }
            return comboBox;
        }


        internal override FrameworkElement GenerateElement(object childData)
        {
            TextBlock textBlock = new TextBlock();
            //textBlock.DataContext = childData;
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
            if (b != null)
            {
                if (b.Mode == BindingMode.OneWay)
                {
                    if (!b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                    {
                        b.Mode = BindingMode.TwoWay;
                    }
                }
                textBlock.SetBinding(TextBlock.TextProperty, b);
            }
            else if (childData is string)
                textBlock.Text = (string)childData;
            return textBlock;
        }

#if BRIDGE
        private class MyConverter : IValueConverter
        {
#if MIGRATION
            public object Convert(object value, Type targetType, object parameter, Globalization.CultureInfo culture)
#else
            public object Convert(object value, Type targetType, object parameter, string language)
#endif
            {
                if (value == null)
                    return null;
                return Enum.GetName((Type)parameter, value);
                //return value.GetType().FullName;
            }

#if MIGRATION
            public object ConvertBack(object value, Type targetType, object parameter, Globalization.CultureInfo culture)
#else
            public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
            {
                if (value == null)
                    return null;
                return Enum.Parse((Type)parameter, (string)value);
            }
        }
#endif
    }
}