// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents an individual validation error.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ValidationSummaryItem : INotifyPropertyChanged
    {
        internal const string PROPERTYNAME_ITEMTYPE = "ItemType";
        private object _context;
        private ValidationSummaryItemType _itemType;
        private string _message;
        private string _messageHeader;

        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItem class.
        /// </summary>
        public ValidationSummaryItem() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItem class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ValidationSummaryItem(string message) : this(message, null, ValidationSummaryItemType.ObjectError, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItem class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageHeader">The header/prefix of the item, such as the property name.</param>
        /// <param name="itemType">The type of error, such as Property or Entity level.</param>
        /// <param name="source">The source of the error message, including the originating control and/or property name.</param>
        /// <param name="context">Context from which the error occurred.  This general property can be used as a container to keep track of the entity, for instance.</param>
        public ValidationSummaryItem(string message, string messageHeader, ValidationSummaryItemType itemType, ValidationSummaryItemSource source, object context)
        {
            this.MessageHeader = messageHeader;
            this.Message = message;
            this.ItemType = itemType;
            this.Context = context;
            this.Sources = new ObservableCollection<ValidationSummaryItemSource>();
            if (source != null)
            {
                this.Sources.Add(source);
            }
        }

        /// <summary>
        /// Gets or sets the object that is the context in which the error occurred.
        /// </summary>
        public object Context
        {
            get
            {
                return this._context;
            }

            set
            {
                if (this._context != value)
                {
                    this._context = value;
                    this.NotifyPropertyChanged("Context");
                }
            }
        }

        /// <summary>
        /// Gets a value that specifies whether the error originated from an object or a property.
        /// </summary>
        public ValidationSummaryItemType ItemType
        {
            get
            {
                return this._itemType;
            }

            set
            {
                if (this._itemType != value)
                {
                    this._itemType = value;
                    // Uses const due to the special case where ItemType changes affect filtering.
                    this.NotifyPropertyChanged(PROPERTYNAME_ITEMTYPE);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text of the error message.
        /// </summary>
        public string Message
        {
            get
            {
                return this._message;
            }

            set
            {
                if (this._message != value)
                {
                    this._message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// Gets the reference to the input control that resulted in this error
        /// </summary>
        public string MessageHeader
        {
            get
            {
                return this._messageHeader;
            }

            set
            {
                if (this._messageHeader != value)
                {
                    this._messageHeader = value;
                    this.NotifyPropertyChanged("MessageHeader");
                }
            }
        }

        /// <summary>
        /// Gets the sources of the error.
        /// </summary>
        public ObservableCollection<ValidationSummaryItemSource> Sources
        {
            get;
            private set;
        }

        /// <summary>
        /// The string representation of the error.
        /// </summary>
        /// <returns>The string representation of the ValidationSummaryItem.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}: {1}", this.MessageHeader, this.Message);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// This event is raised when any of the properties on the object change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
