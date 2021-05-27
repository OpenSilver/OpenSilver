//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents all of the validation items associated with a given input control
    /// </summary>
    internal class ValidationItemCollection : ObservableCollection<ValidationSummaryItem>
    {
        /// <summary>
        /// Clears errors of the given source type
        /// </summary>
        /// <param name="errorType">The type of the error (Entity or Property)</param>
        internal void ClearErrors(ValidationSummaryItemType errorType)
        {
            // Clear entity errors
            ValidationItemCollection errorsToRemove = new ValidationItemCollection();
            foreach (ValidationSummaryItem error in this)
            {
                if (error != null && error.ItemType == errorType)
                {
                    errorsToRemove.Add(error);
                }
            }
            foreach (ValidationSummaryItem error in errorsToRemove)
            {
                this.Remove(error);
            }
        }

        /// <summary>
        /// Clears all the ValidationSummaryItemTypes from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // Manually removes each item at a time so that the "OldItems" parameter is filled
            while (this.Count > 0)
            {
                this.RemoveAt(0);
            }
        }
    }
}