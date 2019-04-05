
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the likelihood that the media engine can play a media source based
    /// on its file type and characteristics.
    /// </summary>
    public enum MediaCanPlayResponse
    {
        /// <summary>
        /// Media engine cannot support the media source.
        /// </summary>
        NotSupported = 0,
        /// <summary>
        /// Media engine might support the media source.
        /// </summary>
        Maybe = 1,
        /// <summary>
        /// Media engine can probably support the media source.
        /// </summary>
        Probably = 2,
    }
}