//-----------------------------------------------------------------------
// <copyright file="PropertyGroupDescription.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Common;

namespace System.Windows.Data
{
    

    /// <summary>
    /// Describes the grouping of items using a property name as the criteria. 
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class PropertyGroupDescription : GroupDescription
    {
#region Private Fields

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

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

#endregion Private Fields

#region Constructors

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the PropertyGroupDescription class.
        /// </summary>
        public PropertyGroupDescription()
        {
            this._stringComparison = StringComparison.Ordinal;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyGroupDescription class.
        /// </summary>
        /// <param name="propertyName">Property name that we want to group by</param>
        public PropertyGroupDescription(string propertyName) : this()
        {
            this._propertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyGroupDescription class.
        /// </summary>
        /// <param name="propertyName">Property name that we want to group by</param>
        /// <param name="converter">Converter applied to the property value before determining the group name</param>
        public PropertyGroupDescription(string propertyName, IValueConverter converter) : this(propertyName)
        {
            this._converter = converter;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyGroupDescription class.
        /// </summary>
        /// <param name="propertyName">Property name that we want to group by</param>
        /// <param name="converter">Converter applied to the property value before determining the group name</param>
        /// <param name="stringComparison">The type of string comparison to use</param>
        public PropertyGroupDescription(string propertyName, IValueConverter converter, StringComparison stringComparison) : this(propertyName, converter)
        {
            this._stringComparison = stringComparison;
        }

#endregion Constructors

#region Public Properties

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets or sets a converter to apply to the property value or the item to 
        /// produce the final value that is used to determine which group(s) an 
        /// item belongs to. 
        /// </summary>
        [DefaultValue(null)]
        public IValueConverter Converter
        {
            get
            {
                return this._converter;
            }

            set
            {
                if (this._converter != value)
                {
                    this._converter = value;
                    this.OnPropertyChanged("Converter");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that is used to determine which 
        /// group(s) an item belongs to. 
        /// </summary>
        [DefaultValue(null)]
        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }

            set
            {
                if (this._propertyName != value)
                {
                    this._propertyName = value;
                    this._cachedType = null;
                    this.OnPropertyChanged("PropertyName");
                }
            }
        }

        /// <summary>
        /// Gets or sets a StringComparison value that specifies the comparison 
        /// between the value of an item (as determined by PropertyName and Converter) 
        /// and the name of a group. 
        /// </summary>
        [DefaultValue(StringComparison.Ordinal)]
        public StringComparison StringComparison
        {
            get
            {
                return this._stringComparison;
            }

            set
            {
                if (this._stringComparison != value)
                {
                    this._stringComparison = value;
                    this.OnPropertyChanged("StringComparison");
                }
            }
        }

#endregion Public Properties

        /// <summary>
        /// Returns the group name(s) for the given item
        /// </summary>
        /// <param name="item">The item to return group names for</param>
        /// <param name="level">The level of grouping</param>
        /// <param name="culture">The CultureInfo to supply to the converter</param>
        /// <returns>The group name(s) for the given item</returns>
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            if (item != null)
            {
                if (this._cachedType == null)
                {
                    this._cachedType = TypeHelper.GetNestedPropertyType(item.GetType(), this._propertyName);
                }

                Exception exception;
                object propertyValue = TypeHelper.GetNestedPropertyValue(item, this._propertyName, this._cachedType, out exception);
                if (exception != null)
                {
                    throw exception;
                }

                if (this.Converter != null)
                {
                    return this.Converter.Convert(propertyValue, typeof(object), null, culture);
                }
                else
                {
                    return propertyValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a value that indicates whether the group name and 
        /// the item name match so that the item belongs to the group.
        /// </summary>
        /// <param name="groupName">The name of the group to check</param>
        /// <param name="itemName">The name of the item to check</param>
        /// <returns>true if the names match and the item belongs to the group; otherwise, false</returns>
        public override bool NamesMatch(object groupName, object itemName)
        {
            string a = groupName as string;
            string b = itemName as string;
            if ((a != null) && (b != null))
            {
                return string.Equals(a, b, this.StringComparison);
            }

            return object.Equals(groupName, itemName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}