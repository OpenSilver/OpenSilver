
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
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Windows;

/// <summary>
/// When used as a resource key for a data template, allows the data template to participate in the lookup process.
/// </summary>
public abstract class TemplateKey : ResourceKey, ISupportInitialize
{
    private readonly TemplateType _templateType;
    private object _dataType;
    private bool _initializing;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateKey"/> class with the specified template type.
    /// </summary>
    /// <param name="templateType">
    /// A <see cref="TemplateType"/> value that specifies the type of this template.
    /// </param>
    protected TemplateKey(TemplateType templateType)
    {
        _dataType = null;
        _templateType = templateType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateKey"/> class with the specified parameters.
    /// </summary>
    /// <param name="templateType">
    /// A <see cref="TemplateType"/> value that specifies the type of this template.
    /// </param>
    /// <param name="dataType">
    /// The type for which this template is designed.
    /// </param>
    protected TemplateKey(TemplateType templateType, object dataType)
    {
        if (ValidateDataType(dataType) is Exception ex)
        {
            throw ex;
        }

        _dataType = dataType;
        _templateType = templateType;
    }

    void ISupportInitialize.BeginInit() => _initializing = true;

    void ISupportInitialize.EndInit()
    {
        if (_dataType is null)
        {
            throw new InvalidOperationException(string.Format(Strings.PropertyMustHaveValue, nameof(DataType), GetType().Name));
        }

        _initializing = false;
    }

    /// <summary>
    /// Gets or sets the assembly that contains the template definition.
    /// </summary>
    /// <returns>
    /// The assembly in which the template is defined.
    /// </returns>
    public override Assembly Assembly
    {
        get
        {
            if (_dataType is Type type)
            {
                return type.Assembly;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets or sets the type for which the template is designed.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> that specifies the type of object that the template is used to display, or
    /// a string that specifies the XML tag name for the XML data that the template is used to display.
    /// </returns>
    public object DataType
    {
        get => _dataType;
        set
        {
            if (!_initializing)
            {
                throw new InvalidOperationException(string.Format(Strings.PropertyIsInitializeOnly, nameof(DataType), GetType().Name));
            }
            if (_dataType != null && value != _dataType)
            {
                throw new InvalidOperationException(string.Format(Strings.PropertyIsImmutable, nameof(DataType), GetType().Name));
            }

            if (ValidateDataType(value) is Exception ex)
            {
                throw ex;
            }

            _dataType = value;
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the given instance is identical to this instance of 
    /// <see cref="TemplateKey"/>.
    /// </summary>
    /// <param name="o">
    /// The object to compare for equality.
    /// </param>
    /// <returns>
    /// true if the two instances are identical; otherwise, false.
    /// </returns>
    public override bool Equals(object o)
    {
        return o is TemplateKey key &&
               _templateType == key._templateType &&
               Equals(_dataType, key._dataType);
    }

    /// <summary>
    /// Returns the hash code for this instance of <see cref="TemplateKey"/>.
    /// </summary>
    /// <returns>
    /// The hash code for this instance of <see cref="TemplateKey"/>.
    /// </returns>
    public override int GetHashCode()
    {
        // note that the hash code can change, but only during intialization
        // and only once (DataType can only be changed once, from null to
        // non-null, and that can only happen during [Begin/End]Init).
        // Technically this is still a violation of the "constant during
        // lifetime" rule, however in practice this is acceptable.  It is
        // very unlikely that someone will put a TemplateKey into a hashtable
        // before it is initialized.

        int hashcode = (int)_templateType;

        if (_dataType is not null)
        {
            hashcode += _dataType.GetHashCode();
        }

        return hashcode;
    }

    /// <summary>
    /// Returns a string representation of this <see cref="TemplateKey"/>.
    /// </summary>
    /// <returns>
    /// A string representation of this <see cref="TemplateKey"/>.
    /// </returns>
    public override string ToString()
    {
        return DataType is not null ?
            $"{GetType().Name}({DataType})" :
            $"{GetType().Name}(null)";
    }

    // Validate against these rules
    //  1. dataType must not be null (except at initialization)
    //  2. dataType must be either a Type (object data) or a string (XML tag name)
    internal static Exception ValidateDataType(object dataType, [CallerArgumentExpression(nameof(dataType))] string argName = null)
    {
        Exception result = null;

        if (dataType is null)
        {
            result = new ArgumentNullException(argName);
        }
        else if (dataType is not Type && dataType is not string)
        {
            result = new ArgumentException(string.Format(Strings.MustBeTypeOrString, dataType.GetType().Name), argName);
        }

        return result;
    }

    /// <summary>
    /// Describes the different types of templates that use <see cref="TemplateKey"/>.
    /// </summary>
    protected enum TemplateType
    {
        /// <summary>
        /// A type that is a <see cref="Windows.DataTemplate"/>.
        /// </summary>
        DataTemplate,

        /// <summary>
        /// A type that is a TableTemplate. This is obsolete.
        /// </summary>
        TableTemplate,
    }
}