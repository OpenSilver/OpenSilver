//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    /// <summary>
    /// Event args for the EditEnded event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormEditEndedEventArgs.
        /// </summary>
        /// <param name="editAction">The edit action.</param>
        public DataFormEditEndedEventArgs(DataFormEditAction editAction)
        {
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the edit action.
        /// </summary>
        public DataFormEditAction EditAction
        {
            get;
            private set;
        }
    }
}
