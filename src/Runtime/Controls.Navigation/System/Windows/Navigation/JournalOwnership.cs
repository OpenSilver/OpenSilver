//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Used by the <see cref="Frame"/> to indicate what type of journal it should use.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public enum JournalOwnership
    {
        /// <summary>
        /// Indicates that the <see cref="Frame"/> should walk up the visual tree to determine if it is top-level.
        /// If it is top-level, this is the same as <see cref="JournalOwnership.UsesParentJournal"/>.
        /// If it is not top-level, this is the same as <see cref="JournalOwnership.OwnsJournal"/>.
        /// </summary>
        Automatic,

        /// <summary>
        /// Indicates that the <see cref="Frame"/> should keep its own journal and not integrate with the browser.
        /// This option can be set on any <see cref="Frame"/>
        /// </summary>
        OwnsJournal,

        /// <summary>
        /// Indicates that the <see cref="Frame"/> should integrate with the browser journal.  If this <see cref="Frame"/> is not
        /// a top-level Frame, this is not valid and an exception will be thrown.
        /// </summary>
        UsesParentJournal
    }
}
