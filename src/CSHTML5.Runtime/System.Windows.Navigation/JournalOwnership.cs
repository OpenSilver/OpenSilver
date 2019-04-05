
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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