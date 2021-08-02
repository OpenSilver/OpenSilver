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

#if OPENSILVER
extern alias OpenSilver;
using OpenSilver::DotNetForHtml5.Core;
#endif
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        public XamlSinglePropertyEditor(PropertyInfo propertyInfo, object targetElement)
        {
            _isInitializing = true;

            InitializeComponent();

            _isInitializing = false;

            _propertyInfo = propertyInfo;
            _targetElement = targetElement;
            _propertyInfo = propertyInfo;

            Refresh();
        }

        void Refresh()
        {
            try
            {
                ButtonOK.Visibility = Visibility.Collapsed;

                // Read property name:
                PropertyNameTextBlock.Text = _propertyInfo.Name + ":";

                // Set property appearance depending on whether there exists a converter from String to the property type:
                Type propertyType = _propertyInfo.PropertyType;
#if OPENSILVER
                bool isItPossibleToConvertFromString = ObjectBuilder.Singleton.CanParse(propertyType);
#else
                bool isItPossibleToConvertFromString = IsItPossibleToConvertFromString(propertyType);
#endif

                SetIsReadOnly(!isItPossibleToConvertFromString);

                // Set "AcceptsReturn" only if it is a string:
                PropertyValueTextBox.AcceptsReturn = propertyType == typeof(string);

                // Attempt to read the property value:
                _isChangingTextProgrammatically = true;
                try
                {
                    object value = _propertyInfo.GetValue(_targetElement);
                    PropertyValueTextBox.Text = ConvertToString(value);
                }
                catch
                {
                    PropertyValueTextBox.Text = "Err";
                }
                _isChangingTextProgrammatically = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void SetIsReadOnly(bool isReadOnly)
        {
            this.PropertyValueTextBox.IsReadOnly = isReadOnly;

            if (isReadOnly)
            {
                this.PropertyValueTextBox.Opacity = 0.7d;
                this.PropertyValueTextBox.Foreground = new SolidColorBrush(Colors.White);
                this.PropertyValueTextBox.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                this.PropertyValueTextBox.Opacity = 1d;
                this.PropertyValueTextBox.Foreground = new SolidColorBrush(Colors.LightGray);
                this.PropertyValueTextBox.Background = new SolidColorBrush(Colors.Black);
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
            try
            {
                string valueAsString = PropertyValueTextBox.Text;
                object convertedValue = ConvertFromString(valueAsString, _propertyInfo.PropertyType);
                _propertyInfo.SetValue(_targetElement, convertedValue);
            }
            catch
            {
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
                // Get a reference to the "ObjectBuilder" class in the Core assembly:
                ReflectionInUserAssembliesHelper.TryGetTypeInCoreAssembly("DotNetForHtml5.Core", null, "ObjectBuilder", out var typeConverter, out var coreAssembly);

                // Call the "CanParse" method:
                var canTypeBeConvertedMethod = typeConverter.GetMethod("CanParse");
                var canTypeBeConverted = (bool)canTypeBeConvertedMethod.Invoke(null, new object[] { typeToWhichWeWouldLikeToConvert });

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
#if OPENSILVER
                var convertedValue = ObjectBuilder.Singleton.Parse(valueAsString, targetType);
#else
                // Get a reference to the "ObjectBuilder" class in the Core assembly:
                ReflectionInUserAssembliesHelper.TryGetTypeInCoreAssembly("DotNetForHtml5.Core", null, "ObjectBuilder", out var typeConverter, out var coreAssembly);

                // Call the "Parse" method:
                var convertFromInvariantStringMethod = typeConverter.GetMethod("Parse");
                var convertedValue = convertFromInvariantStringMethod.Invoke(null, new object[] { targetType, valueAsString });
#endif
                return convertedValue;
            }
        }

        string ConvertToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else if (value is double number && double.IsNaN(number))
            {
                return "Auto";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
