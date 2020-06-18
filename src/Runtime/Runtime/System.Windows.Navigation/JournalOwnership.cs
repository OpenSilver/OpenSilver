

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
    /// Specifies the type of journal used by the frame.
    /// </summary>
    public enum JournalOwnership
    {
        /// <summary>
        /// If the System.Windows.Controls.Frame control is a top-level frame, it integrates
        /// with the browser journal; otherwise, it maintains its own journal.
        /// </summary>
        Automatic = 0,
        
        /// <summary>
        /// The System.Windows.Controls.Frame maintains its own journal. This option
        /// can be used with any System.Windows.Controls.Frame.
        /// </summary>
        OwnsJournal = 1,
      
        /// <summary>
        /// The System.Windows.Controls.Frame integrates with the browser journal. This
        /// option can be used only with a top-level System.Windows.Controls.Frame; otherwise,
        /// an exception is thrown.
        /// </summary>
        UsesParentJournal = 2,
    }
}