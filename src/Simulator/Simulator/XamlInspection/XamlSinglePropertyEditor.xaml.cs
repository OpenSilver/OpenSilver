

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetForHtml5.EmulatorWithoutJavascript.XamlInspection
{
    /// <summary>
    /// Interaction logic for XamlSinglePropertyEditor.xaml
    /// </summary>
    public partial class XamlSinglePropertyEditor : UserControl
    {
        PropertyInfo _propertyInfo;
        object _targetElement;
        bool _isInitializing;
        bool _isChangingTextProgrammatically;
        bool _isReadOnly;

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                if (value)
                {
                    ButtonOK.Visibility = Visibility.Collapsed;

                }
                else
                {
                    ButtonOK.Content = "OK";
                    ButtonOK.Visibility = Visibility.Collapsed;
                }
                _isReadOnly = value;
            }
        }
        public XamlSinglePropertyEditor(PropertyInfo propertyInfo, object targetElement, bool isReadOnly)
        {
            _isInitializing = true;

            InitializeComponent();

            _isInitializing = false;
            IsReadOnly = isReadOnly;

            _propertyInfo = propertyInfo;
            _targetElement = targetElement;
            _propertyInfo = propertyInfo;

            Refresh();
        }

        public void Refresh()
        {
            try
            {
                Type propertyType = _propertyInfo.PropertyType;
                this.PropertyNameTextBlock.Text = _propertyInfo.Name + ":";
                if (!IsReadOnly)
                {
                    ButtonOK.Visibility = Visibility.Collapsed;

                    // Read property name:

                    // Set "AcceptsReturn" only if it is a string:
                    bool isString = (propertyType == typeof(string));
                    this.PropertyValueTextBox.AcceptsReturn = isString;
                }
                else
                {
                    PropertyValueTextBox.IsReadOnly = IsReadOnly;
                }

                // Set property appearance depending on whether there exists a converter from String to the property type:
                bool isItPossibleToConvertFromString = IsItPossibleToConvertFromString(propertyType);
                SetPropertyValueStyle(isItPossibleToConvertFromString);

                // Attempt to read the property value:
                _isChangingTextProgrammatically = true;
                try
                {
                    object value = _propertyInfo.GetValue(_targetElement);
                    this.PropertyValueTextBox.Text = ConvertToString(value);
                }
                catch (Exception)
                {
                    this.PropertyValueTextBox.Text = "Err";
                }
                _isChangingTextProgrammatically = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void SetPropertyValueStyle(bool isConvertibleFromString)
        {
            this.PropertyValueTextBox.IsReadOnly = IsReadOnly || !isConvertibleFromString;

            if (isConvertibleFromString)
            {
                this.PropertyValueTextBox.Opacity = 1d;
                this.PropertyValueTextBox.Foreground = new SolidColorBrush(Colors.LightGray);
                this.PropertyValueTextBox.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                this.PropertyValueTextBox.Opacity = 0.7d;
                this.PropertyValueTextBox.Foreground = new SolidColorBrush(Colors.White);
                this.PropertyValueTextBox.Background = new SolidColorBrush(Colors.Transparent);
                ButtonOK.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            ApplyChange();
        }

        private void PropertyValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApplyChange();
        }

        private void PropertyValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing && !_isChangingTextProgrammatically)
            {
                ButtonOK.Visibility = Visibility.Visible;
            }
        }

        void ApplyChange()
        {
            // Attempt to set the property value:
            if (!IsReadOnly)
            {
                try
                {
                    string valueAsString = PropertyValueTextBox.Text;
                    object convertedValue = ConvertFromString(valueAsString, _propertyInfo.PropertyType);
                    _propertyInfo.SetValue(_targetElement, convertedValue);
                }
                catch
                {
                }
            }
            Refresh();
        }

        bool IsItPossibleToConvertFromString(Type typeToWhichWeWouldLikeToConvert)
        {
            // If it is a string or an Enum, we can convert from string, otherwise, we need to look for a converter in the Core assembly:
            if (typeToWhichWeWouldLikeToConvert == typeof(string)
                || typeToWhichWeWouldLikeToConvert.IsEnum)
            {
                return true;
            }
            else
            {
                // Get a reference to the "TypeFromStringConverters" class in the Core assembly:
                Assembly coreAssembly;
                Type typeFromStringConverter;
                ReflectionInUserAssembliesHelper.TryGetTypeInCoreAssembly("DotNetForHtml5.Core", null, "TypeFromStringConverters", out typeFromStringConverter, out coreAssembly);

                // Call the "CanTypeBeConverted" method:
                MethodInfo canTypeBeConvertedMethod = typeFromStringConverter.GetMethod("CanTypeBeConverted");
                bool canTypeBeConverted = (bool)canTypeBeConvertedMethod.Invoke(null, new object[] { typeToWhichWeWouldLikeToConvert });

                return canTypeBeConverted;
            }
        }

        object ConvertFromString(string valueAsString, Type targetType)
        {
            // If it is a string or an Enum, convert directly, otherwise call the appropriate converter from the Core assembly:
            if (targetType == typeof(string))
            {
                return valueAsString;
            }
            else if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, valueAsString);
            }
            else
            {
                // Get a reference to the "TypeFromStringConverters" class in the Core assembly:
                Type typeFromStringConverter;
                Assembly coreAssembly;
                ReflectionInUserAssembliesHelper.TryGetTypeInCoreAssembly("DotNetForHtml5.Core", null, "TypeFromStringConverters", out typeFromStringConverter, out coreAssembly);

                // Call the "ConvertFromInvariantString" method:
                MethodInfo convertFromInvariantStringMethod = typeFromStringConverter.GetMethod("ConvertFromInvariantString");
                object convertedValue = convertFromInvariantStringMethod.Invoke(null, new object[] { targetType, valueAsString });

                return convertedValue;
            }
        }

        string ConvertToString(object value)
        {
            if (value == null)
            {
                return "";
            }
            else if (value is double && double.IsNaN((double)value))
            {
                return "Auto";
            }
            else
                return value.ToString();
        }
    }
}
