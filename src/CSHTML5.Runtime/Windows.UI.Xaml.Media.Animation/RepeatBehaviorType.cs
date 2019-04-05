
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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Specifies the repeat mode that a Windows.UI.Xaml.Media.Animation.RepeatBehavior
    /// raw value represents.
    /// </summary>
    public enum RepeatBehaviorType
    {
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat for a fixed number of complete runs.
        /// </summary>
        Count = 0,
        //// <summary>
        //// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        //// the timeline should repeat for a time duration, which might result in an
        //// animation terminating part way through.
        //// </summary>
        //Duration = 1, //todo: implement the duration for repeatBehavior.
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat indefinitely.
        /// </summary>
        Forever = 2,
    }
}
