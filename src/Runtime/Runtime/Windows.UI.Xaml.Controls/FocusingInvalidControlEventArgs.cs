//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the ErrorClicked event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class FocusingInvalidControlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ValidationSummaryItemEventArgs class.
        /// </summary>
        /// <param name="item">The selected ValidationSummaryItem</param>
        /// <param name="target">The target is the ValidationSummaryItemSource that will be focused.</param>
        public FocusingInvalidControlEventArgs(ValidationSummaryItem item, ValidationSummaryItemSource target)
        {
            this.Handled = false;
            this.Item = item;
            this.Target = target;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the focusing was handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the error message string
        /// </summary>
        public ValidationSummaryItem Item
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the target ValidationSummaryItemSource.  If this value is changed, it will be the new current source 
        /// and will be focused.
        /// </summary>
        public ValidationSummaryItemSource Target
        {
            get;
            set;
        }
    }
}