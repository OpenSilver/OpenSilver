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
    /// Event args for the AutoGeneratingField event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormAutoGeneratingFieldEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormAutoGeneratingFieldEventArgs.
        /// </summary>
        /// <param name="propertyName">The name of the property for the field being generated.</param>
        /// <param name="propertyType">The type of the property for the field being generated.</param>
        /// <param name="field">The field being generated.</param>
        public DataFormAutoGeneratingFieldEventArgs(string propertyName, Type propertyType, DataField field)
        {
            this.Field = field;
            this.PropertyName = propertyName;
            this.PropertyType = propertyType;
        }

        /// <summary>
        /// Gets or sets the field to be used.
        /// </summary>
        public DataField Field
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the property for which this field is being generated.
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the property for which this field is being generated.
        /// </summary>
        public Type PropertyType
        {
            get;
            private set;
        }
    }
}
