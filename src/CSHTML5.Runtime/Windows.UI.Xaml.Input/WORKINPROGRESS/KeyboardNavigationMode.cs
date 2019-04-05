
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

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if WORKINPROGRESS
    // Summary:
    //     Specifies the tabbing behavior across tab stops for a Silverlight tabbing
    //     sequence within a container.
    public enum KeyboardNavigationMode
    {
        // Summary:
        //     Tab indexes are considered on the local subtree only inside this container.
        Local = 0,
        //
        // Summary:
        //     Focus returns to the first or the last keyboard navigation stop inside of
        //     a container when the first or last keyboard navigation stop is reached.
        Cycle = 1,
        //
        // Summary:
        //     The container and all of its child elements as a whole receive focus only
        //     once.
        Once = 2,
    }
#endif
}