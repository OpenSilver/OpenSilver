//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Encapsulates metadata info for a given property.
    /// </summary>
    internal class ValidationMetadata : INotifyPropertyChanged
    {
        private string _caption;
        private string _description;
        private bool _isRequired;

        /// <summary>
        /// Gets or sets a value indicating whether the field is required
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return this._isRequired;
            }

            set
            {
                if (this._isRequired != value)
                {
                    this._isRequired = value;
                    this.NotifyPropertyChanged("IsRequired");
                }
            }
        }

        /// <summary>
        /// Gets or sets the property description
        /// </summary>
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        public string Caption
        {
            get
            {
                return this._caption;
            }

            set
            {
                if (this._caption != value)
                {
                    this._caption = value;
                    this.NotifyPropertyChanged("Caption");
                }
            }
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
