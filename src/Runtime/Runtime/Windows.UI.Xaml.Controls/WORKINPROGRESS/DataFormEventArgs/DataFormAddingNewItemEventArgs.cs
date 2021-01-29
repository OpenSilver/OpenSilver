﻿//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if WORKINPROGRESS
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Event args for the AddingNewItem event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormAddingNewItemEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormAddingNewItemEventArgs.
        /// </summary>
        public DataFormAddingNewItemEventArgs()
        {
        }
    }
}
#endif