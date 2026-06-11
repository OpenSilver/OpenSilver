
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

using OpenSilver.Internal;
using OpenSilver.Internal.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows;

/// <summary>
/// Implements a markup extension that supports responsive values based on the identified device type 
/// (Mobile, Tablet or Desktop).
/// </summary>
public sealed class ResponsiveExtension : MarkupExtension
{
    private enum SourceProperties : byte
    {
        None,
        RelativeSource,
        ElementName,
        Source,
    }

    private object _mobile = DependencyProperty.UnsetValue;
    private object _tablet = DependencyProperty.UnsetValue;
    private object _desktop = DependencyProperty.UnsetValue;
    private ResponsiveThreshold? _threshold;
    private SourceProperties _sourceInUse;
    private FrameworkElement _source;
    private string _elementName;
    private RelativeSource _relativeSource;
    private bool _isSealed;
    private DynamicValueConverter _dynamicConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponsiveExtension"/> class.
    /// </summary>
    public ResponsiveExtension() { }

    internal ResponsiveExtension(ResponsiveExtension original)
    {
        Debug.Assert(original is not null);

        _mobile = original._mobile;
        _tablet = original._tablet;
        _desktop = original._desktop;
        _threshold = original._threshold;
        _sourceInUse = original._sourceInUse;
        _source = original._source;
        _elementName = original._elementName;
        _relativeSource = original._relativeSource;
    }

    /// <summary>
    /// Gets or sets the value to use when the device is identified as mobile.
    /// </summary>
    /// <returns>
    /// The value for mobile devices.
    /// </returns>
    public object Mobile
    {
        get => _mobile;
        set
        {
            CheckSealed();
            _mobile = value;
        }
    }

    /// <summary>
    /// Gets or sets the value to use when the device is identified as tablet.
    /// </summary>
    /// <returns>
    /// The value for tablet devices.
    /// </returns>
    public object Tablet
    {
        get => _tablet;
        set
        {
            CheckSealed();
            _tablet = value;
        }
    }

    /// <summary>
    /// Gets or sets the value to use when the device is identified as desktop.
    /// </summary>
    /// <returns>
    /// The value for desktop devices.
    /// </returns>
    public object Desktop
    {
        get => _desktop;
        set
        {
            CheckSealed();
            _desktop = value;
        }
    }

    /// <summary>
    /// Gets or sets the responsive threshold that determines the breakpoints for device types.
    /// </summary>
    /// <returns>
    /// The <see cref="ResponsiveThreshold"/> used to distinguish between mobile, tablet and desktop devices.
    /// If null, the default thresholds defined by <see cref="ResponsiveThreshold.Default"/> are used. The 
    /// default is null.
    /// </returns>
    public ResponsiveThreshold? Threshold
    {
        get => _threshold;
        set
        {
            CheckSealed();
            _threshold = value;
        }
    }

    /// <summary>
    /// Gets or sets the element whose width is used to evaluate the responsive breakpoints.
    /// </summary>
    /// <returns>
    /// The element to measure.
    /// </returns>
    public FrameworkElement Source
    {
        get => _source;
        set
        {
            CheckSealed();

            if (_sourceInUse == SourceProperties.None || _sourceInUse == SourceProperties.Source)
            {
                _source = value;
                _sourceInUse = value is null ? SourceProperties.None : SourceProperties.Source;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(Strings.ResponsiveExtensionConflict, SourceProperties.Source, _sourceInUse));
            }
        }
    }

    /// <summary>
    /// Gets or sets the name of the element whose width is used to evaluate the responsive breakpoints.
    /// </summary>
    /// <returns>
    /// The name of the element to measure. When set, the width of that element (rather than the width of the
    /// <see cref="Window"/>) is compared against the <see cref="Threshold"/> to select between <see cref="Mobile"/>,
    /// <see cref="Tablet"/> and <see cref="Desktop"/>. If both <see cref="ElementName"/> and <see cref="RelativeSource"/>
    /// are set, <see cref="ElementName"/> takes precedence. If the name cannot be resolved to a <see cref="FrameworkElement"/>,
    /// the width of the <see cref="Window"/> is used. The default is null. Note that only the width is considered.
    /// </returns>
    public string ElementName
    {
        get => _elementName;
        set
        {
            CheckSealed();

            if (_sourceInUse == SourceProperties.None || _sourceInUse == SourceProperties.ElementName)
            {
                _elementName = value;
                _sourceInUse = value is null ? SourceProperties.None : SourceProperties.ElementName;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(Strings.ResponsiveExtensionConflict, SourceProperties.ElementName, _sourceInUse));
            }
        }
    }

    /// <summary>
    /// Gets or sets the location of the element whose width is used to evaluate the responsive breakpoints, relative
    /// to the element on which the extension is applied.
    /// </summary>
    /// <returns>
    /// A <see cref="Data.RelativeSource"/> describing the element to measure. <see cref="RelativeSourceMode.Self"/>
    /// uses the element the value is applied to, <see cref="RelativeSourceMode.TemplatedParent"/> uses the templated
    /// parent, and <see cref="RelativeSourceMode.FindAncestor"/> walks up the tree using
    /// <see cref="RelativeSource.AncestorType"/> and <see cref="RelativeSource.AncestorLevel"/>. When set, the
    /// width of the resolved element (rather than the width of the <see cref="Window"/>) is compared against the
    /// <see cref="Threshold"/>. If the source cannot be resolved to a <see cref="FrameworkElement"/>, the width of the
    /// <see cref="Window"/> is used. If both <see cref="ElementName"/> and <see cref="RelativeSource"/> are set,
    /// <see cref="ElementName"/> takes precedence. The default is null. Note that only the width is considered.
    /// </returns>
    public RelativeSource RelativeSource
    {
        get => _relativeSource;
        set
        {
            CheckSealed();

            if (_sourceInUse == SourceProperties.None || _sourceInUse == SourceProperties.RelativeSource)
            {
                _relativeSource = value;
                _sourceInUse = value is null ? SourceProperties.None : SourceProperties.RelativeSource;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(Strings.ResponsiveExtensionConflict, SourceProperties.RelativeSource, _sourceInUse));
            }
        }
    }

    /// <summary>
    /// Returns an object that should be set on the property where this extension is applied. For 
    /// <see cref="ResponsiveExtension"/>, this is <see cref="Mobile"/>, <see cref="Tablet"/> or 
    /// <see cref="Desktop"/> depending on the current <see cref="Window"/> width.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// The object to set on the property where the extension is applied. Rather than the actual 
    /// value, this will be an expression that will be evaluated at a later time.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        DependencyObject d = null;
        DependencyProperty dp = null;

        if (serviceProvider is not null)
        {
            // ResponsiveExtensions are not allowed On CLR props except for Setter
            BindingBase.CheckCanReceiveMarkupExtension(this, serviceProvider, out d, out dp);
        }

        return CreateResponsiveExpression(d, dp);
    }

    internal ResponsiveExpression CreateResponsiveExpression(DependencyObject d, DependencyProperty dp)
    {
        Seal();

        object mobile = ConvertHelper(_mobile, dp);
        object tablet = ConvertHelper(_tablet, dp);
        object desktop = ConvertHelper(_desktop, dp);

        return new ResponsiveExpression(
            GetValue(mobile, tablet, desktop),
            GetValue(tablet, desktop, mobile),
            GetValue(desktop, tablet, mobile),
            _threshold,
            _source,
            _elementName,
            _relativeSource);

        static object GetValue(object first, object second, object third)
        {
            if (first != DependencyProperty.UnsetValue)
            {
                return first;
            }

            if (second != DependencyProperty.UnsetValue)
            {
                return second;
            }

            if (third != DependencyProperty.UnsetValue)
            {
                return third;
            }

            return DependencyProperty.UnsetValue;
        }
    }

    private DynamicValueConverter DynamicConverter => _dynamicConverter ??= new DynamicValueConverter(false);

    internal object ConvertHelper(object value, DependencyProperty dp)
    {
        if (value != DependencyProperty.UnsetValue && !dp.IsValidValue(value))
        {
            value = DynamicConverter.Convert(value, dp.PropertyType, null, CultureInfo.InvariantCulture);
        }

        return value;
    }

    private void Seal() => _isSealed = true;

    private void CheckSealed()
    {
        if (_isSealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(ResponsiveExtension)));
        }
    }
}
