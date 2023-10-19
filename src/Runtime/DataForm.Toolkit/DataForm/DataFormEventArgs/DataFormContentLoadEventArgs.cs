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
    /// Event args for the ContentLoaded and ContentLoading events.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormContentLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormContentLoadEventArgs.
        /// </summary>
        /// <param name="content">The content that was loaded or will be loaded.</param>
        /// <param name="mode">The mode of the DataForm.</param>
        public DataFormContentLoadEventArgs(FrameworkElement content, DataFormMode mode)
        {
            this.Content = content;
            this.Mode = mode;
        }

        /// <summary>
        /// Gets the content that was loaded or will be loaded.
        /// </summary>
        public FrameworkElement Content
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the mode of the DataForm.
        /// </summary>
        public DataFormMode Mode
        {
            get;
            private set;
        }
    }
}
