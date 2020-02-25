﻿
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <summary>
        /// Initializes a new instance of the Binding class.
        /// </summary>
        public Binding() { }
        /// <summary>
        /// Initializes a new instance of the Binding class with the given path.
        /// </summary>
        /// <param name="path">The path to the source of the binding.</param>
        public Binding(string path)
        {
            if(!string.IsNullOrWhiteSpace(path))
                Path = new PropertyPath(path);
        }

        /// <summary>
        /// Gets or sets the converter object that is called by the binding engine to
        /// modify the data as it is passed between the source and target, or vice versa.
        /// Returns the IValueConverter object that modifies the data.
        /// </summary>
        public IValueConverter Converter { get; set; }

#if MIGRATION
        /// <summary>
        /// Gets or sets the culture to be used by the Binding.Converter.
        /// Returns the System.Globalization.CultureInfo used by the Binding.Converter.
        /// </summary>
        public CultureInfo ConverterCulture { get; set; }
#else
        /// <summary>
        /// Gets or sets a value that names the language to pass to any converter specified
        /// by the Converter property.
        /// Returns a string that names a language. Interpretation of this value is ultimately up to the converter logic.
        /// </summary>
        public string ConverterLanguage { get; set; }
#endif

        /// <summary>
        /// Gets or sets a parameter that can be used in the Converter logic.
        /// Returns a parameter to be passed to the Converter. This can be used in the conversion logic. The default is null.
        /// </summary>
        public object ConverterParameter { get; set; }

        /// <summary>
        /// Gets or sets the name of the element to use as the binding source for the Binding.
        /// Returns the value of the Name property or x:Name attribute for the element to bind to. The default is null.
        /// </summary>
        public string ElementName { get; set; }

        BindingMode _mode = BindingMode.OneWay;
        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// Returns one of the BindingMode values.
        /// </summary>
        public BindingMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _wasModeSetByUserRatherThanDefaultValue = true;
                _mode = value;
            }
        }

        bool _wasModeSetByUserRatherThanDefaultValue = false; // This is used by the DataGrid due to its behavior when end-user explicitely specifies a "OneWay" binding for one of the columns or if we still have the default value. In fact, if the binding mode is "OneWay", we need to know if it is the default value or if that value has been explicitely set by the user. The behavior is different: the DataGrid will replace the default "OneWay" mode with "TwoWay" unless the user has explicitely set the mode to "OneWay".
        internal bool INTERNAL_WasModeSetByUserRatherThanDefaultValue()
        {
            return _wasModeSetByUserRatherThanDefaultValue;
        }

        PropertyPath _path;
        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// Returns the property path for the source of the binding.
        /// </summary>
        public PropertyPath Path
        {
            get
            {
                if (_path == null)
                {
                    _path = new PropertyPath("");
                }
                return _path;
            }
            set
            {
                _path = value;
                INTERNAL_ComputedPath = value;
            }
        }

        internal PropertyPath INTERNAL_ComputedPath; // If the Source is not specified, it means that we want to bind to the DataContext (eg. <TextBox Text={Binding}/>, so we automatically add "DataContext." to the Path (the result is called "ComputedPath") and we automatically set the Source to the Target of the Binding (the new source is called "ComputedSource") when calling SetBinding.

        /// <summary>
        /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
        /// Returns the relative location of the binding source to use. The default is null.
        /// </summary>
        public RelativeSource RelativeSource { get; set; }

        object _source;
        /// <summary>
        /// Gets or sets the data source for the binding.
        /// Returns the source object that contains the data for the binding.
        /// </summary>
        public object Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                INTERNAL_ComputedSource = value;
            }
        }

        internal object INTERNAL_ComputedSource; // If the Source is not specified, it means that we want to bind to the DataContext (eg. <TextBox Text={Binding}/>, so we automatically add "DataContext." to the Path (the result is called "ComputedPath") and we automatically set the Source to the Target of the Binding (the new source is called "ComputedSource") when calling SetBinding.

        /// <summary>
        /// Update type
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }


        internal Binding Clone()
        {
#if WORKINPROGRESS
            Binding b = new Binding();
            this.CopyTo(b);
            return b;
#else
            Binding b =  new Binding(Path.Path);
            b.Converter = Converter;
#if MIGRATION
            b.ConverterCulture = ConverterCulture;
#else
            b.ConverterLanguage = ConverterLanguage;
#endif
            b.ConverterParameter = ConverterParameter;
            b.ElementName = ElementName; //I don't think people should use this when trying to make a Binding that will be used in different places but we never know.
            b._mode = _mode;
            b._wasModeSetByUserRatherThanDefaultValue = _wasModeSetByUserRatherThanDefaultValue;
            b.RelativeSource = RelativeSource; //I don't think people should use this when trying to make a Binding that will be used in different places but we never know.
            b.Source = Source;
            b.UpdateSourceTrigger = UpdateSourceTrigger;
            b.StringFormat = StringFormat;
            return b;
#endif
        }

        private void CopyTo(Binding target)
        {
            target.Path = new PropertyPath(this.Path == null ? string.Empty : this.Path.Path);
            target.Converter = this.Converter;
#if MIGRATION
            target.ConverterCulture = this.ConverterCulture;
#else
            target.ConverterLanguage = this.ConverterLanguage;
#endif
            target.ConverterParameter = this.ConverterParameter;
            target.ElementName = this.ElementName; //I don't think people should use this when trying to make a Binding that will be used in different places but we never know.
            target._mode = this._mode;
            target._wasModeSetByUserRatherThanDefaultValue = this._wasModeSetByUserRatherThanDefaultValue;
            target.RelativeSource = this.RelativeSource; //I don't think people should use this when trying to make a Binding that will be used in different places but we never know.
            target.Source = this.Source;
            target.UpdateSourceTrigger = this.UpdateSourceTrigger;
            target.StringFormat = this.StringFormat;
        }

        /// <summary>
        /// Do not use this property.
        /// </summary>
        /// <exclude/>
        public TemplateInstance TemplateOwner { get; set; }

#region Validation

#if WORKINPROGRESS

        /// <summary>Gets or sets a value that indicates whether the binding engine will report validation errors from an <see cref="T:System.ComponentModel.IDataErrorInfo" /> implementation on the bound data entity.</summary>
        /// <returns>true if the binding engine will report <see cref="T:System.ComponentModel.IDataErrorInfo" /> validation errors; otherwise, false. The default is false.</returns>
        public bool ValidatesOnDataErrors { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report
        /// exception validation errors.
        /// </summary>
        public bool ValidatesOnExceptions { get; set; }

        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Windows.Data.Binding has already been attached to a target element,
        //     and cannot be modified.
        /// <summary>
        /// Gets or sets a value that indicates whether the System.Windows.FrameworkElement.BindingValidationError
        /// event is raised on validation errors.
        /// </summary>
        public bool NotifyOnValidationError { get; set; }

        /// <summary>
        /// True to force the property to go through the Validation process when the Binding is set or when the Target is added in the Visual tree.
        /// This way, if the source property has an Invalid value when setting the Binding, it will immediately be marked as Invalid instead of waiting
        /// for a value change that keeps/makes it Invalid (which is what happens on Silverlight).
        /// Defaults to False since it is the behaviour of Silverlight and WPF.
        /// </summary>
        public bool ValidatesOnLoad { get; set; }

#endregion

#if WORKINPROGRESS
        public Binding(Binding original)
        {
            if(original != null)
            {
                original.CopyTo(this);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the binding ignores any System.ComponentModel.ICollectionView
        /// settings on the data source.
        /// </summary>
        public bool BindsDirectlyToSource { get; set; }

        public bool ValidatesOnNotifyDataErrors { get; set; }
#endif
    }
}
