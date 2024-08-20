
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

using System.ComponentModel;
using System.Windows.Markup;
using System.Globalization;

namespace System.Windows.Data;

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
[ContentProperty(nameof(Path))]
public class Binding : BindingBase
{
    private IValueConverter _converter;
    private CultureInfo _culture;
    private object _converterParameter;
    private string _elementName;
    private string _xamlPath;
    private PropertyPath _path;
    private RelativeSource _relativeSource;
    private object _source;
    private bool _bindsDirectlyToSource;

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
    /// Initializes a new instance of the <see cref="Binding"/> class with initial property 
    /// values copied from the specified <see cref="Binding"/>.
    /// </summary>
    /// <param name="original">
    /// The <see cref="Binding"/> to copy.
    /// </param>
    public Binding(Binding original)
        : base(original)
    {
        if (original != null)
        {
            _converter = original._converter;
            _culture = original._culture;
            _converterParameter = original._converterParameter;
            _elementName = original._elementName;
            _xamlPath = original._xamlPath;
            _path = original._path;
            _relativeSource = original._relativeSource;
            _source = original._source;
            _bindsDirectlyToSource = original._bindsDirectlyToSource;
        }
    }

    /// <summary>
    /// Gets or sets the converter object that is called by the binding engine to modify the data as 
    /// it is passed between the source and target, or vice versa.
    /// </summary>
    /// <returns>
    /// The <see cref="IValueConverter"/> object that modifies the data.
    /// </returns>
    public IValueConverter Converter
    {
        get { return _converter; }
        set { CheckSealed(); _converter = value; }
    }

    /// <summary>
    /// Gets or sets the culture to be used by the <see cref="Converter"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="CultureInfo"/> used by the <see cref="Converter"/>.
    /// </returns>
    public CultureInfo ConverterCulture
    {
        get { return _culture; }
        set { CheckSealed(); _culture = value; }
    }

    /// <summary>
    /// Gets or sets a parameter that can be used in the <see cref="Converter"/> logic.
    /// </summary>
    /// <returns>
    /// A parameter to be passed to the <see cref="Converter"/>. This can be used in the conversion 
    /// logic. The default is null.
    /// </returns>
    public object ConverterParameter
    {
        get { return _converterParameter; }
        set { CheckSealed(); _converterParameter = value; }
    }

    /// <summary>
    /// Gets or sets the name of the element to use as the binding source object.
    /// </summary>
    /// <returns>
    /// The value of the <see cref="FrameworkElement.Name"/> property or x:Name Attribute of the element
    /// to bind to. The default is null.
    /// </returns>
    public string ElementName
    {
        get { return _elementName; }
        set { CheckSealed(); _elementName = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates the direction of the data flow in the binding.
    /// </summary>
    /// <returns>
    /// One of the <see cref="BindingMode"/> values. The default is <see cref="BindingMode.OneWay"/>.
    /// </returns>
    public BindingMode Mode
    {
        get
        {
            return GetFlagsWithinMask(PrivateFlags.PropagationMask) switch
            {
                PrivateFlags.TwoWay => BindingMode.TwoWay,
                PrivateFlags.OneTime => BindingMode.OneTime,
                _ => BindingMode.OneWay,
            };
        }
        set
        {
            CheckSealed();
            ChangeFlagsWithinMask(PrivateFlags.PropagationMask, FlagsFrom(value));
        }
    }

    /// <summary>
    /// Gets or sets the path to the binding source property.
    /// </summary>
    /// <returns>
    /// The property path for the source of the binding. See <see cref="PropertyPath"/>.
    /// </returns>
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
    /// </summary>
    /// <returns>
    /// The relative location of the binding source to use. The default is null.
    /// </returns>
    public RelativeSource RelativeSource
    {
        get { return _relativeSource; }
        set { CheckSealed(); _relativeSource = value; }
    }

    /// <summary>
    /// Gets or sets the data source for the binding.
    /// </summary>
    /// <returns>
    /// The source object that contains the data for the binding.
    /// </returns>
    public object Source
    {
        get { return _source; }
        set { CheckSealed(); _source = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines the timing of binding source updates for two-way bindings.
    /// </summary>
    /// <returns>
    /// A value that determines when the binding source is updated. The default is <see cref="UpdateSourceTrigger.Default"/>.
    /// </returns>
    public UpdateSourceTrigger UpdateSourceTrigger
    {
        get { return UpdateSourceTriggerInternal; }
        set
        {
            CheckSealed();
            UpdateSourceTriggerInternal = value;
        }
    }

    #region Validation

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report exception validation errors.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report exception validation errors; otherwise, false. The default is false.
    /// </returns>
    public bool ValidatesOnExceptions
    {
        get { return TestFlag(PrivateFlags.ValidatesOnExceptions); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.ValidatesOnExceptions, value);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="FrameworkElement.BindingValidationError"/>
    /// event is raised on validation errors.
    /// </summary>
    /// <returns>
    /// true if the <see cref="FrameworkElement.BindingValidationError"/> event is raised; otherwise, false.
    /// The default is false.
    /// </returns>
    public bool NotifyOnValidationError
    {
        get { return TestFlag(PrivateFlags.NotifyOnValidationError); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.NotifyOnValidationError, value);
        }
    }

    /// <summary>
    /// Indicates whether data binding debugging is enabled.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static bool IsDebuggingEnabled;

    /// <summary>
    /// Gets or sets a value that indicates whether the binding ignores any <see cref="ICollectionView"/>
    /// settings on the data source.
    /// </summary>
    /// <returns>
    /// true if the binding binds directly to the data source; otherwise, false.
    /// </returns>
    public bool BindsDirectlyToSource
    {
        get { return _bindsDirectlyToSource; }
        set { CheckSealed(); _bindsDirectlyToSource = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report validation
    /// errors from an <see cref="INotifyDataErrorInfo"/> implementation on the bound data entity.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report <see cref="INotifyDataErrorInfo"/> validation errors; 
    /// otherwise, false. The default is true.
    /// </returns>
    public bool ValidatesOnNotifyDataErrors
    {
        get { return ValidatesOnNotifyDataErrorsInternal; }
        set
        {
            CheckSealed();
            ValidatesOnNotifyDataErrorsInternal = value;
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report validation
    /// errors from an <see cref="IDataErrorInfo"/> implementation on the bound data entity.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report <see cref="IDataErrorInfo"/> validation errors;
    /// otherwise, false. The default is false.
    /// </returns>
    public bool ValidatesOnDataErrors
    {
        get { return TestFlag(PrivateFlags.ValidatesOnDataErrors); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.ValidatesOnDataErrors, value);
        }
    }

    #endregion

    internal sealed override BindingExpressionBase CreateBindingExpressionOverride(
        DependencyObject target, DependencyProperty dp, BindingExpressionBase owner)
        => BindingExpression.CreateBindingExpression(dp, this, owner);
}
