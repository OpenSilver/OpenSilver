//-----------------------------------------------------------------------
// <copyright file="PropertyGroupDescription.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using OpenSilver.Internal.Data;

namespace System.Windows.Data;

/// <summary>
/// Describes the grouping of items by using a property name as the criteria.
/// </summary>
public class PropertyGroupDescription : GroupDescription
{
    /// <summary>
    /// Cached Type that we store when we are looking for a property value
    /// </summary>
    private Type _cachedType;

    /// <summary>
    /// Private accessor for the Converter
    /// </summary>
    private IValueConverter _converter;

    /// <summary>
    /// Private accessor for the PropertyName
    /// </summary>
    private string _propertyName;

    /// <summary>
    /// Private accessor for the StringComparison
    /// </summary>
    private StringComparison _stringComparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
    /// </summary>
    public PropertyGroupDescription()
    {
        _stringComparison = StringComparison.Ordinal;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class with the specified 
    /// property name.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that specifies which group an item belongs to.
    /// </param>
    public PropertyGroupDescription(string propertyName)
        : this()
    {
        _propertyName = propertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class with the specified 
    /// property name and converter.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that specifies which group an item belongs to. If this parameter is null,
    /// the item itself is passed to the value converter.
    /// </param>
    /// <param name="converter">
    /// An <see cref="IValueConverter"/> object to apply to the property value or the item to produce the 
    /// final value that is used to determine which group(s) an item belongs to. The converter may return 
    /// a collection, which indicates that the items can appear in more than one group.
    /// </param>
    public PropertyGroupDescription(string propertyName, IValueConverter converter)
        : this(propertyName)
    {
        _converter = converter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class with the specified 
    /// property name, converter, and string comparison.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that specifies which group an item belongs to. If this parameter is null, 
    /// the item itself is passed to the value converter.
    /// </param>
    /// <param name="converter">
    /// An <see cref="IValueConverter"/> object to apply to the property value or the item to produce the 
    /// final value that is used to determine which group(s) an item belongs to. The converter may return 
    /// a collection, which indicates that the items can appear in more than one group.
    /// </param>
    /// <param name="stringComparison">
    /// A <see cref="StringComparison"/> value that specifies the comparison between the value of an item 
    /// and the name of a group.
    /// </param>
    public PropertyGroupDescription(string propertyName, IValueConverter converter, StringComparison stringComparison)
        : this(propertyName, converter)
    {
        _stringComparison = stringComparison;
    }

    /// <summary>
    /// Gets or sets a converter to apply to the property value or the item to produce the final value that 
    /// is used to determine which group(s) an item belongs to.
    /// </summary>
    /// <returns>
    /// The converter to apply. The default is null.
    /// </returns>
    [DefaultValue(null)]
    public IValueConverter Converter
    {
        get { return _converter; }
        set
        {
            if (_converter != value)
            {
                _converter = value;
                OnPropertyChanged(nameof(Converter));
            }
        }
    }

    /// <summary>
    /// Gets or sets the name of the property that is used to determine which group(s) an item belongs to.
    /// </summary>
    /// <returns>
    /// The name of the property that is used to determine which group(s) an item belongs to. The default 
    /// is null.
    /// </returns>
    [DefaultValue(null)]
    public string PropertyName
    {
        get { return _propertyName; }
        set
        {
            if (_propertyName != value)
            {
                _propertyName = value;
                _cachedType = null;
                OnPropertyChanged(nameof(PropertyName));
            }
        }
    }

    /// <summary>
    /// Gets or sets a <see cref="System.StringComparison"/> value that specifies the comparison between 
    /// the value of an item (as determined by <see cref="PropertyName"/> and <see cref="Converter"/>) 
    /// and the name of a group.
    /// </summary>
    /// <returns>
    /// An enumeration value that specifies the comparison between the value of an item and the name of 
    /// a group. The default is <see cref="StringComparison.Ordinal"/>.
    /// </returns>
    [DefaultValue(StringComparison.Ordinal)]
    public StringComparison StringComparison
    {
        get { return _stringComparison; }
        set
        {
            if (_stringComparison != value)
            {
                _stringComparison = value;
                OnPropertyChanged(nameof(StringComparison));
            }
        }
    }

    /// <summary>
    /// Returns the group name(s) for the specified item.
    /// </summary>
    /// <param name="item">
    /// The item to return group names for.
    /// </param>
    /// <param name="level">
    /// The level of grouping.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> to supply to the converter.
    /// </param>
    /// <returns>
    /// The group name(s) for the specified item.
    /// </returns>
    public override object GroupNameFromItem(object item, int level, CultureInfo culture)
    {
        if (item is not null)
        {
            _cachedType ??= TypeHelper.GetNestedPropertyType(item.GetType(), _propertyName);

            object propertyValue = TypeHelper.GetNestedPropertyValue(item, _propertyName, _cachedType, out Exception exception);
            if (exception is not null)
            {
                throw exception;
            }

            if (Converter is not null)
            {
                return Converter.Convert(propertyValue, typeof(object), null, culture);
            }
            else
            {
                return propertyValue;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a value that indicates whether the group name and the item name match, which indicates 
    /// that the item belongs to the group.
    /// </summary>
    /// <param name="groupName">
    /// The name of the group to check.
    /// </param>
    /// <param name="itemName">
    /// The name of the item to check.
    /// </param>
    /// <returns>
    /// true if the names match, which indicates that the item belongs to the group; otherwise, false.
    /// </returns>
    public override bool NamesMatch(object groupName, object itemName)
    {
        if (groupName is string a && itemName is string b)
        {
            return string.Equals(a, b, StringComparison);
        }

        return Equals(groupName, itemName);
    }

    private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
}