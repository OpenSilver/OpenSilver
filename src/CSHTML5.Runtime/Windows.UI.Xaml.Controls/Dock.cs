
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies the Dock position of a child element that
    /// is inside a DockPanel.
    /// </summary>
    public enum Dock
    {
        /// <summary>
        /// A child element that is positioned on the left side of the DockPanel.
        /// </summary>
        Left = 0,
        /// <summary>
        /// A child element that is positioned at the top of the DockPanel.
        /// </summary>
        Top = 1,
        /// <summary>
        /// A child element that is positioned on the right side of the DockPanel.
        /// </summary>
        Right = 2,
        /// <summary>
        /// A child element that is positioned at the bottom of the DockPanel.
        /// </summary>
        Bottom = 3,
    }
}