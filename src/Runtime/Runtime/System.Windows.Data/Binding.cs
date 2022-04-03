
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
using System.ComponentModel;
using System.Windows.Markup;

#if MIGRATION
using System.Globalization;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Defines a binding that connects the properties of binding targets and data sources.
    /// </summary>
    /// <example>
    /// You can add a Binding using XAML as follows:
    /// <code lang="XAML" xml:space="preserve" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <TextBlock x:Name="MyTextBlock"
    ///            Text="{Binding Size}"
    ///            HorizontalAlignment="Left"/>
    /// </code>
    /// <code lang="C#">
    /// MyTextBlock.DataContext = myPlanet;
    /// </code>
    /// Note: you can create the Binding directly using C#:
    /// <code lang="C#">
    /// Binding myBinding = new Binding(&quot;Size&quot;);
    /// MyTextBlock.SetBinding(TextBlock.TextProperty, myBinding);
    /// MyTextBlock.DataContext = myPlanet;
    /// </code>
    /// Here is another example using the TwoWay mode, and also using a Converter:
    /// <code lang="C#">
    /// MyTextBox.DataContext = myPlanet;
    /// </code>
    /// <code lang="XAML" xml:space="preserve" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:local="using:MyAssembly.MyApp">
    /// <Border>
    /// <!-- We define the Converter: -->
    ///     <Border.Resources>
    ///         <local:KilometersToMilesConverter x:Key="KilometersToMilesConverter"/>
    ///     </Border.Resources>
    ///     <TextBox x:Name="MyTextBox"
    ///              Text="{Binding Length, Mode=TwoWay, Converter={StaticResource KilometersToMilesConverter}}"
    ///              HorizontalAlignment="Left"/>
    /// </Border>
    /// </code>
    /// Using C#:
    /// <code lang="C#">
    /// Binding myBinding = new Binding(&quot;Size&quot;);
    /// 
    /// //we set the binding in TwoWay mode:
    /// myBinding.Mode = BindingMode.TwoWay;
    /// 
    /// //We create the converter:
    /// myBinding.Converter = new KilometersToMilesConverter();
    /// 
    /// MyTextBox.SetBinding(TextBox.TextProperty, myBinding);
    /// MyTextBox.DataContext = myPlanet;
    /// </code>
    /// </example>
    [ContentProperty("Path")]
    public partial class Binding : BindingBase
    {
        internal bool _isInStyle;
        private IValueConverter _converter;
#if MIGRATION
        private CultureInfo _culture;
#else
        private string _culture;
#endif
        private object _converterParameter;
        private string _elementName;
        private BindingMode _mode = BindingMode.OneWay;
        private string _xamlPath;
        private PropertyPath _path;
        private RelativeSource _relativeSource;
        private object _source;
        private UpdateSourceTrigger _updateSourceTrigger;
        private bool _validatesOnExceptions;
        private bool _notifyOnValidationError;
        private bool _bindsDirectlyToSource;
        private bool _validatesOnNotifyDataErrors;
        private bool _validatesOnDataErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        public Binding() 
        {
            Path = new PropertyPath(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class with an initial
        /// property path for the data source.
        /// </summary>
        /// <param name="path">
        /// The initial property path for the source of the binding.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// path is null.
        /// </exception>
        public Binding(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = new PropertyPath(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class with initial
        /// property values copied from the specified <see cref="Binding"/>.
        /// </summary>
        /// <param name="original">
        /// The <see cref="Binding"/> to copy.
        /// </param>
        public Binding(Binding original)
        {
            if (original != null)
            {
                FallbackValue = original.FallbackValue;
                TargetNullValue = original.TargetNullValue;
                StringFormat = original.StringFormat;

                _isInStyle = original._isInStyle;
                _converter = original._converter;
#if MIGRATION
                _culture = original._culture;
#else
                _culture = original._culture;
#endif
                _converterParameter = original._converterParameter;
                _elementName = original._elementName;
                _mode = original._mode;
                _xamlPath = original._xamlPath;
                _path = original._path;
                _relativeSource = original._relativeSource;
                _source = original._source;
                _updateSourceTrigger = original._updateSourceTrigger;
                _validatesOnExceptions = original._validatesOnExceptions;
                _notifyOnValidationError = original._notifyOnValidationError;
                _bindsDirectlyToSource = original._bindsDirectlyToSource;
                _validatesOnNotifyDataErrors = original._validatesOnNotifyDataErrors;
                _validatesOnDataErrors = original._validatesOnDataErrors;

                _wasModeSetByUserRatherThanDefaultValue = original._wasModeSetByUserRatherThanDefaultValue;
            }
    }

        /// <summary>
        /// Gets or sets the converter object that is called by the binding engine to
        /// modify the data as it is passed between the source and target, or vice versa.
        /// Returns the IValueConverter object that modifies the data.
        /// </summary>
        public IValueConverter Converter
        {
            get { return _converter; }
            set { CheckSealed(); _converter = value; }
        }

#if MIGRATION
        /// <summary>
        /// Gets or sets the culture to be used by the Binding.Converter.
        /// Returns the System.Globalization.CultureInfo used by the Binding.Converter.
        /// </summary>
        public CultureInfo ConverterCulture
        {
            get { return _culture; }
            set { CheckSealed(); _culture = value; }
        }
#else
        /// <summary>
        /// Gets or sets a value that names the language to pass to any converter specified
        /// by the Converter property.
        /// Returns a string that names a language. Interpretation of this value is ultimately up to the converter logic.
        /// </summary>
        public string ConverterLanguage
        {
            get { return _culture; }
            set { CheckSealed(); _culture = value; }
        }
#endif

        /// <summary>
        /// Gets or sets a parameter that can be used in the Converter logic.
        /// Returns a parameter to be passed to the Converter. This can be used in the conversion logic. The default is null.
        /// </summary>
        public object ConverterParameter
        {
            get { return _converterParameter; }
            set { CheckSealed(); _converterParameter = value; }
        }

        /// <summary>
        /// Gets or sets the name of the element to use as the binding source for the Binding.
        /// Returns the value of the Name property or x:Name attribute for the element to bind to. The default is null.
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
            set { CheckSealed(); _elementName = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// Returns one of the BindingMode values.
        /// </summary>
        public BindingMode Mode
        {
            get { return _mode; }
            set 
            {
                CheckSealed();
                _wasModeSetByUserRatherThanDefaultValue = true;
                _mode = value;
            }
        }

        // This is used by the DataGrid due to its behavior when end-user explicitely specifies a
        // "OneWay" binding for one of the columns or if we still have the default value. In fact,
        // if the binding mode is "OneWay", we need to know if it is the default value or if that
        // value has been explicitely set by the user. The behavior is different: the DataGrid will
        // replace the default "OneWay" mode with "TwoWay" unless the user has explicitely set the
        // mode to "OneWay".
        private bool _wasModeSetByUserRatherThanDefaultValue = false;
        
        internal bool INTERNAL_WasModeSetByUserRatherThanDefaultValue()
        {
            return _wasModeSetByUserRatherThanDefaultValue;
        }

        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// Returns the property path for the source of the binding.
        /// </summary>
        public PropertyPath Path
        {
            get { return _path; }
            set 
            { 
                CheckSealed(); 
                _path = value ?? throw new ArgumentNullException(nameof(value)); 
            }
        }

        /// <summary>
        /// Do not use this property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string XamlPath
        {
            get { return _xamlPath; }
            set { CheckSealed(); _xamlPath = value; }
        }

        /// <summary>
        /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
        /// Returns the relative location of the binding source to use. The default is null.
        /// </summary>
        public RelativeSource RelativeSource
        {
            get { return _relativeSource; }
            set { CheckSealed(); _relativeSource = value; }
        }

        /// <summary>
        /// Gets or sets the data source for the binding.
        /// Returns the source object that contains the data for the binding.
        /// </summary>
        public object Source
        {
            get { return _source; }
            set { CheckSealed(); _source = value; }
        }

        /// <summary>
        /// Update type
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return _updateSourceTrigger; }
            set { CheckSealed(); _updateSourceTrigger = value; }
        }

        internal Binding Clone()
        {
            return new Binding(this);
        }

        /// <summary>
        /// Do not use this property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This is unused and will be removed in future releases.")]
        public TemplateInstance TemplateOwner { get; set; }

#region Validation

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report
        /// exception validation errors.
        /// </summary>
        public bool ValidatesOnExceptions
        {
            get { return _validatesOnExceptions; }
            set { CheckSealed(); _validatesOnExceptions = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the System.Windows.FrameworkElement.BindingValidationError
        /// event is raised on validation errors.
        /// </summary>
        public bool NotifyOnValidationError
        {
            get { return _notifyOnValidationError; }
            set { CheckSealed(); _notifyOnValidationError = value; }
        }

        /// <summary>
        /// True to force the property to go through the Validation process when the Binding is set or when the Target is added in the Visual tree.
        /// This way, if the source property has an Invalid value when setting the Binding, it will immediately be marked as Invalid instead of waiting
        /// for a value change that keeps/makes it Invalid (which is what happens on Silverlight).
        /// Defaults to False since it is the behaviour of Silverlight and WPF.
        /// </summary>
        public bool ValidatesOnLoad { get; set; }

        //
        // Summary:
        //     Indicates whether data binding debugging is enabled.
        [OpenSilver.NotImplemented]
        public static bool IsDebuggingEnabled;

        /// <summary>
        /// Gets or sets a value that indicates whether the binding ignores any <see cref="ICollectionView"/>
        /// settings on the data source.
        /// </summary>
        /// <returns>
        /// true if the binding binds directly to the data source; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool BindsDirectlyToSource
        {
            get { return _bindsDirectlyToSource; }
            set { CheckSealed(); _bindsDirectlyToSource = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report validation
        /// errors from an System.ComponentModel.INotifyDataErrorInfo implementation on the
        /// bound data entity.
        /// </summary>
        /// <returns>
        /// true if the binding engine will report System.ComponentModel.INotifyDataErrorInfo
        /// validation errors; otherwise, false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool ValidatesOnNotifyDataErrors
        {
            get { return _validatesOnNotifyDataErrors; }
            set { CheckSealed(); _validatesOnNotifyDataErrors = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report validation
        /// errors from an System.ComponentModel.IDataErrorInfo implementation on the bound
        /// data entity.
        /// </summary>
        /// <returns>
        /// true if the binding engine will report System.ComponentModel.IDataErrorInfo validation
        /// errors; otherwise, false. The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool ValidatesOnDataErrors
        {
            get { return _validatesOnDataErrors; }
            set { CheckSealed(); _validatesOnDataErrors = value; }
        }

        #endregion
    }
}
