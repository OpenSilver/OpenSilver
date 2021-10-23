

/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
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
