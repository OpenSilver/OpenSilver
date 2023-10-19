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
    /// Event args for the EditEnding event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormEditEndingEventArgs.
        /// </summary>
        /// <param name="editAction">The edit action.</param>
        public DataFormEditEndingEventArgs(DataFormEditAction editAction)
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
