// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// The source of a ValidationSummaryItem, indicating the PropertyName and/or Control.
    /// </summary>
    public class ValidationSummaryItemSource
    {
        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItemSource class.
        /// </summary>
        /// <param name="propertyName">The name of the property associated with this error.</param>
        public ValidationSummaryItemSource(string propertyName) : this(propertyName, null)
        { }

        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItemSource class.
        /// </summary>
        /// <param name="propertyName">The name of the property associated with this error.</param>
        /// <param name="control">The control associated with this error.</param>
        public ValidationSummaryItemSource(string propertyName, Control control)
        {
            this.PropertyName = propertyName;
            this.Control = control;
        }

        /// <summary>
        /// Gets the PropertyName.
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Control.
        /// </summary>
        public Control Control
        {
            get;
            private set;
        }

        /// <summary>
        /// Implements the equality check against the PropertyName and Control.
        /// </summary>
        /// <param name="obj">The ValidationSummaryItem being compared.</param>
        /// <returns>A value indicating whether the two references are equal in value.</returns>
        public override bool Equals(object obj)
        {
            ValidationSummaryItemSource other = obj as ValidationSummaryItemSource;
            if (other == null)
            {
                return false;
            }
            return this.PropertyName == other.PropertyName && this.Control == other.Control;
        }

        /// <summary>
        /// Returns a HashCode based on the PropertyName and Control Name
        /// </summary>
        /// <returns>The hash value of the ValidationSummaryItemSource.</returns>
        public override int GetHashCode()
        {
            string sourceString = this.PropertyName + "." + this.Control.Name;
            return sourceString.GetHashCode();
        }
    }
}
