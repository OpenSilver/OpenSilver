

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
using System.Collections.Generic;
using CSHTML5.Native.Html.Controls;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a <see cref="DataGrid"/> column that hosts <see cref="ComboBox"/>
    /// controls in its cells.
    /// </summary>
    internal partial class DataGridComboBoxColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            Binding b = GetBinding();
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
            NativeComboBox cb = new NativeComboBox();
            cb.DataContext = childData;
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
                cb.ItemsSource = enumValuesAsStrings;

                //Add a converter so that we go smoothly between the selected value in the ComboBox and the expected value in the source of the binding:
                b.Converter = new MyConverter();
                b.ConverterParameter = valueType;
#else
                cb.ItemsSource = enumValues;
#endif

                cb.SetBinding(NativeComboBox.SelectedItemProperty, b);
            }
            return cb;
        }
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(dataItem);
        }

        private FrameworkElement GenerateElement(object childData)
        {
            TextBlock textBlock = new TextBlock();
            //textBlock.DataContext = childData;
            Binding b = GetBinding();
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
        private partial class MyConverter : IValueConverter
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